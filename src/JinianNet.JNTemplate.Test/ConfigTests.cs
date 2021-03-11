using Xunit;
using System;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 配置测试（因为配置是全局的，会影响其它类，所以需要单独拿出来测试）
    /// </summary>
    public class ConfigTests
    {
#if testconfig
        /// <summary>
        /// 自定义标签前后缀测试
        /// </summary>
        [Fact]
        public void TestConfig()
        {
            Engine.Configure((c) =>
            {
                c.TagFlag = '@';
                c.TagSuffix = "}}";
                c.TagPrefix = "{{";
            });

            var templateContent = "你好，@name,欢迎来到{{name}}的世界";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("name", "jntemplate");
            var render = template.Render();
            Console.WriteLine(render);
            Assert.Equal("你好，jntemplate,欢迎来到jntemplate的世界", render);

            Engine.Configure(Configuration.EngineConfig.CreateDefault());

        }

        /// <summary>
        /// 测试配置
        /// </summary>
        [Fact]
        public void TestConfig2()
        {
            Engine.Configure((c) =>
            {
                c.DisableeLogogram = true;
            });
            var templateContent = "var $a =34;";
            var template = Engine.CreateTemplate(templateContent);
            var render = template.Render();
            Assert.Equal("var $a =34;", render);
            Engine.Configure(Configuration.EngineConfig.CreateDefault());
        }
#endif

    }
}
