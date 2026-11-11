using JinianNet.JNTemplate.Test.Model;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// <see cref="JinianNet.JNTemplate.TemplateExtensions"/> 与
    /// <see cref="JinianNet.JNTemplate.TemplateContextExtensions"/> 测试
    /// </summary>
    public class TemplateExtensionTests
    {
        private static IEngine CreateEngine()
        {
            return new EngineBuilder().Build();
        }

        private static IEngine CreateInterpretedEngine()
        {
            return new EngineBuilder().UseInterpretationEngine().Build();
        }

        /// <summary>
        /// 同步渲染扩展方法
        /// </summary>
        [Fact]
        public void TestRender()
        {
            var template = CreateInterpretedEngine().CreateTemplate("hello ${name}");
            template.Set("name", "world");
            Assert.Equal("hello world", JinianNet.JNTemplate.TemplateExtensions.Render(template));
        }

        /// <summary>
        /// 异步渲染扩展方法
        /// </summary>
        [Fact]
        public async Task TestRenderAsync()
        {
            var template = CreateInterpretedEngine().CreateTemplate("hello ${name}");
            template.Set("name", "async");
            Assert.Equal("hello async", await JinianNet.JNTemplate.TemplateExtensions.RenderAsync(template));
        }

        /// <summary>
        /// 设置变量：泛型与非泛型重载
        /// </summary>
        [Fact]
        public void TestSet()
        {
            var template = CreateEngine().CreateTemplate("${name}");
            JinianNet.JNTemplate.TemplateExtensions.Set(template, "name", "a", typeof(string));
            Assert.Equal("a", template.Render());

            JinianNet.JNTemplate.TemplateExtensions.Set<string>(template, "name", "b");
            Assert.Equal("b", template.Render());

            // 解释模式：类型由运行时推断
            var interpreted = CreateInterpretedEngine().CreateTemplate("${value}");
            JinianNet.JNTemplate.TemplateExtensions.Set(interpreted, "value", 3, null);
            Assert.Equal("3", interpreted.Render());
        }

        /// <summary>
        /// 设置静态类型：默认使用类型名作为键
        /// </summary>
        [Fact]
        public void TestSetStaticType()
        {
            var template = CreateEngine().CreateTemplate("${Entity}");
            JinianNet.JNTemplate.TemplateExtensions.SetStaticType(template, typeof(Entity));
            Assert.Equal(typeof(Entity), template.Context.TempData.GetType(nameof(Entity)));

            JinianNet.JNTemplate.TemplateExtensions.SetStaticType(template, "Model", typeof(Entity));
            Assert.Equal(typeof(Entity), template.Context.TempData.GetType("Model"));

            // 键为空时回退到类型名
            JinianNet.JNTemplate.TemplateExtensions.SetStaticType(template, string.Empty, typeof(Entity));
            Assert.Equal(typeof(Entity), template.Context.TempData.GetType(nameof(Entity)));

            Assert.Throws<ArgumentNullException>(() => JinianNet.JNTemplate.TemplateExtensions.SetStaticType(template, (Type)null));
            Assert.Throws<ArgumentNullException>(() => JinianNet.JNTemplate.TemplateExtensions.SetStaticType(template, "x", (Type)null));
        }

        /// <summary>
        /// 设置匿名对象：编译模式会生成访问类型
        /// </summary>
        [Fact]
        public void TestSetAnonymousObjectInCompileMode()
        {
            var template = CreateEngine().CreateTemplate("${model.Name}");
            Assert.True(template.IsCompileMode);
            template.SetAnonymousObject("model", new { Name = "compiled" });
            Assert.Equal("compiled", template.Render());
        }

        /// <summary>
        /// 设置匿名对象：解释模式直接写入变量作用域
        /// </summary>
        [Fact]
        public void TestSetAnonymousObjectInInterpretedMode()
        {
            var template = CreateInterpretedEngine().CreateTemplate("${model.Name}");
            Assert.False(template.IsCompileMode);
            template.SetAnonymousObject("model", new { Name = "interpreted" });
            Assert.Equal("interpreted", template.Render());

            // 解释模式下键不允许为空
            Assert.Throws<ArgumentNullException>(() => template.SetAnonymousObject(null, new { Name = "x" }));
        }

        /// <summary>
        /// 上下文设置：变量作用域为空时直接返回
        /// </summary>
        [Fact]
        public void TestContextSet()
        {
            var ctx = CreateInterpretedEngine().CreateContext();
            ctx.TempData = null;
            ctx.Set("key", 1, typeof(int));
            Assert.Null(ctx.TempData);

            Assert.Throws<ArgumentNullException>(() => ctx.Set(null, 1, typeof(int)));
        }

        /// <summary>
        /// 上下文设置匿名对象：空值与数组元素为 null 的场景
        /// </summary>
        [Fact]
        public void TestContextSetAnonymousObject()
        {
            var ctx = CreateInterpretedEngine().CreateContext();

            // 空值直接返回
            ctx.SetAnonymousObject("none", null, typeof(object));
            Assert.False(ctx.TempData.ContainsKey("none"));

            // 匿名对象数组，其中一个元素为 null
            var list = new[] { new { Id = 1 }, null };
            ctx.SetAnonymousObject("list", list, list.GetType());
            Assert.True(ctx.TempData.ContainsKey("list"));
            var array = Assert.IsAssignableFrom<Array>(ctx.TempData["list"]);
            Assert.Equal(2, array.Length);
            Assert.Null(array.GetValue(1));

            Assert.Throws<ArgumentNullException>(() => ctx.SetAnonymousObject(null, new { Id = 1 }));
        }

        /// <summary>
        /// 格式化判断与输出格式化
        /// </summary>
        [Fact]
        public void TestMustFormatAndOutputFormat()
        {
            var ctx = CreateInterpretedEngine().CreateContext();
            Assert.False(ctx.MustFormat((object)null));
            Assert.False(ctx.MustFormat(typeof(int)));
            Assert.Null(ctx.OutputFormat(null));
            Assert.Equal("1", ctx.OutputFormat(1));
        }

        /// <summary>
        /// 兼容性属性 DisableeLogogram 与 DisableLogogram 同步
        /// </summary>
        [Fact]
        public void TestObsoleteDisableLogogram()
        {
            var ctx = CreateInterpretedEngine().CreateContext();
#pragma warning disable CS0618
            ctx.DisableeLogogram = true;
            Assert.True(ctx.DisableLogogram);
            Assert.True(ctx.DisableeLogogram);
#pragma warning restore CS0618
        }
    }
}
