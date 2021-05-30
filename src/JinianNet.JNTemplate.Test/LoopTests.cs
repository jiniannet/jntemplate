using JinianNet.JNTemplate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 循环测试（FOR,FOREARCH）
    /// </summary>
    public partial class TagsTests : TagsTestBase
    {
        /// <summary>
        /// 测试FOR
        /// </summary>
        [Fact]
        public void TestFor()
        {
            var templateContent = "$for(i=1;i<4;i=i+1)${i}$end";//"$for(i=1;i<4;i=i+1)${i}$end"
            var template = Engine.CreateTemplate(templateContent);
            var render = template.Render();

            Assert.Equal("123", render);
        }

        /// <summary>
        /// for的不同写法
        /// </summary>
        [Fact]
        public void TestForPlusPlus()
        {
            var templateContent = "$for(i=0;i<3;i++)${i}$end";//"$for(i=1;i<4;i=i+1)${i}$end"
            var template = Engine.CreateTemplate(templateContent);
            var render = template.Render();

            Assert.Equal("012", render);
        }

        /// <summary>
        /// 测试Forea 遍历对象时，Foreach应优先于for
        /// </summary>
        [Fact]
        public void TestForeach()
        {
            var templateContent = "$foreach(i in list)$i$end";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("list", new int[] { 7, 0, 2, 0, 6 });
            var render = template.Render();
            Assert.Equal("70206", render);
        }

        /// <summary>
        /// 测试ForIn
        /// </summary>
        [Fact]
        public void TestForIn()
        {
            var templateContent = "$for(i in list)$i$end";
            var template = Engine.CreateTemplate("TestForIn",templateContent);
            template.Set("list", new int[] { 7, 0, 2, 0, 6 });
            var render = template.Render();
            Assert.Equal("70206", render);
        }

        /// <summary>
        /// 测试IEnumerable<T>
        /// </summary>
        [Fact]
        public void TestForInEnumerable()
        {
            var list = (IEnumerable<int>)new List<int>(new int[] { 1, 2, 3, 4, 5, 6, 7 });
            var templateContent = "$for(i in list)$i$end";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("list", list);
            var render = template.Render();
            Assert.Equal("1234567", render);
        }
    }
}
