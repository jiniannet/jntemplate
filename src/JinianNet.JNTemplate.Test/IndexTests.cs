using JinianNet.JNTemplate;
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
        public async Task TestArray()
        {
            var templateContent = "$data[0]";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("data", new int[] { 7, 0, 2, 0, 6 });
            var render = await Excute(template);
            Assert.Equal("7", render);
        }

        /// <summary>
        /// 双索引测试
        /// </summary>
        [Fact]
        public async Task TestDoubleIndex()
        {
            var templateContent = "$data[0][1]";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("data", new string[] { "abc", "def", "ghi" });
            var render = await Excute(template);
            Assert.Equal("b", render);
        }
        /// <summary>
        /// 测试索引嵌套
        /// </summary>
        [Fact]
        public async Task TestIndexNesting()
        {
            var templateContent = "$data[arr[0]]";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("data", new string[] { "abc", "def", "ghi" });
            template.Set("arr", new int[] { 1, 4, 0, 8, 3 });
            var render = await Excute(template);
            Assert.Equal("def", render);
        }

        /// <summary>
        /// 测试方法与索引的混合写法
        /// </summary>
        [Fact]
        public async Task TestIndexAndFunc()
        {
            var templateContent = "${test(8,-2).ToString()[0]}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("test",new Func<int,int, int>((x,y) =>
            {
                var r = (x + y) * 35;
                return r;
            }));
            var render = await Excute(template);

            Assert.Equal("2", render);//210
        }



        /// <summary>
        /// 测试索引取值与方法标签
        /// </summary>
        [Fact]
        public async Task TestIndexValue()
        {
            var templateContent = "$data.Get(0)"; //数组取值用get即可取到 List<Int32>用get_Item  原因见.NET的索引实现原理
            var template = Engine.CreateTemplate(templateContent);

            template.Set("data",new int[] { 7, 0, 2, 0, 6 });
            var render = await Excute(template);
            Assert.Equal("7", render);
        }


        /// <summary>
        /// 测试字典
        /// </summary>
        [Fact]
        public async Task TestDict()
        {
            var templateContent = "$data[\"name\"]";//索引也可以和属性一样取值，不过推荐用get_Item，且如果索引是数字时，请尽量使用$data.get_Item(index)
            var template = Engine.CreateTemplate(templateContent);
            var dic = new System.Collections.Generic.Dictionary<string, string>();
            dic["name"] = "你好！jntemplate";
            dic["age"] = "1";
            template.Set("data",dic);
            var render = await Excute(template);
            Assert.Equal("你好！jntemplate", render);
        }

        /// <summary>
        /// 通过get_Item获取索引(数字索引)
        /// </summary>
        [Fact]
        public async Task TestGetItemInt()
        {
            var templateContent = "$data.get_Item(0)"; //数组取值用get即可取到 List<Int32>用get_Item  见.NET的索引实现原理
            var template = Engine.CreateTemplate(templateContent);

            template.Set("data",new System.Collections.Generic.List<int>(new int[] { 7, 0, 2, 0, 6 }));
            var render = await Excute(template);
            Assert.Equal("7", render);
        }

        /// <summary>
        /// 通过get_Item获取索引
        /// </summary>
        [Fact]
        public async Task TestGetItem()
        {
            var templateContent = "$data.get_Item(\"name\")";
            var template = Engine.CreateTemplate(templateContent);
            var dic = new Dictionary<string, string>();
            dic["name"] = "你好！jntemplate";
            dic["age"] = "1";
            template.Set("data",dic);
            var render = await Excute(template);
            Assert.Equal("你好！jntemplate", render);
        }
    }
}
