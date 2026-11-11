namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// <see cref="Extensions"/> 类型转换扩展方法测试
    /// </summary>
    public class ExtensionsTests
    {
        /// <summary>
        /// object 转 float：null、原生类型、布尔、字符串四个分支
        /// </summary>
        [Fact]
        public void TestToRequiredFloat()
        {
            Assert.Equal(1.5f, ((object)null).ToRequiredFloat(1.5f));
            Assert.Equal(2.5f, ((object)2.5f).ToRequiredFloat(0f));
            Assert.Equal(1f, ((object)true).ToRequiredFloat(0f));
            Assert.Equal(0f, ((object)false).ToRequiredFloat(1f));
            Assert.Equal(3.5f, ((object)"3.5").ToRequiredFloat(0f));
            Assert.Equal(9f, ((object)"not-a-number").ToRequiredFloat(9f));
            Assert.Equal(4f, ((object)4).ToRequiredFloat());

            Assert.Equal(1.5f, ((string)null).ToRequiredFloat(1.5f));
            Assert.Equal(1.5f, "1.5".ToRequiredFloat());
            Assert.Equal(7f, "abc".ToRequiredFloat(7f));
        }

        /// <summary>
        /// object 转 double：null、原生类型、布尔、字符串四个分支
        /// </summary>
        [Fact]
        public void TestToRequiredDouble()
        {
            Assert.Equal(1.5d, ((object)null).ToRequiredDouble(1.5d));
            Assert.Equal(2.5d, ((object)2.5d).ToRequiredDouble(0d));
            Assert.Equal(1d, ((object)true).ToRequiredDouble(0d));
            Assert.Equal(0d, ((object)false).ToRequiredDouble(1d));
            Assert.Equal(3.5d, ((object)"3.5").ToRequiredDouble(0d));
            Assert.Equal(9d, ((object)"x").ToRequiredDouble(9d));
            Assert.Equal(4d, ((object)4).ToRequiredDouble());

            Assert.Equal(1.5d, ((string)null).ToRequiredDouble(1.5d));
            Assert.Equal(1.5d, "1.5".ToRequiredDouble());
            Assert.Equal(7d, "abc".ToRequiredDouble(7d));
        }

        /// <summary>
        /// object 转 decimal：null、原生类型、布尔、字符串以及定长格式化
        /// </summary>
        [Fact]
        public void TestToRequiredDecimal()
        {
            Assert.Equal(1.5m, ((object)null).ToRequiredDecimal(1.5m));
            Assert.Equal(2.5m, ((object)2.5m).ToRequiredDecimal(0m));
            Assert.Equal(1m, ((object)true).ToRequiredDecimal(0m));
            Assert.Equal(0m, ((object)false).ToRequiredDecimal(1m));
            Assert.Equal(3.5m, ((object)"3.5").ToRequiredDecimal(0m));
            Assert.Equal(9m, ((object)"x").ToRequiredDecimal(9m));
            Assert.Equal(4m, ((object)4).ToRequiredDecimal());

            Assert.Equal(1.5m, ((string)null).ToRequiredDecimal(1.5m));
            Assert.Equal(1.5m, "1.5".ToRequiredDecimal());
            Assert.Equal(7m, "abc".ToRequiredDecimal(7m));

            // 定长格式化时会去掉多余的 0 与小数点
            Assert.Equal(12m, 12d.ToRequiredDecimal(2));
            Assert.Equal(12.5m, 12.5d.ToRequiredDecimal(4));
        }

        /// <summary>
        /// 字符串转布尔：null、true 文本、数值 1、非法值
        /// </summary>
        [Fact]
        public void TestToRequiredBoolean()
        {
            Assert.True(((object)null).ToRequiredBoolean(true));
            Assert.True(((object)true).ToRequiredBoolean(false));
            Assert.False(((object)false).ToRequiredBoolean(true));
            Assert.False(((object)"abc").ToRequiredBoolean(false));
            Assert.True(((object)"true").ToRequiredBoolean(false));
            Assert.True(((object)1).ToRequiredBoolean(false));
            Assert.False(((object)0).ToRequiredBoolean(false));

            Assert.False(((string)null).ToRequiredBoolean(false));
            Assert.True("true".ToRequiredBoolean(false));
            Assert.True("True".ToRequiredBoolean(false));
            Assert.True("1".ToRequiredBoolean(false));
            Assert.False("0".ToRequiredBoolean(false));
            Assert.False("abc".ToRequiredBoolean(false));
        }

        /// <summary>
        /// object 转 int：null、原生类型、布尔、字符串四个分支
        /// </summary>
        [Fact]
        public void TestToRequiredInt()
        {
            Assert.Equal(3, ((object)null).ToRequiredInt(3));
            Assert.Equal(5, ((object)5).ToRequiredInt(0));
            Assert.Equal(1, ((object)true).ToRequiredInt(0));
            Assert.Equal(0, ((object)false).ToRequiredInt(1));
            Assert.Equal(6, ((object)"6").ToRequiredInt(0));
            Assert.Equal(9, ((object)"x").ToRequiredInt(9));
            Assert.Equal(4, ((object)4).ToRequiredInt());

            Assert.Equal(3, ((string)null).ToRequiredInt(3));
            Assert.Equal(5, "5".ToRequiredInt());
            Assert.Equal(7, "abc".ToRequiredInt(7));
        }

        /// <summary>
        /// object 转 short
        /// </summary>
        [Fact]
        public void TestToRequiredShort()
        {
            Assert.Equal((short)3, ((object)null).ToRequiredShort(3));
            Assert.Equal((short)5, ((object)(short)5).ToRequiredShort(0));
            Assert.Equal((short)1, ((object)true).ToRequiredShort(0));
            Assert.Equal((short)0, ((object)false).ToRequiredShort(1));
            Assert.Equal((short)6, ((object)"6").ToRequiredShort(0));
            Assert.Equal((short)9, ((object)"x").ToRequiredShort(9));
            Assert.Equal((short)4, ((object)(short)4).ToRequiredShort());

            Assert.Equal((short)3, ((string)null).ToRequiredShort(3));
            Assert.Equal((short)5, "5".ToRequiredShort());
            Assert.Equal((short)7, "abc".ToRequiredShort(7));
        }

        /// <summary>
        /// object 转 long，以及 Guid 转 long
        /// </summary>
        [Fact]
        public void TestToRequiredLong()
        {
            Assert.Equal(3L, ((object)null).ToRequiredLong(3L));
            Assert.Equal(5L, ((object)5L).ToRequiredLong(0L));
            Assert.Equal(6L, ((object)"6").ToRequiredLong(0L));
            Assert.Equal(9L, ((object)"x").ToRequiredLong(9L));
            Assert.Equal(4L, ((object)4L).ToRequiredLong());

            Assert.Equal(3L, ((string)null).ToRequiredLong(3L));
            Assert.Equal(5L, "5".ToRequiredLong());
            Assert.Equal(7L, "abc".ToRequiredLong(7L));

            var guid = Guid.NewGuid();
            Assert.Equal(BitConverter.ToInt64(guid.ToByteArray(), 0), guid.ToRequiredLong());
        }

        /// <summary>
        /// object 转 uint
        /// </summary>
        [Fact]
        public void TestToRequiredUInt()
        {
            Assert.Equal(3u, ((object)null).ToRequiredUInt(3u));
            Assert.Equal(5u, ((object)5u).ToRequiredUInt(0u));
            Assert.Equal(1u, ((object)true).ToRequiredUInt(0u));
            Assert.Equal(0u, ((object)false).ToRequiredUInt(1u));
            Assert.Equal(6u, ((object)"6").ToRequiredUInt(0u));
            Assert.Equal(9u, ((object)"x").ToRequiredUInt(9u));
            Assert.Equal(4u, ((object)4u).ToRequiredUInt());

            Assert.Equal(3u, ((string)null).ToRequiredUInt(3u));
            Assert.Equal(5u, "5".ToRequiredUInt());
            Assert.Equal(7u, "abc".ToRequiredUInt(7u));
        }

        /// <summary>
        /// object 转 ushort
        /// </summary>
        [Fact]
        public void TestToRequiredUShort()
        {
            Assert.Equal((ushort)3, ((object)null).ToRequiredUShort(3));
            Assert.Equal((ushort)5, ((object)(ushort)5).ToRequiredUShort(0));
            Assert.Equal((ushort)1, ((object)true).ToRequiredUShort(0));
            Assert.Equal((ushort)0, ((object)false).ToRequiredUShort(1));
            Assert.Equal((ushort)6, ((object)"6").ToRequiredUShort(0));
            Assert.Equal((ushort)9, ((object)"x").ToRequiredUShort(9));
            Assert.Equal((ushort)4, ((object)(ushort)4).ToRequiredUShort());

            Assert.Equal((ushort)3, ((string)null).ToRequiredUShort(3));
            Assert.Equal((ushort)5, "5".ToRequiredUShort());
            Assert.Equal((ushort)7, "abc".ToRequiredUShort(7));
        }

        /// <summary>
        /// object 转 ulong
        /// </summary>
        [Fact]
        public void TestToRequiredULong()
        {
            Assert.Equal(3UL, ((object)null).ToRequiredULong(3UL));
            Assert.Equal(5UL, ((object)5UL).ToRequiredULong(0UL));
            Assert.Equal(6UL, ((object)"6").ToRequiredULong(0UL));
            Assert.Equal(9UL, ((object)"x").ToRequiredULong(9UL));
            Assert.Equal(4UL, ((object)4UL).ToRequiredULong());

            Assert.Equal(3UL, ((string)null).ToRequiredULong(3UL));
            Assert.Equal(5UL, "5".ToRequiredULong());
            Assert.Equal(7UL, "abc".ToRequiredULong(7UL));
        }

        /// <summary>
        /// object 转 byte
        /// </summary>
        [Fact]
        public void TestToRequiredByte()
        {
            Assert.Equal((byte)3, ((object)null).ToRequiredByte(3));
            Assert.Equal((byte)5, ((object)(byte)5).ToRequiredByte(0));
            Assert.Equal((byte)1, ((object)true).ToRequiredByte(0));
            Assert.Equal((byte)0, ((object)false).ToRequiredByte(1));
            Assert.Equal((byte)6, ((object)"6").ToRequiredByte(0));
            Assert.Equal((byte)9, ((object)"x").ToRequiredByte(9));
            Assert.Equal((byte)4, ((object)(byte)4).ToRequiredByte());

            Assert.Equal((byte)3, ((string)null).ToRequiredByte(3));
            Assert.Equal((byte)5, "5".ToRequiredByte());
            Assert.Equal((byte)7, "abc".ToRequiredByte(7));
        }

        /// <summary>
        /// 集合辅助方法：空值不入集合
        /// </summary>
        [Fact]
        public void TestAddNotNullObject()
        {
            var list = new List<string>();
            list.AddNotNullObject("a");
            list.AddNotNullObject(null);
            Assert.Single(list);
            Assert.Equal("a", list[0]);
        }

        /// <summary>
        /// 类型代码：枚举与自定义类型返回 Empty
        /// </summary>
        [Fact]
        public void TestToTypeCode()
        {
            Assert.Equal(TypeCode.Int32, typeof(int).ToTypeCode());
            Assert.Equal(TypeCode.String, typeof(string).ToTypeCode());
            Assert.Equal(TypeCode.Object, typeof(object).ToTypeCode());
            Assert.Equal(TypeCode.Empty, typeof(ExtensionsTests).ToTypeCode());
            Assert.Equal(TypeCode.Empty, typeof(EngineMode).ToTypeCode());
        }
    }
}
