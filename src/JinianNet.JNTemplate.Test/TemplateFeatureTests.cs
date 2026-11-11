using System.Text;
using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Parsers;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 模板功能级测试（$load、$include、JSON 标签）
    /// </summary>
    public class TemplateFeatureTests : IDisposable
    {
        private readonly string workDir;

        public TemplateFeatureTests()
        {
            workDir = Path.Combine(Path.GetTempPath(), "jntemplate-feature-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(workDir);
            File.WriteAllText(Path.Combine(workDir, "part.txt"), "PART", Encoding.UTF8);
            File.WriteAllText(Path.Combine(workDir, "include.txt"), "INCLUDE", Encoding.UTF8);
        }

        public void Dispose()
        {
            if (Directory.Exists(workDir))
            {
                Directory.Delete(workDir, true);
            }
        }

        private IEngine CreateEngine(bool compile)
        {
            var builder = compile
                ? new EngineBuilder().UseCompileEngine()
                : new EngineBuilder().UseInterpretationEngine();
            var engine = builder.Build();
            engine.AppendResourcePath(workDir);
            return engine;
        }

        /// <summary>
        /// $load 编译模式
        /// </summary>
        [Fact]
        public void TestLoadCompile()
        {
            var engine = CreateEngine(true);

            Assert.Equal("aPART", engine.Parse("a$load(\"part.txt\")", (Action<TemplateContext>)null!));

            // 动态路径
            Assert.Equal("PART", engine.Parse("$load(path)", ctx => ctx.Set("path", "part.txt", typeof(string))));

            // 文件不存在时编译失败
            Assert.Throws<CompileException>(() => engine.Compile("load-missing-" + Guid.NewGuid().ToString("N"), "$load(\"missing.txt\")"));
        }

        /// <summary>
        /// $load 解释模式
        /// </summary>
        [Fact]
        public void TestLoadInterpret()
        {
            var engine = CreateEngine(false);

            Assert.Equal("aPART", engine.Parse("a$load(\"part.txt\")", (Action<TemplateContext>)null!));
            Assert.Equal("PART", engine.Parse("$load(path)", ctx => ctx.Set("path", "part.txt", null!)));

            // 路径为空或找不到文件时不输出
            Assert.Equal(string.Empty, engine.Parse("$load(path)", (Action<TemplateContext>)null!));
            Assert.Equal(string.Empty, engine.Parse("$load(\"missing.txt\")", (Action<TemplateContext>)null!));
        }

        /// <summary>
        /// $include 编译模式
        /// </summary>
        [Fact]
        public void TestIncludeCompile()
        {
            var engine = CreateEngine(true);

            Assert.Equal("INCLUDE", engine.Parse("$include(\"include.txt\")", (Action<TemplateContext>)null!));
            Assert.Equal("INCLUDE", engine.Parse("$include(path)", ctx => ctx.Set("path", "include.txt", typeof(string))));

            // 文件不存在时输出提示
            var result = engine.Parse("$include(\"missing.txt\")", (Action<TemplateContext>)null!);
            Assert.Contains("not found", result);
        }

        /// <summary>
        /// $include 解释模式
        /// </summary>
        [Fact]
        public void TestIncludeInterpret()
        {
            var engine = CreateEngine(false);

            Assert.Equal("INCLUDE", engine.Parse("$include(\"include.txt\")", (Action<TemplateContext>)null!));
            Assert.Equal("INCLUDE", engine.Parse("$include(path)", ctx => ctx.Set("path", "include.txt", null!)));
        }

        /// <summary>
        /// JSON 标签解析与解释执行
        /// </summary>
        [Fact]
        public void TestJsonParse()
        {
            var engine = CreateEngine(false);
            var ctx = engine.CreateContext();
            var visitor = new JsonVisitor();

            var tags = ctx.Lexer("${{ \"a\" : 1 }}");
            Assert.Single(tags);
            var jsonTag = Assert.IsType<JsonTag>(tags[0]);
            Assert.Single(jsonTag.Dict);
            Assert.Equal(typeof(Dictionary<object, object>), visitor.GuessType(jsonTag, null!));

            // 解释执行
            var dict = Assert.IsType<Dictionary<object, object>>(visitor.Execute(jsonTag, ctx));
            Assert.Single(dict);
            Assert.Contains("a", Assert.Single(dict.Keys)!.ToString()!);

            // 空字典
            var emptyTag = new JsonTag();
            var emptyResult = Assert.IsType<Dictionary<object, object>>(visitor.Execute(emptyTag, ctx));
            Assert.Empty(emptyResult);
        }

        /// <summary>
        /// JSON 标签在模板中的编译与执行
        /// </summary>
        [Fact]
        public void TestJsonTag()
        {
            // 解释模式
            var interpretEngine = CreateEngine(false);
            var interpreted = interpretEngine.CreateTemplate("${{ \"a\" : 1 }}").Render();
            Assert.Contains("Dictionary", interpreted);

            // 编译模式
            var compileEngine = CreateEngine(true);
            var compiled = compileEngine.Compile("json-" + Guid.NewGuid().ToString("N"), "${{ \"a\" : 1 }}");
            using (var writer = new StringWriter())
            {
                compiled.Render(writer, compileEngine.CreateContext());
                Assert.Contains("Dictionary", writer.ToString());
            }
        }

        /// <summary>
        /// $body 标签在解释模式下的执行
        /// </summary>
        [Fact]
        public void TestBodyTag()
        {
            var engine = CreateEngine(false);
            var ctx = engine.CreateContext();

            var tags = ctx.Lexer("$body");
            var body = Assert.IsType<BodyTag>(Assert.Single(tags));

            // 无子标签
            Assert.Null(ctx.Execute(body));

            var first = new TextTag();
            first.FirstToken = new Token(TokenKind.TextData, "a");
            body.AddChild(first);
            Assert.Equal("a", ctx.Execute(body));

            var second = new TextTag();
            second.FirstToken = new Token(TokenKind.TextData, "b");
            body.AddChild(second);
            Assert.Equal("ab", ctx.Execute(body));
        }

        private void CreateLayoutFiles(string text)
        {
            File.WriteAllText(Path.Combine(workDir, "layout.txt"), "H[$body]F", Encoding.UTF8);
            File.WriteAllText(Path.Combine(workDir, "page.txt"), "$layout(\"layout.txt\")" + text, Encoding.UTF8);
        }

        /// <summary>
        /// $layout 编译模式
        /// </summary>
        [Fact]
        public void TestLayoutCompile()
        {
            CreateLayoutFiles("BODY");
            var engine = CreateEngine(true);

            Assert.Equal("H[BODY]F", engine.Parse(File.ReadAllText(Path.Combine(workDir, "page.txt")), (Action<TemplateContext>)null!));
        }

        /// <summary>
        /// $layout 解释模式
        /// </summary>
        [Fact]
        public void TestLayoutInterpret()
        {
            CreateLayoutFiles("BODY");
            var engine = CreateEngine(false);

            Assert.Equal("H[BODY]F", engine.Parse(File.ReadAllText(Path.Combine(workDir, "page.txt")), (Action<TemplateContext>)null!));

            // 多个子标签
            CreateLayoutFiles("A${1}B");
            var engine2 = CreateEngine(false);
            engine2.Clean();
            Assert.Equal("H[A1B]F", engine2.Parse(File.ReadAllText(Path.Combine(workDir, "page.txt")), (Action<TemplateContext>)null!));
        }
    }
}
