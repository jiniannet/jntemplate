using JinianNet.JNTemplate.Exceptions;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 编译模式下的成员访问与数值计算测试
    /// </summary>
    public class CompileModeTests
    {
        /// <summary>
        /// 用于成员访问测试的模型：字段、静态字段、属性与字符串索引器
        /// </summary>
        public class MemberModel
        {
            public string FieldValue = "field";

            public static string StaticValue = "static";

            public string PropValue { get; set; } = "prop";

            public string this[string key] => "index:" + key;
        }

        private static string Render(string content, Action<ITemplate> init = null)
        {
            var engine = new EngineBuilder().UseCompileEngine().Build();
            var template = engine.CreateTemplate(Guid.NewGuid().ToString("N"), content);
            init?.Invoke(template);
            return template.Render();
        }

        private static string RenderInterpreted(string content, Action<ITemplate> init = null)
        {
            var engine = new EngineBuilder().UseInterpretationEngine().Build();
            var template = engine.CreateTemplate(Guid.NewGuid().ToString("N"), content);
            init?.Invoke(template);
            return template.Render();
        }

        /// <summary>
        /// 编译模式下访问字段、静态字段、属性与索引器
        /// </summary>
        [Fact]
        public void TestMemberAccess()
        {
            var model = new MemberModel();

            Assert.Equal("field", Render("${m.FieldValue}", t => t.Set("m", model)));
            Assert.Equal("static", Render("${m.StaticValue}", t => t.Set("m", model)));
            Assert.Equal("prop", Render("${m.PropValue}", t => t.Set("m", model)));
            // 属性与字段都不存在时退化为字符串索引器
            Assert.Equal("index:AnyKey", Render("${m.AnyKey}", t => t.Set("m", model)));
            // 仅存在字符串索引器的类型（例如字典）
            Assert.Equal("v", Render("${dict.name}", t => t.Set("dict", new Dictionary<string, string> { { "name", "v" } })));
        }

        /// <summary>
        /// 解释模式下访问字段、静态字段、属性与索引器
        /// </summary>
        [Fact]
        public void TestMemberAccessInInterpretedMode()
        {
            var model = new MemberModel();

            Assert.Equal("field", RenderInterpreted("${m.FieldValue}", t => t.Set("m", model)));
            Assert.Equal("static", RenderInterpreted("${m.StaticValue}", t => t.Set("m", model)));
            Assert.Equal("prop", RenderInterpreted("${m.PropValue}", t => t.Set("m", model)));
            Assert.Equal("index:AnyKey", RenderInterpreted("${m.AnyKey}", t => t.Set("m", model)));
        }

        /// <summary>
        /// 未定义的变量在编译阶段抛出异常
        /// </summary>
        [Fact]
        public void TestUndefinedVariable()
        {
            Assert.Throws<CompileException>(() => Render("${missing}"));
        }

        /// <summary>
        /// 不同数值类型的算术运算（类型提升）
        /// </summary>
        [Fact]
        public void TestNumericTypePromotion()
        {
            Assert.Equal("3.5", Render("${1+2.5}"));
            Assert.Equal("3.5", Render("${1+f}", t => t.Set("f", 2.5f)));
            Assert.Equal("3", Render("${1+l}", t => t.Set("l", 2L)));
            Assert.Equal("3", Render("${1+ul}", t => t.Set("ul", 2UL)));
            Assert.Equal("3", Render("${1+ui}", t => t.Set("ui", 2u)));
            Assert.Equal("3", Render("${1+s}", t => t.Set("s", (short)2)));
            Assert.Equal("3", Render("${1+us}", t => t.Set("us", (ushort)2)));
            Assert.Equal("3", Render("${1+b}", t => t.Set("b", (byte)2)));
            Assert.Equal("3", Render("${1+sb}", t => t.Set("sb", (sbyte)2)));
        }

        /// <summary>
        /// 比较运算与取余
        /// </summary>
        [Fact]
        public void TestComparisonOperators()
        {
            Assert.False(bool.Parse(Render("${1>=2}")));
            Assert.True(bool.Parse(Render("${1<=2}")));
            Assert.True(bool.Parse(Render("${1<2}")));
            Assert.False(bool.Parse(Render("${1>2}")));
            Assert.True(bool.Parse(Render("${1==1}")));
            Assert.True(bool.Parse(Render("${1!=2}")));
            Assert.Equal("3", Render("${7%4}"));
        }

        /// <summary>
        /// decimal 参与算术运算：编译模式需调用 decimal 的运算符重载
        /// </summary>
        [Fact]
        public void TestDecimalArithmetic()
        {
            Assert.Equal("3.5", Render("${d+1}", t => t.Set("d", 2.5m)));
            Assert.Equal("3.5", Render("${1+d}", t => t.Set("d", 2.5m)));
            Assert.Equal("5.0", Render("${d+d2}", t => { t.Set("d", 2.5m); t.Set("d2", 2.5m); }));
            Assert.Equal("1.5", Render("${d-1}", t => t.Set("d", 2.5m)));
            Assert.Equal("5.0", Render("${d*2}", t => t.Set("d", 2.5m)));
            Assert.Equal("1.25", Render("${d/2}", t => t.Set("d", 2.5m)));
            Assert.Equal("0.5", Render("${d%1}", t => t.Set("d", 2.5m)));
        }

        /// <summary>
        /// decimal 与其他数值类型混合运算时的类型提升
        /// </summary>
        [Fact]
        public void TestDecimalArithmeticWithOtherNumericTypes()
        {
            Assert.Equal("4.0", Render("${d+1.5}", t => t.Set("d", 2.5m)));
            Assert.Equal("3.0", Render("${d+f}", t => { t.Set("d", 2.5m); t.Set("f", 0.5f); }));
            Assert.Equal("4.5", Render("${d+s}", t => { t.Set("d", 2.5m); t.Set("s", (short)2); }));
            Assert.Equal("4.5", Render("${d+us}", t => { t.Set("d", 2.5m); t.Set("us", (ushort)2); }));
            Assert.Equal("4.5", Render("${d+b}", t => { t.Set("d", 2.5m); t.Set("b", (byte)2); }));
            Assert.Equal("4.5", Render("${d+sb}", t => { t.Set("d", 2.5m); t.Set("sb", (sbyte)2); }));
            Assert.Equal("4.5", Render("${d+l}", t => { t.Set("d", 2.5m); t.Set("l", 2L); }));
            Assert.Equal("4.5", Render("${d+ui}", t => { t.Set("d", 2.5m); t.Set("ui", 2u); }));
            Assert.Equal("4.5", Render("${d+ul}", t => { t.Set("d", 2.5m); t.Set("ul", 2UL); }));
        }

        /// <summary>
        /// decimal 比较运算
        /// </summary>
        [Fact]
        public void TestDecimalComparison()
        {
            Assert.True(bool.Parse(Render("${d>1}", t => t.Set("d", 2.5m))));
            Assert.False(bool.Parse(Render("${d<1}", t => t.Set("d", 2.5m))));
            Assert.True(bool.Parse(Render("${d>=2.5}", t => t.Set("d", 2.5m))));
            Assert.True(bool.Parse(Render("${d<=2.5}", t => t.Set("d", 2.5m))));
            Assert.True(bool.Parse(Render("${d==2.5}", t => t.Set("d", 2.5m))));
            Assert.True(bool.Parse(Render("${d!=1.5}", t => t.Set("d", 2.5m))));
        }

        /// <summary>
        /// if 语句：条件为非布尔类型、分支内容为非字符串类型
        /// </summary>
        [Fact]
        public void TestIfWithNonBooleanCondition()
        {
            // if 分支返回 int / double 时会转换为字符串
            Assert.Equal("5", Render("$if(1>0)${5}$end"));
            Assert.Equal("5.5", Render("$if(1>0)${5.5}$end"));
            // 条件为字符串时按空/非空判断
            Assert.Equal("yes", Render("$if(name)yes$else no$end", t => t.Set("name", "abc")));
            Assert.Equal(" no", Render("$if(name)yes$else no$end", t => t.Set("name", "")));
            // 条件为布尔类型
            Assert.Equal("yes", Render("$if(flag)yes$else no$end", t => t.Set("flag", true)));
            Assert.Equal(" no", Render("$if(flag)yes$else no$end", t => t.Set("flag", false)));
        }

        /// <summary>
        /// if 语句：条件为数值类型时按其值判断真假
        /// </summary>
        [Fact]
        public void TestIfWithNumericCondition()
        {
            Assert.Equal("yes", Render("$if(z)yes$else no$end", t => t.Set("z", 1)));
            Assert.Equal(" no", Render("$if(z)yes$else no$end", t => t.Set("z", 0)));
            Assert.Equal("yes", Render("$if(z)yes$else no$end", t => t.Set("z", 1L)));
            Assert.Equal(" no", Render("$if(z)yes$else no$end", t => t.Set("z", 0L)));
            Assert.Equal("yes", Render("$if(z)yes$else no$end", t => t.Set("z", (short)1)));
            Assert.Equal(" no", Render("$if(z)yes$else no$end", t => t.Set("z", (short)0)));
            Assert.Equal("yes", Render("$if(z)yes$else no$end", t => t.Set("z", 1u)));
            Assert.Equal(" no", Render("$if(z)yes$else no$end", t => t.Set("z", 0u)));
            Assert.Equal("yes", Render("$if(z)yes$else no$end", t => t.Set("z", 1d)));
            Assert.Equal(" no", Render("$if(z)yes$else no$end", t => t.Set("z", 0d)));
            Assert.Equal("yes", Render("$if(z)yes$else no$end", t => t.Set("z", 1f)));
            Assert.Equal(" no", Render("$if(z)yes$else no$end", t => t.Set("z", 0f)));
            Assert.Equal("yes", Render("$if(z)yes$else no$end", t => t.Set("z", 1m)));
            Assert.Equal(" no", Render("$if(z)yes$else no$end", t => t.Set("z", 0m)));
            Assert.Equal("yes", Render("$if(z)yes$else no$end", t => t.Set("z", (byte)1)));
            // 没有 ToBoolean 重载的类型按装箱后的 object 判断（非 null 即为真）
            Assert.Equal("yes", Render("$if(z)yes$else no$end", t => t.Set("z", DateTime.Now)));
        }

        /// <summary>
        /// 数值条件在编译模式与解释模式下结果一致
        /// </summary>
        [Fact]
        public void TestNumericConditionParity()
        {
            var values = new object[] { 1, 0, 1L, 0L, (short)1, (short)0, 1u, 0u, 1d, 0d, 1f, 0f, 1m, 0m, (byte)1 };

            foreach (var value in values)
            {
                var expected = RenderInterpreted("$if(z)yes$else no$end", t => t.Set("z", value));
                var actual = Render("$if(z)yes$else no$end", t => t.Set("z", value));
                Assert.Equal(expected, actual);
            }
        }

        /// <summary>
        /// decimal 与小整型（含 byte/sbyte）运算在两种模式下结果一致
        /// </summary>
        [Fact]
        public void TestNumericArithmeticParity()
        {
            var values = new (string Name, object Value, decimal Sum)[]
            {
                ("i", 2, 4.5m),
                ("l", 2L, 4.5m),
                ("s", (short)2, 4.5m),
                ("us", (ushort)2, 4.5m),
                ("b", (byte)2, 4.5m),
                ("sb", (sbyte)2, 4.5m),
                ("ui", 2u, 4.5m),
                ("ul", 2UL, 4.5m),
                ("f", 0.5f, 3.0m),
                ("d2", 1.5d, 4.0m)
            };

            foreach (var (name, value, sum) in values)
            {
                var expected = sum.ToString();
                void Set(ITemplate t)
                {
                    t.Set("m", 2.5m);
                    // 编译模式需要已知变量类型，故显式指定
                    JinianNet.JNTemplate.TemplateExtensions.Set(t, name, value, value.GetType());
                }

                // decimal 在左、在右两种顺序
                Assert.Equal(expected, Render("${m+" + name + "}", Set));
                Assert.Equal(expected, RenderInterpreted("${m+" + name + "}", Set));
                Assert.Equal(expected, Render("${" + name + "+m}", Set));
                Assert.Equal(expected, RenderInterpreted("${" + name + "+m}", Set));
            }
        }

        /// <summary>
        /// byte/sbyte 参与运算时按 Int32 提升（不再退化为字符串拼接）
        /// </summary>
        [Fact]
        public void TestByteArithmetic()
        {
            void SetByte(ITemplate t)
            {
                t.Set("b", (byte)2);
                t.Set("b2", (byte)2);
                t.Set("sb", (sbyte)2);
                t.Set("sb2", (sbyte)2);
                t.Set("i", 1);
            }

            foreach (var content in new[] { "${b+b2}", "${sb+sb2}", "${i+b}", "${i+sb}", "${b+i}", "${sb+i}" })
            {
                Assert.Equal(Render(content, SetByte), RenderInterpreted(content, SetByte));
            }

            Assert.Equal("4", RenderInterpreted("${b+b2}", SetByte));
            Assert.Equal("4", RenderInterpreted("${sb+sb2}", SetByte));
            Assert.Equal("3", RenderInterpreted("${i+b}", SetByte));
            Assert.Equal("3", RenderInterpreted("${i+sb}", SetByte));
        }
    }
}
