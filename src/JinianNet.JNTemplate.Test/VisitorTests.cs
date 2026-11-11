using System.Reflection;
using System.Reflection.Emit;
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Parsers;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 标签解析器（<see cref="ITagVisitor"/>）的编译/执行逻辑测试。
    /// 大部分场景在模板渲染时已被间接覆盖，本类用于直接覆盖各访客的 Compile/GuessType 分支。
    /// </summary>
    public class VisitorTests
    {
        /// <summary>
        /// 创建带有动态程序集的编译上下文
        /// </summary>
        private static CompileContext CreateCompileContext()
        {
            var name = "JntVisitorTest_" + Guid.NewGuid().ToString("N");
            var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(name), AssemblyBuilderAccess.RunAndCollect);
            var module = assembly.DefineDynamicModule(name);
            var ctx = new CompileContext(new EngineBuilder().Build().CreateContext())
            {
                TypeBuilder = module.DefineType("VisitorTarget", TypeAttributes.Public)
            };
            return ctx;
        }

        private static TemplateContext CreateContext()
        {
            return new EngineBuilder().Build().CreateContext();
        }

        private static TokenCollection Tokens(params Token[] tokens)
        {
            return new TokenCollection(tokens);
        }

        /// <summary>
        /// 构建算术标签
        /// </summary>
        private static ArithmeticTag Arithmetic(ITag left, Operator opt, ITag right)
        {
            var tag = new ArithmeticTag();
            tag.AddChild(left);
            tag.AddChild(new OperatorTag { Value = opt });
            tag.AddChild(right);
            return tag;
        }

        /// <summary>
        /// 编译标签并实际执行，用于验证生成的 IL 合法
        /// </summary>
        private static object CompileAndInvoke(ITag tag, TemplateContext templateContext, Type expectedType)
        {
            var name = "JntArithmeticTest_" + Guid.NewGuid().ToString("N");
            var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(name), AssemblyBuilderAccess.RunAndCollect);
            var module = assembly.DefineDynamicModule(name);
            var typeBuilder = module.DefineType("ArithmeticTarget", TypeAttributes.Public);
            var ctx = new CompileContext(templateContext)
            {
                TypeBuilder = typeBuilder
            };

            Assert.Equal(expectedType, ctx.GuessType(tag));
            var mb = ctx.CompileTag(tag);
            Assert.NotNull(mb);

            var type = typeBuilder.CreateType();
            var instance = Activator.CreateInstance(type);
            return type.GetMethod(mb!.Name)!.Invoke(instance, new object[] { templateContext });
        }

        /// <summary>
        /// 文本标签访客
        /// </summary>
        [Fact]
        public void TestTextVisitor()
        {
            var visitor = new TextVisitor();
            var tag = new TextTag { FirstToken = new Token(TokenKind.TextData, "hello") };
            var ctx = CreateCompileContext();

            Assert.Null(visitor.Parse(null, Tokens()));
            Assert.Equal("TextTag", visitor.Name);
            Assert.Equal(typeof(string), visitor.GuessType(tag, ctx));
            Assert.Equal("hello", visitor.Execute(tag, CreateContext()));
            Assert.Null(visitor.Execute(new NullTag(), CreateContext()));

            var method = visitor.Compile(tag, ctx);
            Assert.NotNull(method);
            Assert.Equal(typeof(string), method.ReturnType);

            // 非文本标签编译时输出空字符串
            var empty = visitor.Compile(new NullTag(), CreateCompileContext());
            Assert.NotNull(empty);
        }

        /// <summary>
        /// 操作符标签访客
        /// </summary>
        [Fact]
        public void TestOperatorVisitor()
        {
            var visitor = new OperatorVisitor();
            var tag = new OperatorTag { Value = Operator.Add };
            var ctx = CreateCompileContext();

            Assert.Null(visitor.Parse(null, Tokens()));
            Assert.Equal(typeof(string), visitor.GuessType(tag, ctx));
            Assert.Equal(Operator.Add, visitor.Execute(tag, CreateContext()));
            Assert.Null(visitor.Execute(new NullTag(), CreateContext()));
            Assert.Throws<ArgumentNullException>(() => visitor.Compile(new NullTag(), ctx));

            var method = visitor.Compile(tag, CreateCompileContext());
            Assert.NotNull(method);
        }

        /// <summary>
        /// 注释标签访客
        /// </summary>
        [Fact]
        public void TestCommentVisitor()
        {
            var visitor = new CommentVisitor();
            var ctx = CreateCompileContext();

            var tag = visitor.Parse(null, Tokens(new Token(TokenKind.Comment, "* c *")));
            var comment = Assert.IsType<CommentTag>(tag);
            Assert.NotNull(comment.FirstToken);

            Assert.Null(visitor.Parse(null, Tokens(new Token(TokenKind.TextData, "c"), new Token(TokenKind.TextData, "d"))));
            Assert.Null(visitor.Execute(comment, CreateContext()));
            Assert.Equal(typeof(void), visitor.GuessType(comment, ctx));
            Assert.NotNull(visitor.Compile(comment, ctx));
        }

        /// <summary>
        /// $end 标签访客
        /// </summary>
        [Fact]
        public void TestEndVisitor()
        {
            var visitor = new EndVisitor();
            var ctx = CreateCompileContext();

            Assert.IsType<EndTag>(visitor.Parse(null, Tokens(new Token(TokenKind.TextData, "end"))));
            Assert.IsType<EndTag>(visitor.Parse(null, Tokens(new Token(TokenKind.TextData, "END"))));
            Assert.Null(visitor.Parse(null, Tokens(new Token(TokenKind.TextData, "other"))));
            Assert.Null(visitor.Parse(null, Tokens()));

            var tag = new EndTag();
            Assert.Equal(typeof(void), visitor.GuessType(tag, ctx));
            Assert.Null(visitor.Execute(tag, CreateContext()));
            Assert.NotNull(visitor.Compile(tag, ctx));
        }

        /// <summary>
        /// 数字标签访客：字面量类型推断（int/long/double）
        /// </summary>
        [Fact]
        public void TestNumberVisitor()
        {
            var visitor = new NumberVisitor();
            var ctx = CreateCompileContext();

            var intTag = Assert.IsType<NumberTag>(visitor.Parse(null, Tokens(new Token(TokenKind.Number, "123"))));
            Assert.Equal(123, intTag.Value);

            // 9 位数字仍在 int 范围内
            var nineTag = Assert.IsType<NumberTag>(visitor.Parse(null, Tokens(new Token(TokenKind.Number, "999999999"))));
            Assert.Equal(999999999, nineTag.Value);

            // 超过 9 位按 long 处理
            var longTag = Assert.IsType<NumberTag>(visitor.Parse(null, Tokens(new Token(TokenKind.Number, "9999999999"))));
            Assert.Equal(9999999999L, longTag.Value);

            // 带小数点按 double 处理
            var doubleTag = Assert.IsType<NumberTag>(visitor.Parse(null, Tokens(new Token(TokenKind.Number, "1.5"))));
            Assert.Equal(1.5d, doubleTag.Value);

            Assert.Null(visitor.Parse(null, Tokens(new Token(TokenKind.TextData, "123"))));
            Assert.Null(visitor.Parse(null, Tokens(new Token(TokenKind.Number, "1"), new Token(TokenKind.Number, "2"))));

            Assert.Equal(typeof(int), visitor.GuessType(intTag, ctx));
            Assert.Equal(123, visitor.Execute(intTag, CreateContext()));
            Assert.Null(visitor.Execute(new TextTag(), CreateContext()));
            Assert.Throws<ArgumentNullException>(() => visitor.Compile(new TextTag(), ctx));

            Assert.NotNull(visitor.Compile(intTag, CreateCompileContext()));
            Assert.NotNull(visitor.Compile(longTag, CreateCompileContext()));
            Assert.NotNull(visitor.Compile(doubleTag, CreateCompileContext()));
        }

        /// <summary>
        /// 布尔标签访客
        /// </summary>
        [Fact]
        public void TestBooleanVisitor()
        {
            var visitor = new BooleanVisitor();
            var tag = new BooleanTag { Value = true };

            Assert.True((bool)visitor.Execute(tag, CreateContext()));
            Assert.Null(visitor.Execute(new TextTag(), CreateContext()));
            Assert.Equal(typeof(bool), visitor.GuessType(tag, CreateCompileContext()));
            Assert.NotNull(visitor.Compile(tag, CreateCompileContext()));
        }

        /// <summary>
        /// null 标签访客
        /// </summary>
        [Fact]
        public void TestNullVisitor()
        {
            var visitor = new NullVisitor();
            var tag = new NullTag();

            Assert.IsType<NullTag>(visitor.Parse(null, Tokens(new Token(TokenKind.TextData, "null"))));
            Assert.Null(visitor.Parse(null, Tokens(new Token(TokenKind.TextData, "nil"))));
            Assert.Null(visitor.Execute(tag, CreateContext()));
            Assert.Equal(typeof(object), visitor.GuessType(tag, CreateCompileContext()));
            Assert.NotNull(visitor.Compile(tag, CreateCompileContext()));
        }

        /// <summary>
        /// body 标签访客
        /// </summary>
        [Fact]
        public void TestBodyVisitor()
        {
            var visitor = new BodyVisitor();
            var tag = new BodyTag();

            Assert.IsType<BodyTag>(visitor.Parse(null, Tokens(new Token(TokenKind.TextData, "body"))));
            Assert.Null(visitor.Parse(null, Tokens(new Token(TokenKind.TextData, "other"))));
            Assert.Equal(typeof(string), visitor.GuessType(tag, CreateCompileContext()));
            Assert.Null(visitor.Execute(new TextTag(), CreateContext()));
        }

        /// <summary>
        /// 引用标签访客
        /// </summary>
        [Fact]
        public void TestReferenceVisitor()
        {
            var visitor = new ReferenceVisitor();
            var ctx = CreateContext();

            var tag = new ReferenceTag();
            tag.AddChild(new TextTag { FirstToken = new Token(TokenKind.TextData, "child") });

            Assert.Equal("child", visitor.Execute(tag, ctx));
            Assert.Null(visitor.Execute(new TextTag(), ctx));
            Assert.Null(visitor.Execute(new ReferenceTag(), ctx));

            var compileContext = CreateCompileContext();
            Assert.Equal(typeof(string), visitor.GuessType(tag, compileContext));
            Assert.NotNull(visitor.Compile(tag, compileContext));
        }

        /// <summary>
        /// 字符串标签访客
        /// </summary>
        [Fact]
        public void TestStringVisitor()
        {
            var visitor = new StringVisitor();
            var tag = new StringTag { Value = "text" };

            Assert.Equal("text", visitor.Execute(tag, CreateContext()));
            Assert.Null(visitor.Execute(new TextTag(), CreateContext()));
            Assert.Equal(typeof(string), visitor.GuessType(tag, CreateCompileContext()));
            Assert.NotNull(visitor.Compile(tag, CreateCompileContext()));
        }

        /// <summary>
        /// 算术标签编译：decimal 运算需调用 decimal 运算符重载，生成的 IL 必须可执行
        /// </summary>
        [Fact]
        public void TestArithmeticCompileWithDecimal()
        {
            var templateContext = CreateContext();
            templateContext.TempData.Set("a", 7.5m, typeof(decimal));
            templateContext.TempData.Set("b", 2m, typeof(decimal));

            Assert.Equal(9.5m, CompileAndInvoke(Arithmetic(new VariableTag { Name = "a" }, Operator.Add, new VariableTag { Name = "b" }), templateContext, typeof(decimal)));
            Assert.Equal(5.5m, CompileAndInvoke(Arithmetic(new VariableTag { Name = "a" }, Operator.Subtract, new VariableTag { Name = "b" }), templateContext, typeof(decimal)));
            Assert.Equal(15m, CompileAndInvoke(Arithmetic(new VariableTag { Name = "a" }, Operator.Multiply, new VariableTag { Name = "b" }), templateContext, typeof(decimal)));
            Assert.Equal(3.75m, CompileAndInvoke(Arithmetic(new VariableTag { Name = "a" }, Operator.Divided, new VariableTag { Name = "b" }), templateContext, typeof(decimal)));
            Assert.Equal(1.5m, CompileAndInvoke(Arithmetic(new VariableTag { Name = "a" }, Operator.Remainder, new VariableTag { Name = "b" }), templateContext, typeof(decimal)));
        }

        /// <summary>
        /// 算术标签编译：比较运算的结果类型为 bool
        /// </summary>
        [Fact]
        public void TestArithmeticCompileWithComparison()
        {
            var templateContext = CreateContext();
            templateContext.TempData.Set("a", 7.5m, typeof(decimal));
            templateContext.TempData.Set("b", 2m, typeof(decimal));

            Assert.Equal(true, CompileAndInvoke(Arithmetic(new VariableTag { Name = "a" }, Operator.GreaterThan, new VariableTag { Name = "b" }), templateContext, typeof(bool)));
            Assert.Equal(true, CompileAndInvoke(Arithmetic(new VariableTag { Name = "a" }, Operator.GreaterThanOrEqual, new VariableTag { Name = "b" }), templateContext, typeof(bool)));
            Assert.Equal(false, CompileAndInvoke(Arithmetic(new VariableTag { Name = "a" }, Operator.LessThan, new VariableTag { Name = "b" }), templateContext, typeof(bool)));
            Assert.Equal(false, CompileAndInvoke(Arithmetic(new VariableTag { Name = "a" }, Operator.LessThanOrEqual, new VariableTag { Name = "b" }), templateContext, typeof(bool)));
            Assert.Equal(false, CompileAndInvoke(Arithmetic(new VariableTag { Name = "a" }, Operator.Equal, new VariableTag { Name = "b" }), templateContext, typeof(bool)));
            Assert.Equal(true, CompileAndInvoke(Arithmetic(new VariableTag { Name = "a" }, Operator.NotEqual, new VariableTag { Name = "b" }), templateContext, typeof(bool)));

            // 整型比较
            Assert.Equal(true, CompileAndInvoke(Arithmetic(new NumberTag { Value = 2 }, Operator.GreaterThan, new NumberTag { Value = 1 }), templateContext, typeof(bool)));
        }
    }
}
