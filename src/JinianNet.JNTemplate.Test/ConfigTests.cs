using Xunit;
using System;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 配置测试（因为配置是全局的，会影响其它类，所以需要单独拿出来测试）
    /// </summary>
    public class ConfigTests
    {
        /// <summary>
        /// 测试配置:自定义标签前后缀
        /// </summary>
        [Fact]
        public void TestConfig()
        {
            var engine = new EngineBuilder().Build();
            engine.Configure((c) =>
            {
                c.TagFlag = '@';
                c.TagSuffix = "}}";
                c.TagPrefix = "{{";
            });

            var templateContent = "你好，@name,欢迎来到{{name}}的世界";
            var template = engine.CreateTemplate(templateContent);
            template.Set("name", "jntemplate");
            var render = template.Render();
            Console.WriteLine(render);
            Assert.Equal("你好，jntemplate,欢迎来到jntemplate的世界", render);

        }

        /// <summary>
        /// 测试配置:自定义标签前后缀2
        /// </summary>
        [Fact]
        public void TestTagConfig()
        {
            var engine = new EngineBuilder().Build();
            engine.Configure((c) =>
            {
                c.TagFlag = '$';
                c.TagSuffix = "}";
                c.TagPrefix = "{$";
            });

            var templateContent = "hello,{$username}{{jsVar}}{$year}!!";
            var template = engine.CreateTemplate(templateContent);
            template.Set("username", "jntemplate");
            template.Set("year",2020);
            var render = template.Render();
            Console.WriteLine(render);
            Assert.Equal("hello,jntemplate{{jsVar}}2020!!", render);

        }

        /// <summary>
        /// 测试配置:禁用简写
        /// </summary>
        [Fact]
        public void TestDisableeLogogram()
        {
            var engine = new EngineBuilder().Build();
            engine.Configure((c) =>
            {
                c.DisableeLogogram = true;
            });
            var templateContent = "var $a =34;";
            var template = engine.CreateTemplate(templateContent);
            var render = template.Render();
            Assert.Equal("var $a =34;", render);
        }

    }
}
