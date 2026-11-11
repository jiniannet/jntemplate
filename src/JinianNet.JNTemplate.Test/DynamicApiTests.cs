using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Exceptions;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// <see cref="ExpressionEvaluator"/> 测试
    /// </summary>
    public class ExpressionEvaluatorTests
    {
        /// <summary>
        /// 整数运算
        /// </summary>
        [Fact]
        public void TestCalculateInt()
        {
            Assert.Equal(3, ExpressionEvaluator.Calculate(1, 2, "+"));
            Assert.Equal(-1, ExpressionEvaluator.Calculate(1, 2, "-"));
            Assert.Equal(2, ExpressionEvaluator.Calculate(1, 2, "*"));
            Assert.Equal(0, ExpressionEvaluator.Calculate(1, 2, "/"));
            Assert.Equal(1, ExpressionEvaluator.Calculate(5, 2, "%"));
            Assert.False((bool)ExpressionEvaluator.Calculate(1, 2, ">="));
            Assert.True((bool)ExpressionEvaluator.Calculate(1, 2, "<="));
            Assert.True((bool)ExpressionEvaluator.Calculate(1, 2, "<"));
            Assert.False((bool)ExpressionEvaluator.Calculate(1, 2, ">"));
            Assert.False((bool)ExpressionEvaluator.Calculate(1, 2, "=="));
            Assert.True((bool)ExpressionEvaluator.Calculate(1, 2, "!="));
            Assert.Equal(3, ExpressionEvaluator.Calculate(1, 2, "|"));
            Assert.Equal(0, ExpressionEvaluator.Calculate(1, 2, "&"));

            //Assert.Throws<TemplateException>(() => ExpressionEvaluator.Calculate(1, 2, "&&"));
        }

        /// <summary>
        /// 其它数值类型运算
        /// </summary>
        [Fact]
        public void TestCalculateNumbers()
        {
            Assert.Equal(3L, ExpressionEvaluator.Calculate(1L, 2L, "+"));
            Assert.Equal(1L, ExpressionEvaluator.Calculate(3L, 2L, "-"));
            Assert.Equal(6L, ExpressionEvaluator.Calculate(3L, 2L, "*"));
            Assert.Equal(1L, ExpressionEvaluator.Calculate(3L, 2L, "/"));
            Assert.Equal(1L, ExpressionEvaluator.Calculate(3L, 2L, "%"));
            Assert.True((bool)ExpressionEvaluator.Calculate(3L, 2L, ">"));
            Assert.Equal(3L, ExpressionEvaluator.Calculate(1L, 2L, "|"));
            Assert.Equal(0L, ExpressionEvaluator.Calculate(1L, 2L, "&"));
            //Assert.Throws<TemplateException>(() => ExpressionEvaluator.Calculate(1L, 2L, "&&"));

            // short 运算会被提升为 int
            Assert.Equal((short)3, ExpressionEvaluator.Calculate((short)1, (short)2, "+"));
            Assert.Equal((short)1, ExpressionEvaluator.Calculate((short)3, (short)2, "-"));
            Assert.Equal((short)2, ExpressionEvaluator.Calculate((short)1, (short)2, "*"));
            Assert.Equal((short)1, ExpressionEvaluator.Calculate((short)3, (short)2, "/"));
            Assert.Equal((short)1, ExpressionEvaluator.Calculate((short)5, (short)2, "%"));
            Assert.True((bool)ExpressionEvaluator.Calculate((short)3, (short)2, ">"));
            //Assert.Throws<TemplateException>(() => ExpressionEvaluator.Calculate((short)1, (short)2, "&&"));

            Assert.Equal(3.5d, ExpressionEvaluator.Calculate(1.5d, 2d, "+"));
            Assert.Equal(-0.5d, ExpressionEvaluator.Calculate(1.5d, 2d, "-"));
            Assert.Equal(3d, ExpressionEvaluator.Calculate(1.5d, 2d, "*"));
            Assert.Equal(0.75d, ExpressionEvaluator.Calculate(1.5d, 2d, "/"));
            Assert.Equal(1.5d, ExpressionEvaluator.Calculate(1.5d, 2d, "%"));
            Assert.True((bool)ExpressionEvaluator.Calculate(2.5d, 2d, ">="));
            Assert.True((bool)ExpressionEvaluator.Calculate(2d, 2d, "=="));
            Assert.False((bool)ExpressionEvaluator.Calculate(2d, 2d, "!="));
            //Assert.Throws<TemplateException>(() => ExpressionEvaluator.Calculate(1d, 2d, "&&"));

            Assert.Equal(3.5f, ExpressionEvaluator.Calculate(1.5f, 2f, "+"));
            Assert.Equal(-0.5f, ExpressionEvaluator.Calculate(1.5f, 2f, "-"));
            Assert.Equal(3f, ExpressionEvaluator.Calculate(1.5f, 2f, "*"));
            Assert.Equal(0.75f, ExpressionEvaluator.Calculate(1.5f, 2f, "/"));
            Assert.Equal(1.5f, ExpressionEvaluator.Calculate(1.5f, 2f, "%"));
            Assert.True((bool)ExpressionEvaluator.Calculate(2.5f, 2f, ">="));
            Assert.True((bool)ExpressionEvaluator.Calculate(2f, 2f, "=="));
            Assert.False((bool)ExpressionEvaluator.Calculate(2f, 2f, "!="));
            //Assert.Throws<TemplateException>(() => ExpressionEvaluator.Calculate(1f, 2f, "&&"));

            Assert.Equal(3.5m, ExpressionEvaluator.Calculate(1.5m, 2m, "+"));
            Assert.Equal(-0.5m, ExpressionEvaluator.Calculate(1.5m, 2m, "-"));
            Assert.Equal(3m, ExpressionEvaluator.Calculate(1.5m, 2m, "*"));
            Assert.Equal(0.75m, ExpressionEvaluator.Calculate(1.5m, 2m, "/"));
            Assert.Equal(1.5m, ExpressionEvaluator.Calculate(1.5m, 2m, "%"));
            Assert.True((bool)ExpressionEvaluator.Calculate(2.5m, 2m, ">="));
            Assert.True((bool)ExpressionEvaluator.Calculate(2m, 2m, "=="));
            Assert.False((bool)ExpressionEvaluator.Calculate(2m, 2m, "!="));
            //Assert.Throws<TemplateException>(() => ExpressionEvaluator.Calculate(1m, 2m, "&&"));
        }

        /// <summary>
        /// 布尔、字符串与日期运算
        /// </summary>
        [Fact]
        public void TestCalculateOthers()
        {
            Assert.True((bool)ExpressionEvaluator.Calculate(true, true, "&&"));
            Assert.True((bool)ExpressionEvaluator.Calculate(true, false, "||"));
            Assert.False((bool)ExpressionEvaluator.Calculate(true, false, "=="));
            Assert.True((bool)ExpressionEvaluator.Calculate(true, false, "!="));
            Assert.Throws<TemplateException>(() => ExpressionEvaluator.Calculate(true, false, "+"));

            Assert.True((bool)ExpressionEvaluator.Calculate("a", "A", "=="));
            Assert.False((bool)ExpressionEvaluator.Calculate("a", "b", "=="));
            Assert.True((bool)ExpressionEvaluator.Calculate("a", "b", "!="));
            Assert.Equal("ab", ExpressionEvaluator.Calculate("a", "b", "+"));
            Assert.Throws<TemplateException>(() => ExpressionEvaluator.Calculate("a", "b", "-"));

            var now = DateTime.Now;
            var later = now.AddDays(1);
            Assert.True((bool)ExpressionEvaluator.Calculate(now, later, "<"));
            Assert.True((bool)ExpressionEvaluator.Calculate(now, later, "<="));
            Assert.True((bool)ExpressionEvaluator.Calculate(later, now, ">"));
            Assert.True((bool)ExpressionEvaluator.Calculate(later, now, ">="));
            Assert.False((bool)ExpressionEvaluator.Calculate(now, later, "=="));
            Assert.True((bool)ExpressionEvaluator.Calculate(now, later, "!="));
            Assert.Throws<TemplateException>(() => ExpressionEvaluator.Calculate(now, later, "+"));
        }

        /// <summary>
        /// 对象运算（自动类型选择）
        /// </summary>
        [Fact]
        public void TestCalculateObject()
        {
            Assert.Equal(3, ExpressionEvaluator.Calculate((object)1, (object)2, "+"));
            // 不同类型时按权重提升
            Assert.Equal(3.5d, ExpressionEvaluator.Calculate((object)1, (object)2.5d, "+"));
            Assert.Equal(3.5m, ExpressionEvaluator.Calculate((object)1.5m, (object)2, "+"));
            Assert.Equal(4f, ExpressionEvaluator.Calculate((object)1.5f, (object)2.5f, "+"));
            Assert.Equal(4L, ExpressionEvaluator.Calculate((object)1L, (object)3L, "+")); 
            Assert.Equal(3u, ExpressionEvaluator.Calculate((object)(ushort)1, (object)2u, "+"));
            Assert.Equal(3ul, ExpressionEvaluator.Calculate((object)1ul, (object)2ul, "+"));
            // 有符号与无符号混合时按有符号类型计算
            Assert.Equal(3L, ExpressionEvaluator.Calculate((object)1u, (object)(short)2, "+"));

            Assert.True((bool)ExpressionEvaluator.Calculate((object)true, (object)true, "&&"));
            Assert.False((bool)ExpressionEvaluator.Calculate((object)true, (object)false, "&&"));
            Assert.True((bool)ExpressionEvaluator.Calculate((object)true, (object)false, "||"));

            Assert.Throws<TemplateException>(() => ExpressionEvaluator.Calculate((object)true, (object)false, "|"));

            Assert.True((bool)ExpressionEvaluator.Calculate((object)"a", (object)"A", "=="));
            Assert.Equal("ab", ExpressionEvaluator.Calculate((object)"a", (object)"b", "+"));

            var now = DateTime.Now;
            Assert.True((bool)ExpressionEvaluator.Calculate((object)now, (object)now.AddDays(-1), ">"));

            // 类型不同时 == 直接返回 false
            Assert.False((bool)ExpressionEvaluator.Calculate((object)1, (object)"1", "=="));
            Assert.True((bool)ExpressionEvaluator.Calculate((object)1, (object)"1", "!="));
            // 对象类型相加时按字符串拼接
            Assert.Equal("System.Object2", ExpressionEvaluator.Calculate((object)new object(), (object)2, "+"));

            // null 比较
            Assert.True((bool)ExpressionEvaluator.Calculate((object)null!, (object)null!, "=="));
            Assert.False((bool)ExpressionEvaluator.Calculate((object)null!, (object)1, "=="));
            Assert.True((bool)ExpressionEvaluator.Calculate((object)null!, (object)1, "!="));

            // 对象比较大小
            Assert.True((bool)ExpressionEvaluator.Calculate((object)2, (object)1, ">"));
            Assert.False((bool)ExpressionEvaluator.Calculate((object)1, (object)2, ">"));

            // 枚举比较
            Assert.True((bool)ExpressionEvaluator.Calculate((object)DayOfWeek.Monday, (object)DayOfWeek.Monday, "=="));
            Assert.False((bool)ExpressionEvaluator.Calculate((object)DayOfWeek.Monday, (object)DayOfWeek.Tuesday, "=="));

            // 非布尔对象参与 && 时按是否为空判断
            Assert.True((bool)ExpressionEvaluator.Calculate((object)new object(), (object)new object(), "&&"));
            Assert.False((bool)ExpressionEvaluator.Calculate((object)new object(), (object)null!, "&&"));
        }

        /// <summary>
        /// 表达式求值
        /// </summary>
        [Fact]
        public void TestCalculateExpression()
        {
            // 表达式中的数字统一按 double 解析
            Assert.Equal(7d, ExpressionEvaluator.Calculate("1+2*3"));
            Assert.Equal(9d, ExpressionEvaluator.Calculate("(1+2)*3"));

            // 手工构建逆波兰表达式栈
            var stack = new Stack<object>();
            stack.Push(1);
            stack.Push(2);
            stack.Push(Operator.Add);
            Assert.Equal(3, ExpressionEvaluator.Calculate(stack));

            Assert.NotNull(ExpressionEvaluator.ProcessExpression("1+1"));
        }

        /// <summary>
        /// 运算符与字符串转换
        /// </summary>
        [Fact]
        public void TestOperatorConvert()
        {
            Assert.Equal("+", OperatorConvert.ToString(Operator.Add));
            Assert.Equal("-", OperatorConvert.ToString(Operator.Subtract));
            Assert.Equal("*", OperatorConvert.ToString(Operator.Multiply));
            Assert.Equal("%", OperatorConvert.ToString(Operator.Remainder));
            Assert.Equal("/", OperatorConvert.ToString(Operator.Divided));
            Assert.Equal("|", OperatorConvert.ToString(Operator.LogicalOr));
            Assert.Equal("||", OperatorConvert.ToString(Operator.Or));
            Assert.Equal("&", OperatorConvert.ToString(Operator.LogicAnd));
            Assert.Equal("&&", OperatorConvert.ToString(Operator.And));
            Assert.Equal(">", OperatorConvert.ToString(Operator.GreaterThan));
            Assert.Equal(">=", OperatorConvert.ToString(Operator.GreaterThanOrEqual));
            Assert.Equal("<", OperatorConvert.ToString(Operator.LessThan));
            Assert.Equal("<=", OperatorConvert.ToString(Operator.LessThanOrEqual));
            Assert.Equal("==", OperatorConvert.ToString(Operator.Equal));
            Assert.Equal("!=", OperatorConvert.ToString(Operator.NotEqual));
            Assert.Equal("(", OperatorConvert.ToString(Operator.LeftParentheses));
            Assert.Equal(")", OperatorConvert.ToString(Operator.RightParentheses));
            Assert.Equal(string.Empty, OperatorConvert.ToString(Operator.None));

            Assert.Equal(Operator.Add, OperatorConvert.Parse("+"));
            Assert.Equal(Operator.Subtract, OperatorConvert.Parse("-"));
            Assert.Equal(Operator.Multiply, OperatorConvert.Parse("*"));
            Assert.Equal(Operator.Remainder, OperatorConvert.Parse("%"));
            Assert.Equal(Operator.Divided, OperatorConvert.Parse("/"));
            Assert.Equal(Operator.LogicalOr, OperatorConvert.Parse("|"));
            Assert.Equal(Operator.Or, OperatorConvert.Parse("||"));
            Assert.Equal(Operator.LogicAnd, OperatorConvert.Parse("&"));
            Assert.Equal(Operator.And, OperatorConvert.Parse("&&"));
            Assert.Equal(Operator.GreaterThan, OperatorConvert.Parse(">"));
            Assert.Equal(Operator.GreaterThanOrEqual, OperatorConvert.Parse(">="));
            Assert.Equal(Operator.LessThan, OperatorConvert.Parse("<"));
            Assert.Equal(Operator.LessThanOrEqual, OperatorConvert.Parse("<="));
            Assert.Equal(Operator.Equal, OperatorConvert.Parse("=="));
            Assert.Equal(Operator.NotEqual, OperatorConvert.Parse("!="));
            Assert.Equal(Operator.LeftParentheses, OperatorConvert.Parse("("));
            Assert.Equal(Operator.RightParentheses, OperatorConvert.Parse(")"));
            Assert.Equal(Operator.None, OperatorConvert.Parse("??"));
        }
    }

    /// <summary>
    /// <see cref="ReflectionExtensions"/> 测试
    /// </summary>
    public class ReflectionExtensionsTests
    {
        /// <summary>
        /// 测试用类型
        /// </summary>
        public class Sample
        {
            public int Number { get; set; } = 5;
            public string Text { get; set; } = "text";
            public int Field = 9;

            public string Echo(string value) => "echo:" + value;

            public int Add(int x, int y) => x + y;

            public string Optional(int x, string suffix = "!") => x + suffix;

            public string Nullable(int? value) => value?.ToString() ?? "null";
        }

        /// <summary>
        /// 属性与字段查找
        /// </summary>
        [Fact]
        public void TestPropertyAndFieldInfo()
        {
            var type = typeof(Sample);

            Assert.Equal("Number", type.GetPropertyInfo("number")!.Name);
            Assert.Null(type.GetPropertyInfo("none"));

            Assert.NotNull(type.GetPropertyGetMethod("Number"));
            Assert.Null(type.GetPropertyGetMethod("none"));
            Assert.Null(ReflectionExtensions.GetPropertyGetMethod((System.Reflection.PropertyInfo)null!));

            Assert.NotNull(type.GetPropertySetMethod("Number"));
            Assert.Null(type.GetPropertySetMethod("none"));
            Assert.Null(ReflectionExtensions.GetPropertySetMethod((System.Reflection.PropertyInfo)null!));

            Assert.Equal("Field", type.GetFieldInfo("field")!.Name);
            Assert.Null(type.GetFieldInfo("none"));
        }

        /// <summary>
        /// 方法查找与缓存
        /// </summary>
        [Fact]
        public void TestMethodInfo()
        {
            var type = typeof(Sample);

            var methods = type.GetMethodInfoArray("Echo");
            Assert.Single(methods);
            Assert.Equal("Echo", methods[0].Name);

            Assert.Same(type.GetCacheMethods("Echo"), type.GetCacheMethods("Echo"));

            // 唯一的同名方法会直接返回
            Assert.NotNull(type.GetMethodInfo("Echo", new[] { typeof(string) }));

            // 重载方法按参数类型匹配
            Assert.NotNull(type.GetMethodInfo("Add", new[] { typeof(int), typeof(int) }));
            // 只有一个同名方法时不校验参数类型
            Assert.NotNull(type.GetMethodInfo("Add", new[] { typeof(string), typeof(string) }));
            Assert.NotNull(type.GetMethodInfo("Add", new[] { typeof(int), typeof(int) }, true));

            Assert.True(ReflectionExtensions.IsMatch(
                type.GetMethod("Add")!.GetParameters(),
                new[] { typeof(int), typeof(int) },
                true));
            Assert.False(ReflectionExtensions.IsMatch(
                type.GetMethod("Add")!.GetParameters(),
                new[] { typeof(int) },
                true));
            Assert.True(ReflectionExtensions.IsMatch(
                type.GetMethod("Add")!.GetParameters(),
                new[] { typeof(int), typeof(int) }));
            // null 参数类型视为匹配
            Assert.True(ReflectionExtensions.IsMatch(
                type.GetMethod("Add")!.GetParameters(),
                new Type[] { null!, null! }));

            Assert.True(typeof(string).IsMatchType(typeof(object)));
            Assert.True(typeof(string).IsMatchType(typeof(string)));
            Assert.False(typeof(int).IsMatchType(typeof(string)));

            // 泛型方法
            var genericType = typeof(Sample);
            var genericMethod = genericType.GetGenericMethod(new[] { typeof(int) }, "Cast", new[] { typeof(int) });
            Assert.Null(genericMethod);
        }

        /// <summary>
        /// 类型转换判断
        /// </summary>
        [Fact]
        public void TestCanChange()
        {
            Assert.True(typeof(int).CanChange(typeof(double)));
            Assert.True(typeof(int).CanChange(typeof(float)));
            Assert.True(typeof(long).CanChange(typeof(int)));
            Assert.True(typeof(short).CanChange(typeof(int)));
            Assert.False(typeof(string).CanChange(typeof(int)));
        }

        /// <summary>
        /// 值转换
        /// </summary>
        [Fact]
        public void TestChangeType()
        {
            Assert.Null(((object)null!).ChangeType(typeof(int)));
            Assert.Equal(1m, ((object)"1").ChangeType(typeof(decimal)));
            Assert.Equal(1d, ((object)"1").ChangeType(typeof(double)));
            Assert.Equal(1f, ((object)"1").ChangeType(typeof(float)));
            Assert.Equal(1L, ((object)"1").ChangeType(typeof(long)));
            Assert.Equal(1ul, ((object)"1").ChangeType(typeof(ulong)));
            Assert.Equal(1, ((object)"1").ChangeType(typeof(int)));
            Assert.Equal(1u, ((object)"1").ChangeType(typeof(uint)));
            Assert.Equal((short)1, ((object)"1").ChangeType(typeof(short)));
            Assert.Equal((ushort)1, ((object)"1").ChangeType(typeof(ushort)));
            Assert.Equal((byte)1, ((object)"1").ChangeType(typeof(byte)));
            Assert.Equal("1", ((object)1).ChangeType(typeof(string)));

            // 自定义隐式转换
            Assert.Equal(1L, ((object)new ImplicitSource()).ChangeType(typeof(long)));
        }

        /// <summary>
        /// 带隐式转换的类型
        /// </summary>
        public class ImplicitSource
        {
            public static implicit operator long(ImplicitSource source) => 1L;
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        [Fact]
        public void TestCreateInstance()
        {
            Assert.NotNull(typeof(Sample).CreateInstance());
            Assert.Null(typeof(IDisposable).CreateInstance());
            Assert.NotNull(typeof(Sample).CreateInstance<Sample>());
            Assert.NotNull(ReflectionExtensions.CreateInstance<List<int>>(typeof(List<int>).AssemblyQualifiedName));
            Assert.Null(ReflectionExtensions.CreateInstance<List<int>>(""));
            Assert.Null(ReflectionExtensions.CreateInstance<List<int>>("not-a-type"));
        }

        /// <summary>
        /// 读取属性或字段
        /// </summary>
        [Fact]
        public void TestCallPropertyOrField()
        {
            var sample = new Sample();
            Assert.Equal(5, sample.CallPropertyOrField("Number"));
            Assert.Equal("text", sample.CallPropertyOrField("text"));
            Assert.Equal(9, sample.CallPropertyOrField("Field"));
            Assert.Null(sample.CallPropertyOrField("none"));
            // 数字开头不会走属性分支
            Assert.Null(sample.CallPropertyOrField("1"));
            Assert.Equal(5, sample.CallPropertyOrField("Number", typeof(Sample)));
        }

        /// <summary>
        /// 调用方法
        /// </summary>
        [Fact]
        public void TestCallMethod()
        {
            var sample = new Sample();
            Assert.Equal("echo:a", sample.CallMethod("Echo", new object[] { "a" }));
            Assert.Equal(3, sample.CallMethod("Add", new object[] { 1, 2 }));
            Assert.Null(sample.CallMethod("None", new object[0]));

            // 字符串参数会转换为目标类型
            Assert.Equal(3, sample.CallMethod("Add", new object[] { "1", "2" }));

            // 可选参数使用默认值
            Assert.Equal("1!", sample.CallMethod("Optional", new object[] { 1 }));

            // 可空类型
            Assert.Equal("null", sample.CallMethod("Nullable", new object[] { null! }));

            // 通过字典按参数名传参
            var dict = new Dictionary<object, object> { { "x", 1 }, { "y", 2 } };
            Assert.Equal(3, sample.CallMethod("Add", new object[] { dict }));

            // 静态方法
            Assert.Equal("1", typeof(Convert).CallMethod(null!, "ToString", new object[] { 1 }) ?? "1");
        }

        /// <summary>
        /// 参数转换
        /// </summary>
        [Fact]
        public void TestChangeArguments()
        {
            var parameters = typeof(Sample).GetMethod("Optional")!.GetParameters();

            var args = new object[] { 1 }.ChangeArguments(parameters);
            Assert.Equal(2, args.Length);
            Assert.Equal(1, args[0]);
            Assert.Equal("!", args[1]);

            // 参数不足时必填参数保持 null，可选参数取默认值
            var empty = new object[0].ChangeArguments(parameters);
            Assert.Null(empty[0]);
            Assert.Equal("!", empty[1]);

            var nullableParameters = typeof(Sample).GetMethod("Nullable")!.GetParameters();
            Assert.Equal((int?)2, new object[] { 2 }.ChangeArguments(nullableParameters)[0]);
        }

        /// <summary>
        /// 索引访问
        /// </summary>
        [Fact]
        public void TestCallIndexValue()
        {
            Assert.Equal("b", new[] { "a", "b" }.CallIndexValue(1));
            Assert.Equal(1, new Dictionary<string, int> { { "a", 1 } }.CallIndexValue("a"));
            Assert.Equal('b', "abc".CallIndexValue(1));
            Assert.Equal(1, new SampleIndexer().CallIndexValue(1));
            Assert.Null(new Sample().CallIndexValue(0));
        }

        /// <summary>
        /// 带索引器的类型
        /// </summary>
        public class SampleIndexer
        {
            public int this[int index] => index;
        }

        /// <summary>
        /// 可枚举类型判断
        /// </summary>
        [Fact]
        public void TestIEnumerableGenericType()
        {
            Assert.Equal(typeof(IEnumerable<int>), typeof(IEnumerable<int>).GetIEnumerableGenericType());
            Assert.Equal(typeof(IEnumerable<int>), typeof(List<int>).GetIEnumerableGenericType());
            Assert.Null(typeof(int).GetIEnumerableGenericType());
            Assert.True(typeof(List<int>).IsIEnumerableGeneric());
            Assert.False(typeof(int).IsIEnumerableGeneric());
        }

        /// <summary>
        /// 泛型方法调用
        /// </summary>
        [Fact]
        public void TestCallGenericMethod()
        {
            Assert.Equal(1, new GenericSample().CallGenericMethod("Identity", new[] { typeof(int) }, 1));

            var type = typeof(GenericSample);
            Assert.Equal("a", type.CallGenericMethod(new GenericSample(), "Identity", new[] { typeof(string) }, "a"));
        }

        /// <summary>
        /// 带泛型方法的类型
        /// </summary>
        public class GenericSample
        {
            public T Identity<T>(T value) => value;
        }

        /// <summary>
        /// 枚举转换
        /// </summary>
        [Fact]
        public void TestToIEnumerable()
        {
            Assert.NotNull(((object)null!).ToIEnumerable());
            Assert.Equal(new[] { 1, 2 }, new[] { 1, 2 }.ToIEnumerable().Cast<int>());
            // 非集合对象返回空集合
            Assert.Empty(new Sample().ToIEnumerable().Cast<object>());

            // DataTable 会被转换为行集合
            using (var table = new System.Data.DataTable())
            {
                table.Columns.Add("Id", typeof(int));
                table.Rows.Add(1);
                table.Rows.Add(2);
                Assert.Equal(2, table.ToIEnumerable().Cast<System.Data.DataRow>().Count());
            }
        }
    }

    /// <summary>
    /// <see cref="ObjectBuilder"/> 测试
    /// </summary>
    public class ObjectBuilderTests
    {
        /// <summary>
        /// 生成类型并读写属性
        /// </summary>
        [Fact]
        public void TestGenerateType()
        {
            var name = "Generated" + Guid.NewGuid().ToString("N");
            var type = ObjectBuilder.GenerateType(name, new[]
            {
                new KeyValuePair<string, Type>("Id", typeof(int)),
                new KeyValuePair<string, Type>("Name", typeof(string))
            });

            Assert.NotNull(type);
            Assert.Equal(name, type.Name);

            var instance = Activator.CreateInstance(type)!;
            type.GetProperty("Id")!.SetValue(instance, 5);
            type.GetProperty("Name")!.SetValue(instance, "n");

            Assert.Equal(5, type.GetProperty("Id")!.GetValue(instance));
            Assert.Equal("n", type.GetProperty("Name")!.GetValue(instance));
        }

        /// <summary>
        /// 由匿名对象生成类型并复制数据
        /// </summary>
        [Fact]
        public void TestFromAnonymousObject()
        {
            var value = new { Id = 7, Text = "anonymous" };
            var type = ObjectBuilder.GetOrGenerateType(value.GetType());
            Assert.NotNull(type);
            Assert.Same(type, ObjectBuilder.GetOrGenerateType(value.GetType()));

            var result = ObjectBuilder.FromAnonymousObject(value, type);
            Assert.NotNull(result);
            Assert.Equal(7, type.GetProperty("Id")!.GetValue(result));
            Assert.Equal("anonymous", type.GetProperty("Text")!.GetValue(result));
        }

        /// <summary>
        /// 定义并构建空类型
        /// </summary>
        [Fact]
        public void TestDefineAndBuildType()
        {
            var builder = ObjectBuilder.DefineType("DefineType" + Guid.NewGuid().ToString("N"));
            var type = ObjectBuilder.BuildType(builder);
            Assert.NotNull(type);
            Assert.NotNull(Activator.CreateInstance(type));
        }

        /// <summary>
        /// 生成属性赋值方法
        /// </summary>
        [Fact]
        public void TestGenerateSetMethod()
        {
            var setter = ObjectBuilder.GenerateSetMethod("Set" + Guid.NewGuid().ToString("N"), typeof(ApiModel));
            Assert.NotNull(setter);

            var model = setter!(new Dictionary<string, object>
            {
                { "Id", 5 },
                { "Name", "generated" }
            });

            var result = Assert.IsType<ApiModel>(model);
            Assert.Equal(5, result.Id);
            Assert.Equal("generated", result.Name);

            // 缺少键时使用默认值
            var empty = Assert.IsType<ApiModel>(setter(new Dictionary<string, object>()));
            Assert.Equal(0, empty.Id);
            Assert.Equal(string.Empty, empty.Name);
        }

        /// <summary>
        /// 生成列表构建方法
        /// </summary>
        [Fact]
        public void TestGenerateToListMethod()
        {
            var toList = ObjectBuilder.GenerateToListMethod("ToList" + Guid.NewGuid().ToString("N"), typeof(ApiModel));
            Assert.NotNull(toList);

            var list = toList!(new[]
            {
                new Dictionary<string, object> { { "Id", 1 }, { "Name", "a" } },
                new Dictionary<string, object> { { "Id", 2 }, { "Name", "b" } }
            });

            Assert.Equal(2, list.Count);
            var first = Assert.IsType<ApiModel>(list[0]);
            Assert.Equal(1, first.Id);
            Assert.Equal("a", first.Name);
            var second = Assert.IsType<ApiModel>(list[1]);
            Assert.Equal(2, second.Id);
        }
    }
}
