using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Parsers;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 异常对象测试
    /// </summary>
    public class ExceptionTests
    {
        private static TextTag CreateTag(string text)
        {
            var tag = new TextTag();
            tag.FirstToken = new Token(TokenKind.TextData, text) { BeginLine = 3, BeginColumn = 7 };
            return tag;
        }

        /// <summary>
        /// 无参构造
        /// </summary>
        [Fact]
        public void TestTemplateExceptionDefault()
        {
            var e = new TemplateException();
            Assert.NotNull(e.Message);
            Assert.Equal(0, e.Line);
            Assert.Equal(0, e.Column);
            Assert.Null(e.Code);
        }

        /// <summary>
        /// 带行号列号的构造方法会拼接位置信息
        /// </summary>
        [Fact]
        public void TestTemplateExceptionWithPosition()
        {
            var e = new TemplateException("syntax error", 2, 5);
            Assert.Equal("syntax error [line:2,col:5]", e.Message);
            Assert.Equal(2, e.Line);
            Assert.Equal(5, e.Column);
        }

        /// <summary>
        /// 基于标签的构造方法会记录标签位置
        /// </summary>
        [Fact]
        public void TestTemplateExceptionWithTag()
        {
            var tag = CreateTag("hello");
            var inner = new InvalidOperationException("boom");
            var e = new TemplateException(tag, "error", inner);

            Assert.Same(inner, e.InnerException);
            Assert.Equal(3, e.Line);
            Assert.Equal(7, e.Column);
            Assert.Contains("error on hello", e.Message);
            Assert.Contains("[line:3,col:7]", e.Message);

            // 无标签时不拼接 on ...
            var e2 = new TemplateException(null!, "error", inner);
            Assert.Equal("error", e2.Message);
            Assert.Equal(0, e2.Line);

            // 无标签 token 时只设置消息
            var e3 = new TemplateException(new TestTag(), "error", inner);
            Assert.Equal(0, e3.Line);
        }

        /// <summary>
        /// ParseException 构造方法
        /// </summary>
        [Fact]
        public void TestParseException()
        {
            Assert.NotNull(new ParseException().Message);
            Assert.Equal("msg", new ParseException("msg").Message);

            var positioned = new ParseException("msg", 1, 2);
            Assert.Equal(1, positioned.Line);
            Assert.Equal(2, positioned.Column);

            var tag = CreateTag("abc");
            var inner = new Exception("inner");
            var e = new ParseException(tag, inner);
            Assert.Equal("inner on abc [line:3,col:7]", e.Message);
            Assert.Same(inner, e.InnerException);
        }

        /// <summary>
        /// RuntimeException 构造方法
        /// </summary>
        [Fact]
        public void TestRuntimeException()
        {
            var inner = new Exception("inner");
            var tag = CreateTag("abc");
            var e = new RuntimeException(tag, "msg", inner);
            Assert.Same(inner, e.InnerException);
            Assert.Equal(3, e.Line);
            Assert.Contains("msg on abc", e.Message);

            // 消息构造方法的 InnerException 不会被传递（保持原有行为）
            var e2 = new RuntimeException("msg", inner);
            Assert.Equal("msg", e2.Message);
            Assert.Null(e2.InnerException);
        }

        /// <summary>
        /// 编译器异常：根据上下文决定是否抛出
        /// </summary>
        [Fact]
        public void TestCompilerResultThrowException()
        {
            var engine = new EngineBuilder().Build();
            var ctx = engine.CreateContext();
            ctx.ThrowExceptions = false;

            CompilerResult.ThrowException(ctx, new Exception("inner"), 4, 8, "$x");
            Assert.Single(ctx.AllErrors);
            var first = Assert.IsType<RuntimeException>(ctx.AllErrors[0]);
            Assert.Equal(4, first.Line);
            Assert.Equal(8, first.Column);
            Assert.Contains("inner on $x [line:4,col:8]", first.Message);

            // 已包含位置信息的异常直接使用
            ctx.ClearError();
            var templateException = new TemplateException("known", 1, 1);
            CompilerResult.ThrowException(ctx, templateException, 4, 8, "$x");
            Assert.Same(templateException, ctx.AllErrors[0]);
        }

        /// <summary>
        /// 上下文异常集合
        /// </summary>
        [Fact]
        public void TestTemplateContextErrors()
        {
            var engine = new EngineBuilder().Build();
            var ctx = engine.CreateContext();
            ctx.ThrowExceptions = false;

            Assert.Null(ctx.Error);
            ctx.AddError(new Exception("a"));
            ctx.AddError(new Exception("b"));
            Assert.Equal(2, ctx.AllErrors.Count);
            Assert.Equal("a", ctx.Error!.Message);

            ctx.ClearError();
            Assert.Empty(ctx.AllErrors);

            ctx.ThrowExceptions = true;
            Assert.Throws<Exception>(() => ctx.AddError(new Exception("throw")));
        }
    }

    /// <summary>
    /// 标签解析器（<see cref="Resolver"/>）测试
    /// </summary>
    public class ResolverTests
    {
        private static Resolver CreateResolver(params ITagVisitor[] visitors)
        {
            var resolver = new Resolver();
            foreach (var visitor in visitors)
            {
                resolver.Register(visitor);
            }
            return resolver;
        }

        /// <summary>
        /// 注册、索引与枚举
        /// </summary>
        [Fact]
        public void TestRegisterAndEnumerate()
        {
            var textVisitor = new TextVisitor();
            var nullVisitor = new NullVisitor();
            var resolver = CreateResolver(textVisitor, nullVisitor);

            Assert.Equal(2, resolver.Count);
            Assert.False(resolver.IsReadOnly);
            Assert.Contains(textVisitor, resolver);
            Assert.DoesNotContain(new NumberVisitor(), resolver);

            Assert.Equal(0, resolver.IndexOf(textVisitor));
            Assert.Equal(1, resolver.IndexOf(nullVisitor));
            Assert.Equal(-1, resolver.IndexOf(new NumberVisitor()));

            Assert.Same(textVisitor, resolver[0]);
            Assert.Same(nullVisitor, resolver[1]);
            //Assert.Null(resolver[5]);
            Assert.Same(textVisitor, resolver["TextTag"]!.Visitor);
            Assert.Null(resolver["not-exists"]);
            Assert.Null(resolver[string.Empty]);

            Assert.Equal(new ITagVisitor[] { textVisitor, nullVisitor }, resolver.ToArray());

            var array = new ITagVisitor[2];
            resolver.CopyTo(array, 0);
            Assert.Same(textVisitor, array[0]);
            Assert.Same(nullVisitor, array[1]);
        }

        /// <summary>
        /// 移除访客后索引会重排
        /// </summary>
        [Fact]
        public void TestRemoveAndIndexReset()
        {
            var textVisitor = new TextVisitor();
            var nullVisitor = new NullVisitor();
            var numberVisitor = new NumberVisitor();
            var resolver = CreateResolver(textVisitor, nullVisitor, numberVisitor);

            Assert.True(resolver.Remove(textVisitor));
            Assert.False(resolver.Remove(textVisitor));
            Assert.Equal(2, resolver.Count);
            Assert.Equal(0, resolver.IndexOf(nullVisitor));
            Assert.Equal(1, resolver.IndexOf(numberVisitor));

            resolver.RemoveAt(0);
            Assert.Single(resolver);
            Assert.Equal(0, resolver.IndexOf(numberVisitor));

            resolver.RemoveAt(100);
            Assert.Single(resolver);

            resolver.Clear();
            Assert.Empty(resolver);
            Assert.Empty(resolver);
        }

        /// <summary>
        /// Insert 与重复名称检测
        /// </summary>
        [Fact]
        public void TestInsert()
        {
            var resolver = new Resolver();
            // 空集合时直接注册
            resolver.Insert(0, new TextVisitor());
            Assert.Single(resolver);

            var nullVisitor = new NullVisitor();
            resolver.Insert(0, nullVisitor);
            Assert.Equal(0, resolver.IndexOf(nullVisitor));
            Assert.Equal(1, resolver.IndexOf(new TextVisitor()));

            var numberVisitor = new NumberVisitor();
            resolver.Insert(1, numberVisitor);
            Assert.Equal(1, resolver.IndexOf(numberVisitor));
            Assert.Equal(2, resolver.IndexOf(new TextVisitor()));

            // 重复名称
            Assert.Throws<ArgumentException>(() => resolver.Insert(0, new TextVisitor()));
            // 索引超出范围时退回注册
            var stringVisitor = new StringVisitor();
            resolver.Insert(100, stringVisitor);
            Assert.Equal(4, resolver.Count);

            // 索引器赋值
            var commentVisitor = new CommentVisitor();
            resolver[0] = commentVisitor;
            Assert.Same(commentVisitor, resolver[0]);
            Assert.Null(resolver["NullTag"]);
            Assert.Throws<ArgumentOutOfRangeException>(() => resolver[100] = new BooleanVisitor());
        }

        /// <summary>
        /// 解析：无匹配时返回 null
        /// </summary>
        [Fact]
        public void TestParsing()
        {
            var engine = new EngineBuilder().Build();
            var parser = engine.CreateContext().CreateTemplateParser(Array.Empty<Token>());
            var resolver = CreateResolver(new TextVisitor());

            Assert.Null(resolver.Parsing(parser, null!));
            Assert.Null(resolver.Parsing(parser, new TokenCollection()));
            Assert.Null(resolver.Parsing(null!, new TokenCollection()));
            Assert.Null(resolver.Parsing(parser, new TokenCollection(new[] { new Token(TokenKind.TextData, "x") })));

            var empty = new Resolver();
            Assert.Null(empty.Parsing(parser, new TokenCollection(new[] { new Token(TokenKind.TextData, "x") })));
        }

        /// <summary>
        /// 执行与编译：未注册的标签抛出异常
        /// </summary>
        [Fact]
        public void TestExcuteAndCompile()
        {
            var engine = new EngineBuilder().Build();
            var ctx = engine.CreateContext();
            var resolver = CreateResolver(new TextVisitor());

            Assert.Null(resolver.Execute(null!, ctx));

            var textTag = new TextTag();
            textTag.FirstToken = new Token(TokenKind.TextData, "hello");
            Assert.Equal("hello", resolver.Execute(textTag, ctx));
            Assert.Equal("hello", resolver.Execute(nameof(TextTag), textTag, ctx));

            Assert.Throws<ParseException>(() => resolver.Execute("Unknown", textTag, ctx));
            Assert.Throws<CompileException>(() => resolver.Compile("Unknown", textTag, null!));
            Assert.Equal(typeof(string), resolver.GuessType(textTag, null!));
            Assert.Equal(typeof(string), resolver.GetType(textTag, null!));
            Assert.Throws<CompileException>(() => resolver.GuessType("Unknown", textTag, null!));
        }
    }
}
