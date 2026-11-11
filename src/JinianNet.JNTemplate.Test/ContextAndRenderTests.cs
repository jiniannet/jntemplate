using System.Reflection.Emit;
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Parsers;
using StringReader = JinianNet.JNTemplate.Resources.StringReader;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 模板上下文与渲染流程测试
    /// </summary>
    public class ContextAndRenderTests
    {
        private class HiddenModel
        {
            public int Id { get; set; }
        }

        private static ITag CreateTextTag(string text)
        {
            var tag = new TextTag();
            tag.FirstToken = new Token(TokenKind.TextData, text);
            tag.LastToken = tag.FirstToken;
            return tag;
        }

        /// <summary>
        /// 直接渲染标签集合
        /// </summary>
        [Fact]
        public void TestRenderTags()
        {
            var ctx = new EngineBuilder().Build().CreateContext();
            var collection = new TagCollection();
            collection.Add(CreateTextTag("a"));
            collection.Add(CreateTextTag("b"));

            using (var writer = new StringWriter())
            {
                ctx.Render(writer, collection);
                Assert.Equal("ab", writer.ToString());
            }

            // 空集合与 null 集合
            using (var writer = new StringWriter())
            {
                ctx.Render(writer, null!);
                ctx.Render(writer, new TagCollection());
                Assert.Equal(string.Empty, writer.ToString());
            }

            Assert.Throws<ArgumentNullException>(() => ctx.Render(null!, collection));
        }

        /// <summary>
        /// 渲染时的异常处理：不抛出时输出标签源码
        /// </summary>
        [Fact]
        public void TestRenderTagException()
        {
            var engine = new EngineBuilder().Build();
            engine.Register(new DynamicVisitor(typeof(TestTag), (tag, c) => throw new TemplateException("template error")));
            engine.Register(new DynamicVisitor(typeof(ErrorTestTag), (tag, c) => throw new InvalidOperationException("runtime error")));

            var ctx = engine.CreateContext();
            ctx.ThrowExceptions = false;

            var templateErrorTag = new TestTag();
            templateErrorTag.FirstToken = new Token(TokenKind.TextData, "err1");
            templateErrorTag.LastToken = templateErrorTag.FirstToken;

            var runtimeErrorTag = new ErrorTestTag();
            runtimeErrorTag.FirstToken = new Token(TokenKind.TextData, "err2");
            runtimeErrorTag.LastToken = runtimeErrorTag.FirstToken;

            var collection = new TagCollection();
            collection.Add(templateErrorTag);
            collection.Add(runtimeErrorTag);

            using (var writer = new StringWriter())
            {
                ctx.Render(writer, collection);
                // 模板异常直接输出标签源码，运行时异常包装为解析异常后同样输出源码
                Assert.Equal("err1err2", writer.ToString());
            }

            Assert.Equal(2, ctx.AllErrors.Count);
            Assert.IsType<TemplateException>(ctx.AllErrors[0]);
            Assert.IsType<ParseException>(ctx.AllErrors[1]);
        }

        /// <summary>
        /// 上下文错误收集与抛出
        /// </summary>
        [Fact]
        public void TestThrowException()
        {
            var ctx = new EngineBuilder().Build().CreateContext();
            var tag = CreateTextTag("source");
            var exception = new TemplateException("error");

            ctx.ThrowExceptions = false;
            ctx.ThrowException(exception, tag, new StringWriter());
            Assert.Single(ctx.AllErrors);
            Assert.Same(exception, ctx.Error);

            // 禁用源码输出时不写内容
            ctx.ClearError();
            ctx.DisableCodeOutput = true;
            using (var writer = new StringWriter())
            {
                ctx.ThrowException(exception, tag, writer);
                Assert.Equal(string.Empty, writer.ToString());
            }

            // ThrowExceptions 为 true 时直接抛出
            ctx.ThrowExceptions = true;
            Assert.Throws<TemplateException>(() => ctx.ThrowException(exception, tag, new StringWriter()));
        }

        /// <summary>
        /// 异步渲染与异步异常处理
        /// </summary>
        [Fact]
        public async Task TestRenderAsync()
        {
            var ctx = new EngineBuilder().Build().CreateContext();
            var collection = new TagCollection();
            collection.Add(CreateTextTag("a"));
            collection.Add(CreateTextTag("b"));

            using (var writer = new StringWriter())
            {
                await ctx.RenderAsync(writer, collection, TestContext.Current.CancellationToken);
                Assert.Equal("ab", writer.ToString());
            }

            using (var writer = new StringWriter())
            {
                await ctx.RenderAsync(writer, null!, TestContext.Current.CancellationToken);
                await ctx.RenderAsync(writer, new TagCollection(), TestContext.Current.CancellationToken);
                Assert.Equal(string.Empty, writer.ToString());
            }

            await Assert.ThrowsAsync<ArgumentNullException>(() => ctx.RenderAsync(null!, collection, TestContext.Current.CancellationToken));

            // 异步异常处理
            ctx.ThrowExceptions = false;
            var tag = CreateTextTag("source");
            using (var writer = new StringWriter())
            {
                await ctx.ThrowExceptionAsync(new TemplateException("error"), tag, writer);
                Assert.Equal("source", writer.ToString());
            }
            Assert.Single(ctx.AllErrors);
        }

        /// <summary>
        /// 解释模板与缓存
        /// </summary>
        [Fact]
        public void TestInterpretTemplate()
        {
            var ctx = new EngineBuilder().UseInterpretationEngine().Build().CreateContext();

            var result = ctx.InterpretTemplate("hello ${name}");
            Assert.NotNull(result);

            // 同名模板会使用缓存
            var cached = ctx.InterpretTemplate("render-cache", new StringReader("cache"));
            var cached2 = ctx.InterpretTemplate("render-cache", new StringReader("cache"));
            Assert.Same(cached, cached2);

            Assert.Throws<ArgumentNullException>(() => ctx.InterpretTemplate(null!, new StringReader("x")));

            // 关闭缓存后每次都会重新解析
            ctx.EnableCache = false;
            Assert.NotSame(
                ctx.InterpretTemplate("render-cache2", new StringReader("cache")),
                ctx.InterpretTemplate("render-cache2", new StringReader("cache")));
        }

        /// <summary>
        /// 词法解析
        /// </summary>
        [Fact]
        public void TestLexer()
        {
            var ctx = new EngineBuilder().Build().CreateContext();
            Assert.Empty(ctx.Lexer(null!));
            Assert.Empty(ctx.Lexer(string.Empty));
            Assert.True(ctx.Lexer("hello ${name}").Count > 0);
            Assert.NotNull(ctx.CreateTemplateLexer("text"));
        }

        /// <summary>
        /// 编译模板与空模板
        /// </summary>
        [Fact]
        public async Task TestCompileTemplate()
        {
            var ctx = new EngineBuilder().UseCompileEngine().Build().CreateContext();
            ctx.Name = "compile-template";

            // reader 为 null 时返回空白模板
            var empty = ctx.CompileTemplate("empty", null!);
            Assert.IsType<EmptyCompileTemplate>(empty);

            using (var writer = new StringWriter())
            {
                empty.Render(writer, ctx);
                await empty.RenderAsync(writer, ctx, TestContext.Current.CancellationToken);
                Assert.Equal(string.Empty, writer.ToString());
            }

            Assert.Throws<ArgumentNullException>(() => ctx.CompileTemplate(null!, new StringReader("x")));

            var result = ctx.CompileTemplate("compile-template", new StringReader("hello"));
            Assert.NotNull(result);
            var cached = ctx.CompileTemplate("compile-template", new StringReader("hello"));
            Assert.Same(result, cached);
        }

        /// <summary>
        /// 空编译模板输出消息
        /// </summary>
        [Fact]
        public async Task TestEmptyCompileTemplate()
        {
            var ctx = new EngineBuilder().Build().CreateContext();
            ctx.ThrowExceptions = true;

            var template = new EmptyCompileTemplate(ctx, "message");
            Assert.True(template.IsCompileMode);

            using (var writer = new StringWriter())
            {
                template.Render(writer, ctx);
                Assert.Equal("message", writer.ToString());
            }

            using (var writer = new StringWriter())
            {
                await template.RenderAsync(writer, ctx, TestContext.Current.CancellationToken);
                Assert.Equal("message", writer.ToString());
            }

            // 未开启异常输出时不写内容
            ctx.ThrowExceptions = false;
            using (var writer = new StringWriter())
            {
                template.Render(writer, ctx);
                await template.RenderAsync(writer, ctx, TestContext.Current.CancellationToken);
                Assert.Equal(string.Empty, writer.ToString());
            }

            // 无消息与取消令牌
            var blank = new EmptyCompileTemplate(ctx);
            using (var writer = new StringWriter())
            {
                blank.Render(writer, ctx);
                await blank.RenderAsync(writer, ctx, TestContext.Current.CancellationToken);
                Assert.Equal(string.Empty, writer.ToString());
            }
        }

        /// <summary>
        /// 解释结果渲染
        /// </summary>
        [Fact]
        public async Task TestInterpretResult()
        {
            var engine = new EngineBuilder().UseInterpretationEngine().Build();
            var ctx = engine.CreateContext();

            var tags = new TagCollection();
            tags.Add(CreateTextTag("a"));

            var result = new InterpretResult(tags);
            Assert.Same(tags, result.Tags);

            using (var writer = new StringWriter())
            {
                result.Render(writer, ctx);
                Assert.Equal("a", writer.ToString());
            }

            using (var writer = new StringWriter())
            {
                await result.RenderAsync(writer, ctx, TestContext.Current.CancellationToken);
                Assert.Equal("a", writer.ToString());
            }
        }

        /// <summary>
        /// 渲染模板对象（TemplateRender）
        /// </summary>
        [Fact]
        public async Task TestTemplateRender()
        {
            var engine = new EngineBuilder().UseInterpretationEngine().Build();
            var ctx = engine.CreateContext();
            ctx.Name = "template-render";
            ctx.Set("name", "render", null!);

            var render = new TemplateRender(ctx, new StringReader("hello ${name}"));

            using (var writer = new StringWriter())
            {
                render.Render(writer);
                Assert.Equal("hello render", writer.ToString());
            }

            using (var writer = new StringWriter())
            {
                await render.RenderAsync(writer, TestContext.Current.CancellationToken);
                Assert.Equal("hello render", writer.ToString());
            } 
        }

        /// <summary>
        /// 编译结果基类
        /// </summary>
        [Fact]
        public async Task TestCompilerResult()
        {
            var ctx = new EngineBuilder().Build().CreateContext();
            var result = new TestCompilerResult();

            using (var writer = new StringWriter())
            {
                result.Render(writer, ctx);
                Assert.Equal("compiled", writer.ToString());
            }

            // writer 为 null 时使用内部 StringWriter
            result.Render(null!, ctx);
            Assert.Throws<ArgumentNullException>(() => result.Render(new StringWriter(), null!));

            using (var writer = new StringWriter())
            {
                await result.RenderAsync(writer, ctx, TestContext.Current.CancellationToken);
                Assert.Equal("compiledcompiled5", writer.ToString());
            }

            result.RenderAsync(null!, ctx, TestContext.Current.CancellationToken).GetAwaiter().GetResult();
            await Assert.ThrowsAsync<ArgumentNullException>(() => result.RenderAsync(new StringWriter(), null!, TestContext.Current.CancellationToken)); 
        }

        /// <summary>
        /// 编译期局部变量
        /// </summary>
        [Fact]
        public void TestLocalVar()
        {
            var method = new DynamicMethod("test", typeof(void), Type.EmptyTypes);
            var local = new LocalVar(method.GetILGenerator());

            local.Declare(typeof(int));
            Assert.Equal(1, local.Declare("name", typeof(string)));

            Assert.True(local.CanDeclare("name"));
            Assert.False(local.CanDeclare("other"));
            Assert.Equal(1, local["name"]);
            Assert.Throws<KeyNotFoundException>(() => local["other"]);
        }

        /// <summary>
        /// 上下文复制与克隆
        /// </summary>
        [Fact]
        public void TestContextCopyAndClone()
        {
            var engine = new EngineBuilder().UseCompileEngine().Build();
            var ctx = engine.CreateContext();
            ctx.Name = "copy";
            ctx.CurrentPath = "path";
            ctx.Set("a", 1, typeof(int));

            var copy = new Context(ctx);
            Assert.Equal("copy", copy.Name);
            Assert.Equal("path", copy.CurrentPath);
            Assert.Equal(ctx.TempData, copy.TempData);

            var clone = (TemplateContext)ctx.Clone();
            Assert.NotSame(ctx, clone);
            Assert.Equal(ctx.Name, clone.Name);

            Assert.Throws<ArgumentNullException>(() => TemplateContext.CreateContext(null!));
            var created = TemplateContext.CreateContext(ctx);
            Assert.NotSame(ctx, created);
            Assert.Equal("copy", created.Name);
            Assert.Equal(ctx.TempData, created.TempData!.Parent);
        }

        /// <summary>
        /// 资源目录与变量作用域
        /// </summary>
        [Fact]
        public void TestResourceDirectoriesAndScope()
        {
            var ctx = new EngineBuilder().Build().CreateContext();

            ctx.ResourceDirectories.Clear();
            Assert.Equal(Directory.GetCurrentDirectory(), Assert.Single(ctx.GetResourceDirectories()));

            ctx.ResourceDirectories.Add("a");
            Assert.Equal(new[] { "a" }, ctx.GetResourceDirectories());

            var scope = ctx.CreateVariableScope();
            Assert.Same(ctx.TempData, scope.Parent);
        }

        /// <summary>
        /// 输出格式化
        /// </summary>
        [Fact]
        public void TestOutputFormat()
        {
            var ctx = new EngineBuilder().Build().CreateContext();
            Assert.Null(ctx.OutputFormat(null!));
            Assert.Equal("5", ctx.OutputFormat(5));
            Assert.False(ctx.MustFormat(null!));
            Assert.False(ctx.MustFormat(5));
            Assert.False(ctx.MustFormat(typeof(int)));
        }

        /// <summary>
        /// 设置模板数据：编译模式下的匿名对象与非法类型
        /// </summary>
        [Fact]
        public void TestSetData()
        {
            var compileCtx = new EngineBuilder().UseCompileEngine().Build().CreateContext();
            var interpretCtx = new EngineBuilder().UseInterpretationEngine().Build().CreateContext();

            Assert.Throws<ArgumentNullException>(() => compileCtx.Set(null!, 1, typeof(int)));

            // 匿名对象
            compileCtx.Set("anonymous", new { Name = "x" }, null!);
            Assert.NotNull(compileCtx.TempData["anonymous"]);

            // 匿名对象数组
            var arr = new[] { new { Name = "a" }, new { Name = "b" } };
            compileCtx.Set("anonymousArray", arr, null!);
            Assert.NotNull(compileCtx.TempData["anonymousArray"]);

            // 不可见且非匿名类型会抛出异常
            Assert.Throws<ArgumentException>(() => compileCtx.Set("hidden", new HiddenModel(), typeof(HiddenModel)));

            // 解释模式：值与类型均可为空
            interpretCtx.Set("nullValue", null!, typeof(int));
            Assert.Null(interpretCtx.TempData["nullValue"]);
            interpretCtx.Set("value", 10, null!);
            Assert.Equal(10, interpretCtx.TempData["value"]);
        }

        /// <summary>
        /// 匿名对象设置
        /// </summary>
        [Fact]
        public void TestSetAnonymousObject()
        {
            var ctx = new EngineBuilder().UseCompileEngine().Build().CreateContext();

            Assert.Throws<ArgumentNullException>(() => ctx.SetAnonymousObject("a", null!));
            Assert.Throws<ArgumentNullException>(() => ctx.SetAnonymousObject(null!, new { a = 1 }));

            var value = new { Id = 1, Name = "n" };
            ctx.SetAnonymousObject("model", value);
            Assert.NotNull(ctx.TempData["model"]);
        }

        /// <summary>
        /// 模板扩展方法
        /// </summary>
        [Fact]
        public async Task TestTemplateExtensions()
        {
            var engine = new EngineBuilder().UseCompileEngine().Build();
            var template = engine.CreateTemplate("hello");

            Assert.Equal("hello", template.Render());
            Assert.Equal("hello", await template.RenderAsync());

            template.Set("name", "value", typeof(string));
            Assert.NotNull(template.Context.TempData["name"]);

            template.SetStaticType("static", typeof(ApiModel));
            Assert.Equal(typeof(ApiModel), template.Context.TempData.GetType("static"));

            template.SetStaticType(typeof(int));
            Assert.Equal(typeof(int), template.Context.TempData.GetType("Int32"));

            Assert.Throws<ArgumentNullException>(() => template.SetStaticType("x", null!));
            Assert.Throws<ArgumentNullException>(() => template.SetStaticType(null!));

            template.SetAnonymousObject("anonymous", new { a = 1 });
            Assert.NotNull(template.Context.TempData["anonymous"]);
        }

        /// <summary>
        /// 变量作用域
        /// </summary>
        [Fact]
        public void TestVariableScope()
        {
            var parent = new VariableScope();
            parent.Set("a", 1, typeof(int));

            var scope = new VariableScope(parent);
            Assert.Same(parent, scope.Parent);
            Assert.Equal(1, scope["a"]);
            Assert.Null(scope["none"]);
            Assert.True(scope.ContainsKey("a"));
            Assert.False(scope.ContainsKey("none"));
            Assert.Equal(1, scope.Count);
            Assert.Equal(typeof(int), scope.GetType("a"));

            // 更新父级作用域中的变量
            Assert.True(scope.Update("a", 2));
            Assert.Equal(2, parent["a"]);
            Assert.False(scope.Update("none", 1));

            scope.Set("b", "text", null!);
            Assert.Equal(typeof(string), scope.GetType("b"));
            Assert.Equal(2, scope.Count);
            Assert.Contains("b", scope.Keys);

            scope.Set("c", null!, typeof(int));
            Assert.Null(scope["c"]);

            // 清除
            scope.Clear();
            Assert.Equal(0, scope.Count - parent.Count);
            Assert.False(scope.Remove("a"));

            scope.Set("d", 4, typeof(int));
            scope.Clear(true);
            Assert.Null(parent["a"]);

            Assert.NotNull(scope.GetEnumerator());
            foreach (var kv in scope)
            {
                Assert.NotNull(kv.Key);
            }
        }

        /// <summary>
        /// 编译与渲染文件
        /// </summary>
        [Fact]
        public void TestCompileFile()
        {
            var dir = Path.Combine(Path.GetTempPath(), "jntemplate-file-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                var path = Path.Combine(dir, "file.txt");
                File.WriteAllText(path, "file content");

                var engine = new EngineBuilder().UseCompileEngine().Build();
                engine.AppendResourcePath(dir);
                var ctx = engine.CreateContext();

                Assert.Equal("file content", ctx.CompileAndRenderFile("file.txt"));
                Assert.NotNull(ctx.CompileFile("file.txt"));

                ctx.ResourceDirectories.Clear();
                ctx.CurrentPath = dir;
                Assert.NotNull(ctx.CompileFile("file.txt"));
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        /// <summary>
        /// 编译数据上下文
        /// </summary>
        [Fact]
        public void TestCompileWithAction()
        {
            var engine = new EngineBuilder().UseCompileEngine().Build();
            var ctx = engine.CreateContext();

            var result = ctx.Compile(null!, "hello", c => c.Name = "custom");
            Assert.NotNull(result);

            var emptyName = ctx.Compile(string.Empty, "hello");
            Assert.NotNull(emptyName);
        }

        /// <summary>
        /// 上下文对象加载
        /// </summary>
        [Fact]
        public async Task TestContextLoad()
        {
            var dir = Path.Combine(Path.GetTempPath(), "jntemplate-load-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                var path = Path.Combine(dir, "load.txt");
                File.WriteAllText(path, "load content");

                var ctx = new EngineBuilder().Build().CreateContext();
                ctx.ResourceDirectories.Clear();
                ctx.ResourceDirectories.Add(dir);

                Assert.Equal(path, ctx.FindFullPath("load.txt"));
                Assert.NotNull(ctx.Load("load.txt"));
                Assert.Null(ctx.Load("not-exists.txt"));
                Assert.NotNull(await ctx.LoadAsync("load.txt"));
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }
    }

    /// <summary>
    /// 测试用标签类型（未内置，可用于注册自定义访客）
    /// </summary>
    public class ErrorTestTag : Nodes.Tag, Nodes.ITag
    {
        /// <inheritdoc />
        public override bool IsSimple => true;
    }

    /// <summary>
    /// 测试用编译结果
    /// </summary>
    public class TestCompilerResult : CompilerResult
    {
        /// <inheritdoc />
        protected override void RenderResult(TextWriter writer, TemplateContext context)
        {
            writer.Write("compiled");
        }

        /// <inheritdoc />
        protected override List<object> GetRenderTask(TemplateContext context)
        {
            return new List<object>
            {
                Task.FromResult("compiled"),
                "compiled",
                5
            };
        }
    }
}
