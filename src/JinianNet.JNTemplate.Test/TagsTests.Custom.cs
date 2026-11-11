using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Parsers;
using JinianNet.JNTemplate.Test.ViewModel;
using System.Reflection;
using System.Reflection.Emit;
using static System.Net.Mime.MediaTypeNames;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 更多高极用法测试
    /// </summary>
    public partial class TagsTests : TagsTestBase
    {
        class TestTagVisitor : TagVisitor<TestTag>, ITagVisitor
        {
            public MethodInfo Compile(ITag tag, CompileContext context)
            {
                var t = tag as TestTag;
                var mb = context.CreateReturnMethod<TestTag>(typeof(string));
                var il = mb.GetILGenerator();
                il.Emit(OpCodes.Ldstr, "say " + t.Document);
                il.Emit(OpCodes.Ret);
                return mb.GetBaseDefinition();
            }

            public object Execute(ITag tag, TemplateContext context)
            {
                return $"say {(tag as TestTag).Document}";
            }

            public Type GuessType(ITag tag, CompileContext context)
            {
                return typeof(string);
            }

            public ITag Parse(TemplateParser parser, TokenCollection tc)
            {
                if (tc.Count == 2 && tc.First.Text == ":")
                {
                    return new TestTag
                    {
                        Document = tc[1].Text
                    };
                }
                return null;
            }
        }
        /// <summary>
        /// 测试自定义标签
        /// </summary>
        [Fact]
        public void TestCustomTag()
        {
            TestCreateRunDoubleEngine("${:hello}",
                "say hello",
                engine => engine.Register(new TestTagVisitor()),
                null);
        }

        /// <summary>
        /// 测试加载器
        /// </summary>
        [Fact]
        public void TestLoader()
        {
            var path = "11111";
            var render = "当前是模板：11111 hello,jntemplate";
            var loader = new TestLoader();

            TestLoadRunDoubleEngine(path, render,
                engine => engine.UseLoader(loader),
                template => template.Set("name", "jntemplate"));
        }
    }
}
