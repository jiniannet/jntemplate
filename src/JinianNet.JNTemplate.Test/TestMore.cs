using JinianNet.JNTemplate;
using JinianNet.JNTemplate.Compile;
using JinianNet.JNTemplate.Resources;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Xunit;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 更多高极用法测试
    /// </summary>
    public partial class TagsTests : TagsTestBase
    {
        /// <summary>
        /// 测试自定义标签
        /// </summary>
        [Fact]
        public void YestUserTag()
        {
            //这是一个简单的自定义标签
            Engine.Register<TestTag>(new TestParser(),
                (tag, c) => {
                    var t = tag as TestTag;
                    var mb = Compiler.Builder.CreateReutrnMethod<TestTag>(c, typeof(string));
                    var il = mb.GetILGenerator();
                    il.Emit(OpCodes.Ldstr, "say " + t.Document);
                    il.Emit(OpCodes.Ret);
                    return mb.GetBaseDefinition();
                },
                (tag, c) => typeof(string));

            var templateContent = "${:hello}";
            var template = Engine.CreateTemplate(templateContent);
            var render = template.Render();

            Assert.Equal("say hello", render);
        }

        /// <summary>
        /// 测试加载器
        /// </summary>
        [Fact]
        public void TestLoader()
        {

            Runtime.SetLoader(new TestLoader()); 
            var template = Engine.LoadTemplate("11111");
            template.Set("name","jntemplate");
            var render = template.Render();

            //还源FileLoader
            Engine.Configure((c) => c.Loader = new FileLoader()); 
            Assert.Equal($"当前是模板：11111 hello,jntemplate", render);
        }
    }
}
