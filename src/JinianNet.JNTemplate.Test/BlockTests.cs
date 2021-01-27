using JinianNet.JNTemplate;
using System;
using System.Collections.Generic;
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
        /// 测试索引取值与方法标签
        /// </summary>
        [Fact]
        public async Task TestLoadParent()
        {
            var templateContent = "$load(\"public.html\")";
            var template = Engine.CreateTemplate(templateContent);

#if NETCOREAPP
            template.Context.CurrentPath = new System.IO.DirectoryInfo(System.AppContext.BaseDirectory).Parent.Parent.Parent.FullName + System.IO.Path.DirectorySeparatorChar.ToString() + "templets";
#else
            template.Context.CurrentPath = new System.IO.DirectoryInfo(System.Environment.CurrentDirectory).Parent.Parent.FullName + System.IO.Path.DirectorySeparatorChar.ToString() + "templets";
#endif

            //var loader = new JinianNet.JNTemplate.FileLoader();
            //loader.ResourceDirectories = new List<string>(new string[] { new System.IO.DirectoryInfo(System.Environment.CurrentDirectory).Parent.Parent.FullName + System.IO.Path.DirectorySeparatorChar.ToString() + "templets" });
            //Engine.SetLodeProvider(loader);

            var render = await Excute(template);
            Assert.Equal("this is public", render);
        }

        /// <summary>
        /// 测试索引取值与方法标签
        /// </summary>
        [Fact]
        public async Task TestInclub()
        {
            var templateContent = "$include(\"include/header.txt\")";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("name","jntemplate");
#if NETCOREAPP
            template.Context.CurrentPath = new System.IO.DirectoryInfo(System.AppContext.BaseDirectory).Parent.Parent.Parent.FullName + System.IO.Path.DirectorySeparatorChar.ToString() + "templets" + System.IO.Path.DirectorySeparatorChar.ToString() + "default";
#else
            template.Context.CurrentPath = new System.IO.DirectoryInfo(System.Environment.CurrentDirectory).Parent.Parent.FullName + System.IO.Path.DirectorySeparatorChar.ToString() + "templets" + System.IO.Path.DirectorySeparatorChar.ToString() + "default";
#endif
            var render = await Excute(template);
            Assert.Equal("你好，$name", render);
        }


        /// <summary>
        /// 测试索引取值与方法标签
        /// </summary>
        [Fact]
        public async Task TestLoad()
        {
            var templateContent = "$load(\"include/header.txt\")";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("name","jntemplate");
#if NETCOREAPP
            template.Context.CurrentPath = new System.IO.DirectoryInfo(System.AppContext.BaseDirectory).Parent.Parent.Parent.FullName + System.IO.Path.DirectorySeparatorChar.ToString() + "templets" + System.IO.Path.DirectorySeparatorChar.ToString() + "default";
#else
            template.Context.CurrentPath = new System.IO.DirectoryInfo(System.Environment.CurrentDirectory).Parent.Parent.FullName + System.IO.Path.DirectorySeparatorChar.ToString() + "templets" + System.IO.Path.DirectorySeparatorChar.ToString() + "default";
#endif
            var render = await Excute(template);
            Assert.Equal("你好，jntemplate", render);
        }

        /// <summary>
        /// 测试Layout
        /// </summary>
        [Fact]
        public async Task TestLayout()
        {
            var templateContent = "$layout(\"include/layout.txt\")<h1>主体内容<h1>";
            var template = Engine.CreateTemplate(templateContent);
#if NETCOREAPP
            template.Context.CurrentPath = new System.IO.DirectoryInfo(System.AppContext.BaseDirectory).Parent.Parent.Parent.FullName + System.IO.Path.DirectorySeparatorChar.ToString() + "templets" + System.IO.Path.DirectorySeparatorChar.ToString() + "default";
#else
            template.Context.CurrentPath = new System.IO.DirectoryInfo(System.Environment.CurrentDirectory).Parent.Parent.FullName + System.IO.Path.DirectorySeparatorChar.ToString() + "templets" + System.IO.Path.DirectorySeparatorChar.ToString() + "default";
#endif
            var render = await Excute(template);
            Assert.Equal("这是LAYOUT头部<h1>主体内容<h1>这是LAYOUT尾部", render);
        }
    }
}
