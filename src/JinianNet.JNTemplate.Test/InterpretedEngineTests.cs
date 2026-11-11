namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 解释模式引擎测试（覆盖 <see cref="TemplateRender"/> 与 Dynamic 命名空间下的解释执行逻辑）
    /// </summary>
    public class InterpretedEngineTests
    {
        private static IEngine CreateEngine()
        {
            return new EngineBuilder().UseInterpretationEngine().Build();
        }

        private static string Render(string content, Action<ITemplate>? init = null)
        {
            var engine = CreateEngine();
            var template = engine.CreateTemplate(content);
            init?.Invoke(template);
            return template.Render();
        }

        /// <summary>
        /// 文本与变量
        /// </summary>
        [Fact]
        public void TestTextAndVariable()
        {
            Assert.Equal("hello world", Render("hello world"));
            Assert.Equal("hello jntemplate", Render("hello ${name}", t => t.Set("name", "jntemplate")));
            Assert.Equal("hello ", Render("hello ${missing}"));
            Assert.Equal("3", Render("${a}", t => t.Set("a", 3)));
        }

        /// <summary>
        /// 算术表达式
        /// </summary>
        [Fact]
        public void TestArithmetic()
        {
            Assert.Equal("18", Render("${8+2*5}"));
            Assert.Equal("2180", Render("${3000-1000--200-20}"));
            Assert.Equal("7", Render("${(1+2)*2+1}"));
            Assert.Equal("3", Render("${7%4}"));
            Assert.Equal("2", Render("${5/2}"));
            Assert.Equal("abc", Render("${a+b}", t =>
            {
                t.Set("a", "a");
                t.Set("b", "bc");
            }));
            Assert.Equal("-4", Render("${0-x}", t => t.Set("x", 4)));
        }

        /// <summary>
        /// 逻辑比较
        /// </summary>
        [Fact]
        public void TestLogic()
        {
            Assert.Equal("True", Render("${1>0}"));
            Assert.Equal("True", Render("${1==1 && 2==2}"));
            Assert.Equal("False", Render("${1==1 && 1==2}"));
            Assert.Equal("True", Render("${1==2 || 1==1}"));
            Assert.Equal("True", Render("${a!=b}", t =>
            {
                t.Set("a", 1);
                t.Set("b", 2);
            }));
        }

        /// <summary>
        /// if 语句
        /// </summary>
        [Fact]
        public void TestIf()
        {
            Assert.Equal("yes", Render("$if(1>0)yes$end"));
            Assert.Equal(" no", Render("$if(1>2)yes$else no$end"));
            Assert.Equal("b", Render("$if(a==1)a$elseif(a==2)b$else c$end", t => t.Set("a", 2)));
            Assert.Equal(" empty", Render("$if(str)full$else empty$end", t => t.Set("str", "")));
            Assert.Equal(string.Empty, Render("$if(1>2)yes$end"));
        }

        /// <summary>
        /// for 循环
        /// </summary>
        [Fact]
        public void TestFor()
        {
            Assert.Equal("012", Render("$for(i=0;i<3;i++)${i}$end"));
            Assert.Equal("0123", Render("$for(i=0;i<4;i++)${i}$end"));
            Assert.Equal("2", Render("$set(i=0)$for(;i<5;i++)$if(i==2)${i}$end$end"));
        }

        /// <summary>
        /// foreach 循环
        /// </summary>
        [Fact]
        public void TestForeach()
        {
            Assert.Equal("123", Render("$foreach(i in list)${i}$end", t => t.Set("list", new[] { 1, 2, 3 })));
            Assert.Equal("a-b-", Render("$foreach(s in list)${s}-$end", t => t.Set("list", new List<string> { "a", "b" })));

            var dict = new Dictionary<string, object> { { "a", 1 }, { "b", 2 } };
            Assert.Equal("a=1;b=2;", Render("$foreach(kv in dict)${kv.Key}=${kv.Value};$end", t => t.Set("dict", dict)));
        }

        /// <summary>
        /// set 与自定义变量
        /// </summary>
        [Fact]
        public void TestSet()
        {
            Assert.Equal("5", Render("$set(a=5)${a}"));
            Assert.Equal("10", Render("$set(a=5)$set(b=a*2)${b}"));
            Assert.Equal("ok", Render("$set(name=\"ok\")${name}"));
        }

        /// <summary>
        /// 对象成员访问与方法调用
        /// </summary>
        [Fact]
        public void TestMemberAccess()
        {
            var model = new ApiModel { Id = 3, Name = "model" };
            Assert.Equal("model", Render("${model.Name}", t => t.Set("model", model)));
            Assert.Equal("MODEL", Render("${model.Name.ToUpper()}", t => t.Set("model", model)));
            Assert.Equal("3", Render("${model.Id}", t => t.Set("model", model)));
            Assert.Equal("mode", Render("${model.Name.Substring(0,4)}", t => t.Set("model", model)));

            // 空引用不会抛出异常
            Assert.Equal(string.Empty, Render("${model.Name}", t => t.Set<object>("model", null!)));
        }

        /// <summary>
        /// 索引访问
        /// </summary>
        [Fact]
        public void TestIndex()
        {
            Assert.Equal("b", Render("${list[1]}", t => t.Set("list", new[] { "a", "b" })));
            Assert.Equal("2", Render("${dict[\"b\"]}", t => t.Set("dict", new Dictionary<string, int> { { "a", 1 }, { "b", 2 } })));
            Assert.Equal("a", Render("${dict[1]}", t => t.Set("dict", new Dictionary<int, string> { { 1, "a" } })));
        }

        /// <summary>
        /// 数组
        /// </summary>
        [Fact]
        public void TestArray()
        {
            Assert.Equal("3", Render("${arr.Length}", t => t.Set("arr", new[] { 1, 2, 3 })));
        }

        /// <summary>
        /// 注释与转义
        /// </summary>
        [Fact]
        public void TestComment()
        {
            Assert.Equal("ab", Render("a$* comment *$b"));
            Assert.Equal("a$", Render("a$$b"));
        }

        /// <summary>
        /// 异步渲染
        /// </summary>
        [Fact]
        public async Task TestRenderAsync()
        {
            var engine = CreateEngine();
            var template = engine.CreateTemplate("hello ${name}");
            template.Set("name", "async");

            using (var writer = new StringWriter())
            {
                await template.RenderAsync(writer, TestContext.Current.CancellationToken);
                Assert.Equal("hello async", writer.ToString());
            }

            Assert.Equal("hello async", await template.RenderAsync());
        }

        /// <summary>
        /// 自定义标签在解释模式下解析
        /// </summary>
        [Fact]
        public void TestCustomTag()
        {
            var engine = CreateEngine();
            var parser = new TestParser();
            var parseVisitor = new Parsers.DynamicVisitor(typeof(TestTag), parser.Parse, null!, null!);
            engine.Register(parseVisitor);

            // 自定义标签没有执行逻辑时不输出内容
            var template = engine.CreateTemplate("a${:b}");
            Assert.Equal("a", template.Render());

            var ctx = engine.CreateContext();
            var tag = new TestTag();
            Assert.Null(parseVisitor.Execute(tag, ctx));
            Assert.Null(parseVisitor.Compile(tag, null!));
            Assert.Null(parseVisitor.GuessType(tag, null!));

            // 仅提供执行方法的访客
            var excuteVisitor = new Parsers.DynamicVisitor(typeof(TestTag), (t, c) => "custom");
            Assert.Equal("custom", excuteVisitor.Execute(tag, ctx));
            Assert.Null(excuteVisitor.Parse(null!, new Nodes.TokenCollection()));
            Assert.Equal("TestTag", excuteVisitor.Name);
        }
    }
}
