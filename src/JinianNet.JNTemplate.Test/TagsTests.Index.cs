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
        public void TestIntArray()
        {
            var templateContent = "$data[0]";
            TestCreateRunDoubleEngine(templateContent,
              "7",
              template =>
              {
                  template.Set("data", new int[] { 7, 0, 2, 0, 6 });
              });
        }

        /// <summary>
        /// 双索引测试
        /// </summary>
        [Fact]
        public void TestStringCharArray()
        {
            var templateContent = "$data[0][1]";
            TestCreateRunDoubleEngine(templateContent,
              "b",
              template =>
              {
                  template.Set("data", new string[] { "abc", "def", "ghi" });
              });
        }
        /// <summary>
        /// 测试索引嵌套
        /// </summary>
        [Fact]
        public void TestIndexNesting()
        {
            var templateContent = "$data[arr[0]]";
            TestCreateRunDoubleEngine(templateContent,
                "def",
                  template =>
                  {
                      template.Set("data", new string[] { "abc", "def", "ghi" });
                      template.Set("arr", new int[] { 1, 4, 0, 8, 3 });
                  });
        }

        /// <summary>
        /// 测试方法与索引的混合写法
        /// </summary>
        [Fact]
        public void TestIndexAndFunc()
        {
            var templateContent = "${test(8,-2).ToString()[0]}";
            TestCreateRunDoubleEngine(templateContent,
                "2",
                template =>
                {
                    template.Set("test", new Func<int, int, int>((x, y) =>
                    {
                        var r = (x + y) * 35;
                        return r;
                    }));
                });
        }



        /// <summary>
        /// 测试索引取值与方法标签
        /// </summary>
        [Fact]
        public void TestIndexValue()
        {
            var templateContent = "$data.Get(0)";
            TestCreateRunDoubleEngine(templateContent,
                "7",
                template =>
                {
                    template.Set("data", new int[] { 7, 0, 2, 0, 6 });
                });
        }


        /// <summary>
        /// 测试字典
        /// </summary>
        [Fact]
        public void TestDictionaryString()
        {
            var templateContent = "$data[\"name\"]";
            TestCreateRunDoubleEngine(templateContent,
                "你好！jntemplate",
                template =>
                {
                    var dic = new System.Collections.Generic.Dictionary<string, string>();
                    dic["name"] = "你好！jntemplate";
                    dic["age"] = "1";
                    template.Set("data", dic);
                });
        }

        /// <summary>
        /// 测试字典
        /// </summary>
        [Fact]
        public void TestDictionaryObject()
        {
            var templateContent = "$data[\"name\"]";
            TestCreateRunDoubleEngine(templateContent,
                "你好！jntemplate",
                template =>
                {
                    var dic = new Dictionary<string, object>();
                    dic["name"] = "你好！jntemplate";
                    dic["age"] = "1";
                    template.Set("data", dic);
                });
        }


        /// <summary>
        /// 测试字典
        /// </summary>
        [Fact]
        public void TestDictOutput()
        {
            var templateContent = "$dict";
            TestCreateRunDoubleEngine(templateContent,
                null,
                template =>
                {
                    template.Set("dict", new Dictionary<string, object>());
                },
                render => Assert.StartsWith("System.Collections.Generic.Dictionary`2", render));
        }

        /// <summary>
        /// 通过get_Item获取索引(数字索引)
        /// </summary>
        [Fact]
        public void TestGetItemInt()
        {
            var templateContent = "$data.get_Item(0)";
            TestCreateRunDoubleEngine(templateContent,
                "7",
                template =>
                {
                    template.Set("data", new System.Collections.Generic.List<int>(new int[] { 7, 0, 2, 0, 6 }));
                });
        }

        /// <summary>
        /// 通过get_Item获取索引
        /// </summary>
        [Fact]
        public void TestGetItem()
        {
            var templateContent = "$data.get_Item(\"name\")";
            TestCreateRunDoubleEngine(templateContent,
                "你好！jntemplate",
                template =>
                {
                    var dic = new Dictionary<string, string>();
                    dic["name"] = "你好！jntemplate";
                    dic["age"] = "1";
                    template.Set("data", dic);
                });
        }
    }
}
