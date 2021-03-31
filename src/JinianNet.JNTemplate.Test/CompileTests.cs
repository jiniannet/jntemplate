using JinianNet.JNTemplate;
using JinianNet.JNTemplate.Test.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 测试编译方法
    /// </summary>
    public partial class TagsTests : TagsTestBase
    {
        /// <summary>
        /// 测试预编译
        /// </summary>
        [Fact]
        public void TestPrecompiled()
        {
            var path = string.Join(Path.DirectorySeparatorChar, new string[] { Environment.CurrentDirectory, "templets", "default", "questionlist.html" });
            var r = Engine.CompileFile(path, path, (ctx) =>
             {
                 ctx.Set("Site", typeof(SiteInfo));
                 ctx.Set("func", typeof(TemplateMethod));
                 ctx.CurrentPath = System.IO.Path.GetDirectoryName(path);
             });
            Assert.NotNull(r);
        }


        /// <summary>
        /// 测试预编译
        /// </summary>
        [Fact]
        public void TestCompiledText()
        {
            var name = "hello";
            var r = Engine.Compile(name, "hello,$name", (ctx) =>
            {
                ctx.Set("name", typeof(string));
            });
            Assert.NotNull(r);
        }
    }
}
