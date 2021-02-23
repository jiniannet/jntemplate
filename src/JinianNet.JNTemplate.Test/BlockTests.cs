using JinianNet.JNTemplate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 块标签测试
    /// </summary>
    public partial class TagsTests : TagsTestBase
    {

        /// <summary>
        /// 测试Load
        /// </summary>
        [Fact]
        public void TestLoadParent()
        {
            var templateContent = "$load(\"public.html\")";
            var template = Engine.CreateTemplate(templateContent);
            template.Context.CurrentPath = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}templets"; 

            var render = template.Render();
            Assert.Equal("this is public", render);
        }


        /// <summary>
        /// 测试索引取值与方法标签
        /// </summary>
        [Fact]
        public void TestLoadVar()
        {
            var templateContent = "$load(path)";
            var template = Engine.CreateTemplate(templateContent);
            template.Context.CurrentPath = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}templets";
            template.Set("path", $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}templets{Path.DirectorySeparatorChar}public.html");
            var render = template.Render();
            Assert.Equal("this is public", render);
        }

        /// <summary>
        /// 测试索引取值与方法标签
        /// </summary>
        [Fact]
        public void TestInclub()
        {
            var templateContent = "$include(\"include/header.txt\")";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("name","jntemplate");
            template.Context.CurrentPath = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}templets{Path.DirectorySeparatorChar}default";
            var render = template.Render();
            Assert.Equal("你好，$name", render);
        }


        /// <summary>
        /// 测试索引取值与方法标签
        /// </summary>
        [Fact]
        public void TestIncludeVar()
        {
            var templateContent = "$include(path)";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("path", "include/header.txt");
            template.Context.CurrentPath = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}templets{Path.DirectorySeparatorChar}default";
            var render = template.Render();
            Assert.Equal("你好，$name", render);
        }

        /// <summary>
        /// 测试索引取值与方法标签
        /// </summary>
        [Fact]
        public void TestLoad()
        {
            var templateContent = "$load(\"include/header.txt\")";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("name","jntemplate");
            template.Context.CurrentPath = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}templets{Path.DirectorySeparatorChar}default";
            var render = template.Render();
            Assert.Equal("你好，jntemplate", render);
        }


        /// <summary>
        /// 测试Layout
        /// </summary>
        [Fact]
        public void TestLayout()
        {
            var templateContent = "$layout(\"include/layout.txt\")<h1>主体内容<h1>";
            var template = Engine.CreateTemplate(templateContent);
            template.Context.CurrentPath = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}templets{Path.DirectorySeparatorChar}default";
            var render = template.Render();
            Assert.Equal("这是LAYOUT头部<h1>主体内容<h1>这是LAYOUT尾部", render);
        }




        /// <summary>
        /// 测试Layout
        /// </summary>
        [Fact]
        public void TestLoadTemplate()
        {
            var path = string.Join(Path.DirectorySeparatorChar, 
                new string[] {
                    Environment.CurrentDirectory,
                    "templets","default","include","header.txt"
                });
            var template = Engine.LoadTemplate(path); 
            template.Set("name", "jntemplate");
            var render = template.Render();
            Assert.Equal("你好，jntemplate", render);
        }
    }
}
