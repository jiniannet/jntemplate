namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// <see cref="Utility"/> 工具方法测试
    /// </summary>
    public class UtilityTests
    {
        /// <summary>
        /// 字符串转布尔值
        /// </summary>
        [Theory]
        [InlineData("true", true)]
        [InlineData("TRUE", true)]
        [InlineData("True", true)]
        [InlineData("false", false)]
        [InlineData("1", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void TestStringToBoolean(string input, bool expected)
        {
            Assert.Equal(expected, Utility.StringToBoolean(input));
        }

        /// <summary>
        /// 字符串转换为布尔：空值为 false，其它为 true
        /// </summary>
        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("0", true)]
        [InlineData("false", true)]
        public void TestToBooleanString(string input, bool expected)
        {
            Assert.Equal(expected, Utility.ToBoolean(input));
        }

        /// <summary>
        /// 数值类型转换为布尔
        /// </summary>
        [Fact]
        public void TestToBooleanNumbers()
        {
            Assert.False(Utility.ToBoolean(0));
            Assert.True(Utility.ToBoolean(1));
            Assert.True(Utility.ToBoolean(-1));

            Assert.False(Utility.ToBoolean(0L));
            Assert.True(Utility.ToBoolean(100L));

            Assert.False(Utility.ToBoolean((short)0));
            Assert.True(Utility.ToBoolean((short)2));

            Assert.False(Utility.ToBoolean(0d));
            Assert.True(Utility.ToBoolean(0.1d));

            Assert.False(Utility.ToBoolean(0m));
            Assert.True(Utility.ToBoolean(0.1m));

            Assert.False(Utility.ToBoolean(0f));
            Assert.True(Utility.ToBoolean(0.1f));
        }

        /// <summary>
        /// object 转换为布尔：按运行时类型分支处理
        /// </summary>
        [Fact]
        public void TestToBooleanObject()
        {
            Assert.False(Utility.ToBoolean((object)null));
            Assert.False(Utility.ToBoolean((object)0));
            Assert.True(Utility.ToBoolean((object)1));
            Assert.True(Utility.ToBoolean((object)2L));
            Assert.True(Utility.ToBoolean((object)(ushort)3));
            Assert.True(Utility.ToBoolean((object)4u));
            Assert.True(Utility.ToBoolean((object)5ul));
            Assert.False(Utility.ToBoolean((object)0m));
            Assert.True(Utility.ToBoolean((object)0.5d));
            Assert.True(Utility.ToBoolean((object)0.5f));
            Assert.False(Utility.ToBoolean((object)""));
            Assert.True(Utility.ToBoolean((object)"a"));
            Assert.False(Utility.ToBoolean((object)false));
            Assert.True(Utility.ToBoolean((object)true));
            // 默认分支：非基础类型一律 true
            Assert.True(Utility.ToBoolean((object)new object()));
            Assert.True(Utility.ToBoolean((object)DateTime.MinValue));
        }

        /// <summary>
        /// 哈希值与模板名称
        /// </summary>
        [Fact]
        public void TestToHashCodeAndTemplateName()
        {
            Assert.Equal(0L, Utility.ToHashCode((Type)null));
            Assert.Equal(0L, Utility.ToHashCode((string)null));
            Assert.NotEqual(0L, Utility.ToHashCode(typeof(string)));
            Assert.NotEqual(0L, Utility.ToHashCode("jntemplate"));

            Assert.StartsWith("ContentTemp", Utility.ContentToTemplateName("hello"));
            Assert.StartsWith("FileTemp", Utility.PathToTemplateName("a.txt"));
            // 相同内容生成相同模板名称
            Assert.Equal(Utility.ContentToTemplateName("hello"), Utility.ContentToTemplateName("hello"));
            Assert.NotEqual(Utility.ContentToTemplateName("hello"), Utility.ContentToTemplateName("hello2"));
        }

        /// <summary>
        /// 字符判断
        /// </summary>
        [Fact]
        public void TestCharHelper()
        {
            Assert.True(Utility.IsLetter('a'));
            Assert.True(Utility.IsLetter('Z'));
            Assert.False(Utility.IsLetter('1'));
            Assert.False(Utility.IsLetter('_'));

            Assert.True(Utility.AllowWord('a'));
            Assert.True(Utility.AllowWord('Z'));
            Assert.True(Utility.AllowWord('9'));
            Assert.True(Utility.AllowWord('_'));
            Assert.False(Utility.AllowWord('-'));
        }

        /// <summary>
        /// 字符串比较（忽略大小写，null 安全）
        /// </summary>
        [Fact]
        public void TestIsEqual()
        {
            Assert.True(Utility.IsEqual("abc", "ABC"));
            Assert.True(Utility.IsEqual(null, null));
            Assert.False(Utility.IsEqual(null, "a"));
            Assert.False(Utility.IsEqual("a", null));
            Assert.False(Utility.IsEqual("a", "b"));
        }

        /// <summary>
        /// 字典转换
        /// </summary>
        [Fact]
        public void TestToDictionary()
        {
            var source = new Dictionary<object, object>
            {
                { "a", 1 },
                { "b", "2" },
                { "c", 3L }
            };

            var dict = Utility.ToDictionary<string, string>(source);
            Assert.Equal(3, dict.Count);
            Assert.Equal("1", dict["a"]);
            Assert.Equal("2", dict["b"]);
            Assert.Equal("3", dict["c"]);

            var intDict = Utility.ToDictionary<string, int>(new Dictionary<object, object> { { "x", 5 } });
            Assert.Equal(5, intDict["x"]);
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        [Fact]
        public void TestChange()
        {
            Assert.Equal(10, Utility.Change<int>(10));
            Assert.Equal("10", Utility.Change<string>(10));
            Assert.Equal(10, Utility.Change<int?>(10));
            Assert.Throws<InvalidCastException>(() => Utility.Change<DateTime>("2020-01-01"));
        }

        /// <summary>
        /// 默认值生成
        /// </summary>
        [Fact]
        public void TestGenerateDefaultValue()
        {
            Assert.Null(Utility.GenerateDefaultValue<string>());
            Assert.Equal(0, Utility.GenerateDefaultValue<int>());
        }

        /// <summary>
        /// 任务同步等待
        /// </summary>
        [Fact]
        public void TestExecuteTask()
        {
            Assert.Equal(5, Utility.ExecuteTask(Task.FromResult(5)));
            Assert.Equal(0, Utility.ExecuteTask<int>(null!));
        }

        /// <summary>
        /// 异步任务转字符串
        /// </summary>
        [Fact]
        public async Task TestTaskToStringAsync()
        {
            Assert.Equal("5", await Utility.ExecuteReturnTaskAsync(Task.FromResult(5)));
            Assert.Null(await Utility.ExecuteReturnTaskAsync(Task.FromResult<string>(null!)));
            Assert.Equal(string.Empty, await Utility.ExecuteTaskAsync(Task.CompletedTask));
            Assert.Equal("text", await Utility.ExecuteStringAsync("text"));
        }

        /// <summary>
        /// TextWriter 异步写入重载
        /// </summary>
        [Fact]
        public async Task TestWriteAsync()
        {
            using (var writer = new StringWriter())
            {
                await Utility.WriteAsync(writer, "a");
                await Utility.WriteAsync(writer, (object)"b");
                await Utility.WriteAsync(writer, (object)null!);
                await Utility.WriteAsync(writer, (ValueType)1);
                await Utility.WriteAsync(writer, (ValueType)null!);
                await Utility.WriteTaskAsync(writer, Task.FromResult("c"));
                await Utility.WriteTaskAsync(writer, Task.FromResult<string>(null!));

                Assert.Equal("ab1c", writer.ToString());
            }
        }
    }
}
