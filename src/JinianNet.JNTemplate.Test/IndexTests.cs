using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit; 

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 索引测试
    /// </summary>
    public partial class TagsTests : TagsTestBase
    {

        /// <summary>
        /// 测试数组(普通索引)
        /// </summary>
        [Fact]
        public void TestArray()
        {
            var templateContent = "$data[0]";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("data", new int[] { 7, 0, 2, 0, 6 });
            var render = template.Render();
            Assert.Equal("7", render);
        }

        /// <summary>
        /// 双索引测试
        /// </summary>
        [Fact]
        public void TestDoubleIndex()
        {
            var templateContent = "$data[0][1]";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("data", new string[] { "abc", "def", "ghi" });
            var render = template.Render();
            Assert.Equal("b", render);
        }
        /// <summary>
        /// 测试索引嵌套
        /// </summary>
        [Fact]
        public void TestIndexNesting()
        {
            var templateContent = "$data[arr[0]]";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("data", new string[] { "abc", "def", "ghi" });
            template.Set("arr", new int[] { 1, 4, 0, 8, 3 });
            var render = template.Render();
            Assert.Equal("def", render);
        }

        /// <summary>
        /// 测试方法与索引的混合写法
        /// </summary>
        [Fact]
        public void TestIndexAndFunc()
        {
            var templateContent = "${test(8,-2).ToString()[0]}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("test",new Func<int,int, int>((x,y) =>
            {
                var r = (x + y) * 35;
                return r;
            }));
            var render = template.Render();

            Assert.Equal("2", render);//210
        }



        /// <summary>
        /// 测试索引取值与方法标签
        /// </summary>
        [Fact]
        public void TestIndexValue()
        {
            var templateContent = "$data.Get(0)";  
            var template = Engine.CreateTemplate(templateContent);

            template.Set("data",new int[] { 7, 0, 2, 0, 6 });
            var render = template.Render();
            Assert.Equal("7", render);
        }


        /// <summary>
        /// 测试字典
        /// </summary>
        [Fact]
        public void TestDict()
        {
            var templateContent = "$data[\"name\"]";
            var template = Engine.CreateTemplate("TestDict",templateContent);
            var dic = new System.Collections.Generic.Dictionary<string, string>();
            dic["name"] = "你好！jntemplate";
            dic["age"] = "1";
            template.Set("data",dic);
            var render = template.Render();
            Assert.Equal("你好！jntemplate", render);
        }

        /// <summary>
        /// 测试字典
        /// </summary>
        [Fact]
        public void TestDictAndObject()
        {
            var templateContent = "$data[\"name\"]";
            var template = Engine.CreateTemplate(templateContent);
            var dic = new Dictionary<string, object>();
            dic["name"] = "你好！jntemplate";
            dic["age"] = "1";
            template.Set("data", dic);
            var render = template.Render();
            Assert.Equal("你好！jntemplate", render);
        }


        /// <summary>
        /// 测试字典
        /// </summary>
        [Fact]
        public void TestDictOutput()
        {
            var templateContent = "$dict";
            var template = Engine.CreateTemplate("TestDictOutput",templateContent); 
            template.Set("dict", new Dictionary<string, object>());
            var render = template.Render();
            Assert.StartsWith("System.Collections.Generic.Dictionary`2", render);
        }

        /// <summary>
        /// 通过get_Item获取索引(数字索引)
        /// </summary>
        [Fact]
        public void TestGetItemInt()
        {
            var templateContent = "$data.get_Item(0)"; 
            var template = Engine.CreateTemplate(templateContent);

            template.Set("data",new System.Collections.Generic.List<int>(new int[] { 7, 0, 2, 0, 6 }));
            var render = template.Render();
            Assert.Equal("7", render);
        }

        /// <summary>
        /// 通过get_Item获取索引
        /// </summary>
        [Fact]
        public void TestGetItem()
        {
            var templateContent = "$data.get_Item(\"name\")";
            var template = Engine.CreateTemplate(templateContent);
            var dic = new Dictionary<string, string>();
            dic["name"] = "你好！jntemplate";
            dic["age"] = "1";
            template.Set("data",dic);
            var render = template.Render();
            Assert.Equal("你好！jntemplate", render);
        }
    }
}
