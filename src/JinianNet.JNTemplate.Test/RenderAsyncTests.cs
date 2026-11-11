using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Parsers;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 异步结果（<see cref="Task"/>）渲染与渲染异常处理测试
    /// </summary>
    public class RenderAsyncTests
    {
        private static IEngine CreateInterpretedEngine()
        {
            return new EngineBuilder().UseInterpretationEngine().Build();
        }

        /// <summary>
        /// 同步渲染：标签结果为 Task&lt;T&gt; 时输出其 Result
        /// </summary>
        [Fact]
        public void TestRenderTaskResult()
        {
            var template = CreateInterpretedEngine().CreateTemplate("${task}");
            template.Set("task", Task.FromResult("async-value"));
            Assert.Equal("async-value", template.Render());
        }

        /// <summary>
        /// 同步渲染：标签结果为非泛型 Task 时不输出内容
        /// </summary>
        [Fact]
        public void TestRenderNonGenericTask()
        {
            var template = CreateInterpretedEngine().CreateTemplate("a${task}b");
            template.Set("task", CreateNonGenericTask());
            Assert.Equal("ab", template.Render());
        }

        /// <summary>
        /// 异步渲染：Task&lt;T&gt; 结果会被输出
        /// </summary>
        [Fact]
        public async Task TestRenderAsyncTaskResult()
        {
            var template = CreateInterpretedEngine().CreateTemplate("${task}");
            template.Set("task", Task.FromResult(42));
            Assert.Equal("42", await template.RenderAsync());
        }

        /// <summary>
        /// 异步渲染：非泛型 Task 结果不输出内容
        /// </summary>
        [Fact]
        public async Task TestRenderAsyncNonGenericTask()
        {
            var template = CreateInterpretedEngine().CreateTemplate("a${task}b");
            template.Set("task", CreateNonGenericTask());
            Assert.Equal("ab", await template.RenderAsync());
        }

        /// <summary>
        /// 创建已完成且非泛型的 <see cref="Task"/>（Task.CompletedTask 实际为 Task&lt;VoidTaskResult&gt;）
        /// </summary>
        private static Task CreateNonGenericTask()
        {
            var task = new Task(() => { });
            task.RunSynchronously();
            return task;
        }

        /// <summary>
        /// 同步渲染：TemplateException 会被记录，并输出源码
        /// </summary>
        [Fact]
        public void TestRenderThrowsTemplateException()
        {
            var engine = CreateInterpretedEngine();
            engine.Register(new DynamicVisitor(typeof(TestTag), (t, c) =>
            {
                throw new TemplateException("boom");
            }));

            var ctx = engine.CreateContext();
            ctx.ThrowExceptions = false;

            using (var writer = new StringWriter())
            {
                ctx.Render(writer, CreateTags());
                Assert.Equal("boom", ctx.Error!.Message);
                Assert.Equal("TestTag", writer.ToString());
            }
        }

        /// <summary>
        /// 同步渲染：普通异常会包装为 ParseException
        /// </summary>
        [Fact]
        public void TestRenderThrowsException()
        {
            var engine = CreateInterpretedEngine();
            engine.Register(new DynamicVisitor(typeof(TestTag), (t, c) =>
            {
                throw new InvalidOperationException("inner");
            }));

            var ctx = engine.CreateContext();
            ctx.ThrowExceptions = false;

            using (var writer = new StringWriter())
            {
                ctx.Render(writer, CreateTags());
                var exception = Assert.IsType<ParseException>(ctx.Error);
                Assert.Equal("inner", exception.GetBaseException().Message);
                Assert.Equal("TestTag", writer.ToString());
            }
        }

        /// <summary>
        /// 异步渲染：TemplateException 与普通异常均被记录
        /// </summary>
        [Fact]
        public async Task TestRenderAsyncThrowsException()
        {
            var engine = CreateInterpretedEngine();
            engine.Register(new DynamicVisitor(typeof(TestTag), (t, c) =>
            {
                throw new TemplateException("boom");
            }));

            var ctx = engine.CreateContext();
            ctx.ThrowExceptions = false;

            using (var writer = new StringWriter())
            {
                await ctx.RenderAsync(writer, CreateTags(), TestContext.Current.CancellationToken);
                Assert.IsType<TemplateException>(ctx.Error);
                Assert.Equal("TestTag", writer.ToString());
            }

            var engine2 = CreateInterpretedEngine();
            engine2.Register(new DynamicVisitor(typeof(TestTag), (t, c) =>
            {
                throw new InvalidOperationException("inner");
            }));

            var ctx2 = engine2.CreateContext();
            ctx2.ThrowExceptions = false;

            using (var writer = new StringWriter())
            {
                await ctx2.RenderAsync(writer, CreateTags(), TestContext.Current.CancellationToken);
                var exception = Assert.IsType<ParseException>(ctx2.Error);
                Assert.Equal("inner", exception.GetBaseException().Message);
                Assert.Equal("TestTag", writer.ToString());
            }
        }

        /// <summary>
        /// 空集合与空 writer 的处理
        /// </summary>
        [Fact]
        public async Task TestRenderEmptyCollection()
        {
            var ctx = CreateInterpretedEngine().CreateContext();
            using (var writer = new StringWriter())
            {
                ctx.Render(writer, new TagCollection());
                await ctx.RenderAsync(writer, new TagCollection(), TestContext.Current.CancellationToken);
                ctx.Render(writer, null!);
                await ctx.RenderAsync(writer, null!, TestContext.Current.CancellationToken);
                Assert.Equal(string.Empty, writer.ToString());
            }

            Assert.Throws<ArgumentNullException>(() => ctx.Render(null!, CreateTags()));
            await Assert.ThrowsAsync<ArgumentNullException>(() => ctx.RenderAsync(null!, CreateTags(), TestContext.Current.CancellationToken));
        }

        private static TagCollection CreateTags()
        {
            var tags = new TagCollection();
            tags.Add(new TestTag());
            return tags;
        }
    }
}
