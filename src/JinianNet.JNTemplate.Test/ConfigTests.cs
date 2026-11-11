using static System.Net.Mime.MediaTypeNames;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 配置测试（因为配置是全局的，会影响其它类，所以需要单独拿出来测试）
    /// </summary>
    public class ConfigTests : TagsTestBase
    {

        /// <summary>
        /// 测试配置:自定义标签前后缀
        /// </summary>
        [Fact]
        public void TestConfig()
        {

            var templateContent = "你好，@name,欢迎来到{{name}}的世界";
            var text = "你好，jntemplate,欢迎来到jntemplate的世界";

            TestCreateRunDoubleEngine(templateContent,
                text,
                engine => engine.Configure(c =>
                {
                    c.TagFlag = '@';
                    c.TagSuffix = "}}";
                    c.TagPrefix = "{{";
                }),
                template => template.Set("name", "jntemplate"));

        }

        /// <summary>
        /// 测试配置:自定义标签前后缀2
        /// </summary>
        [Fact]
        public void TestTagConfig()
        {
            var templateContent = "hello,{$username}{{jsVar}}{$year}!!";
            var text = "hello,jntemplate{{jsVar}}2020!!";

            TestCreateRunDoubleEngine(templateContent,
                text,
                engine => engine.Configure(c =>
                {
                    c.TagFlag = '$';
                    c.TagSuffix = "}";
                    c.TagPrefix = "{$";
                }),
                template =>
                {
                    template.Set("username", "jntemplate");
                    template.Set("year", 2020);
                });
        }

        /// <summary>
        /// 测试配置:禁用简写
        /// </summary>
        [Fact]
        public void TestDisableeLogogram()
        {
            var templateContent = "var $a =34";
            var text = "var $a =34";

            TestCreateRunDoubleEngine(templateContent,
                text,
                engine => engine.Configure(c =>
                {
                    c.DisableLogogram = true;
                }),
                template =>
                {
                });
        }

        /// <summary>
        /// 测试标签前后空白字符串处理
        /// </summary>
        [Fact]
        public void TestStripWhiteSpace()
        {
            var templateContent = @"
your data is:
$set(key1=1)
$set(key2=2)
$set(key3=3)
$set(key4=4)
$set(key5=5)
$set(key6=6)
$key5";
            var text = "your data is:5";
            TestCreateRunDoubleEngine(templateContent,
                text,
                template => template.Context.OutMode = OutMode.StripWhiteSpace);
        }


        /// <summary>
        /// 测试标签输出模式
        /// </summary>
        [Fact]
        public void TestAutoMode()
        {
            var templateContent =
@"<ul>
$for(i =0;i<5;i++)
<li>$i</li>
$end
<ul>";
            var text = @"<ul>
<li>0</li>
<li>1</li>
<li>2</li>
<li>3</li>
<li>4</li>
<ul>";
            TestCreateRunDoubleEngine(templateContent,
                text,
                template => template.Context.OutMode = OutMode.Auto);
        }



        /// <summary>
        /// 测试全局数据
        /// </summary>
        [Fact]
        public void TestGlobalData()
        {
            var templateContent = "hello,${name}";
            var text = $"hello,jntemplate";

            TestCreateRunDoubleEngine(templateContent,
                text,
                engine => engine.Configure(c =>
                {
                    c.Data.Set("name", "jntemplate");
                }),
                template =>
                {
                });
        }
    }
}
