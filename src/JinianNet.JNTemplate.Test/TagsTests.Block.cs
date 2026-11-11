namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 块标签测试
    /// </summary>
    public partial class TagsTests : TagsTestBase
    {



        /// <summary>
        /// 测试子目录LOAD
        /// </summary>
        [Fact]
        public void TestLoad()
        {
            var templateContent = "$load(\"include/header.txt\")";
            TestCreateRunDoubleEngine(templateContent,
                "你好，jntemplate",
                template =>
                {
                    template.Set("name", "jntemplate");
                    template.Context.CurrentPath = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}templets{Path.DirectorySeparatorChar}default";
                });
        }
        /// <summary>
        /// 测试Load
        /// </summary>
        [Fact]
        public void TestLoadParent()
        {
            var templateContent = "$load(\"public.html\")";

            TestCreateRunDoubleEngine(templateContent,
                "this is public",
                template => template.Context.CurrentPath = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}templets");
        }


        /// <summary>
        /// 测试LOAD使用变量
        /// </summary>
        [Fact]
        public void TestLoadVar()
        {
            var templateContent = "$load(path)";
            TestCreateRunDoubleEngine(templateContent,
                "this is public",
                template =>
                {
                    template.Context.CurrentPath = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}templets";
                    template.Set("path", $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}templets{Path.DirectorySeparatorChar}public.html");
                });
        }

        /// <summary>
        /// 测试include
        /// </summary>
        [Fact]
        public void TestInclub()
        {
            var templateContent = "$include(\"include/header.txt\")";
            TestCreateRunDoubleEngine(templateContent,
                "你好，$name",
                template =>
                {
                    template.Set("name", "jntemplate");
                    template.Context.CurrentPath = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}templets{Path.DirectorySeparatorChar}default";
                });
        }


        /// <summary>
        /// 测试include使用变量
        /// </summary>
        [Fact]
        public void TestIncludeVar()
        {
            var templateContent = "$include(path)";
            TestCreateRunDoubleEngine(templateContent,
                "你好，$name",
                template =>
                {
                    template.Set("path", "include/header.txt");
                    template.Context.CurrentPath = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}templets{Path.DirectorySeparatorChar}default";
                });
        }


        /// <summary>
        /// 测试Layout
        /// </summary>
        [Fact]
        public void TestLayout()
        {
            var templateContent = "$layout(\"include/layout.txt\")<h1>主体内容<h1>";

            TestCreateRunDoubleEngine(templateContent,
                "这是LAYOUT头部<h1>主体内容<h1>这是LAYOUT尾部",
                template =>
                {
                    template.Context.CurrentPath = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}templets{Path.DirectorySeparatorChar}default";
                });
        }




        /// <summary>
        /// 测试LoadTemplate
        /// </summary>
        [Fact]
        public void TestLoadTemplate()
        {
            var path = string.Join(Path.DirectorySeparatorChar,
                new string[] {
                    Environment.CurrentDirectory,
                    "templets","default","include","header.txt"
                });
            var render = "你好，jntemplate";

            TestLoadRunDoubleEngine(path, render, template => template.Set("name", "jntemplate"));
        }
    }
}
