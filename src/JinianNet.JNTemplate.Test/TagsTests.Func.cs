using JinianNet.JNTemplate.Test.Model;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 测试函数方法
    /// </summary>
    public partial class TagsTests : TagsTestBase
    {
        /// <summary>
        /// 测试委托方法
        /// </summary>
        [Fact]
        public void TestDelegateFunction()
        {
            var templateContent = "$test(\"字符串\",1,true)";
            TestCreateRunDoubleEngine(templateContent,
                "您输入的参数：字符串，1，True",
                template => template.Set<Func<string, int, bool, string>>("test", (a, b, c) =>
                {
                    return $"您输入的参数：{a}，{b}，{c}";
                }));
        }

        /// <summary>
        /// 测试类方法
        /// </summary>
        [Fact]
        public void TestClassFunction()
        {
            var templateContent = "$fun.Test(\"字符串\",1,true)";
            TestCreateRunDoubleEngine(templateContent,
                "您输入的参数是有：字符串 1 True ",
                template => template.Set("fun", new TemplateMethod()));
        }

        /// <summary>
        /// 测试INT与INT?类型的自动转换
        /// </summary>
        [Fact]
        public void TestParamNullable()
        {
            var templateContent = "$fun.TestNullable(70206)";
            TestCreateRunDoubleEngine(templateContent,
                "input:70206",
                template => template.Set("fun", new TemplateMethod()));
        }

        /// <summary>
        /// 测试实参带负数的函数
        /// </summary>
        [Fact]
        public void TestNegativeFunc()
        {

            var templateContent = "$test(8,-2)";
            TestCreateRunDoubleEngine(templateContent,
                "6",
                template => template.Set<Func<int, int, string>>("test", (x, y) =>
                {
                    var r = x + y;
                    return r.ToString();
                }));
        }


        /// <summary>
        /// 测试方法参数
        /// </summary>
        [Fact]
        public void TestFunctionParamer()
        {
            var templateContent = "${ArticleList(15,\"<div class=\\\"col-md-6 col-sm-12 col-12\\\"><div class=\\\"notice-k\\\"><div class=\\\"notice-h4\\\"><a href=\\\"/default/show/[ID]\\\">[F_Title]</a></div><p>[F_Summery]</p><div class=\\\"notice-bottom\\\"><div class=\\\"notice-bottom-left\\\">[F_AddTime]</div><div class=\\\"notice-bottom-right\\\">[F_Author]</div></div></div></div>\",4,20,60)}";
            TestCreateRunDoubleEngine(templateContent,
                "15",
                template => template.Set<Func<int, string, int, int, int, string>>("ArticleList", (id, html, page, rows, classId) =>
                {
                    return id.ToString();
                }));
        }

        /// <summary>
        /// 测试方法参数
        /// </summary>
        [Fact]
        public void TestFunctionGrammar()
        {
            var templateContent = "${ArticleList(Nav.Class.Sort,10,false,20,60,true,false)}";
            TestCreateRunDoubleEngine(templateContent,
                "1",
                template =>
                {
                    template.Set<Func<int, int, bool, int, int, bool, bool, int>>("ArticleList", (a, b, c, d, e, f, g) =>
                    {
                        return a;
                    });
                    template.Set("Nav", new Nav
                    {
                        Class = new Nav.ClassValue
                        {
                            Sort = 1
                        }
                    });
                });
        }


        /// <summary>
        /// 测试Func委托(1.4.0以上版本支持)
        /// </summary>
        [Fact]
        public void TestFunctionFunc()
        {
            var templateContent = "${test(\"test data\")}";
            TestCreateRunDoubleEngine(templateContent,
                "input:test data",
                template =>
                {
                    template.Set<Func<string, string>>("test", (text) =>
                    {
                        return "input:" + text;
                    });
                });
        }

        /// <summary>
        /// 测试Func委托(1.4.0以上版本支持)
        /// </summary>
        [Fact]
        public void TestCoxFunc()
        {
            var templateContent = "${test(\"test data\",9527)}";
            TestCreateRunDoubleEngine(templateContent,
                "input:test data id:9527",
                template =>
                {
                    template.Set<Func<string, int, string>>("test", (text, id) =>
                    {
                        return "input:" + text + " id:" + id;
                    });
                });
        }

        /// <summary>
        /// 测试Action委托(1.4.0以上版本支持)
        /// </summary>
        [Fact]
        public void TestFunctionAction()
        {
            var templateContent = "${action(\"test data\")}";

            TestCreateRunDoubleEngine(templateContent,
                "",
                template =>
                {
                    template.Set<Action<string>>("action", (text) =>
                    {
                        Console.WriteLine("input:" + text);//无返回
                    });
                });

        }

        /// <summary>
        /// 测试静态方法
        /// </summary>
        [Fact]
        public void TestStaticConcat()
        {
            var templateContent = "${string.Concat(\"str1\",\"str2\")}";

            TestCreateRunDoubleEngine(templateContent,
                "str1str2",
                template => template.SetStaticType("string", typeof(string)));
        }


        /// <summary>
        /// 测试属性方法
        /// </summary>
        [Fact]
        public void TestPropertyFunc()
        {
            var templateContent = "${model.PropertyFunc(100)}";
            TestCreateRunDoubleEngine(templateContent,
                "200",
                template => template.Set("model", new FuncInfo()
                {
                    PropertyFunc = (i) => (i * 2).ToString()
                }));
        }


        /// <summary>
        /// 测试字段方法
        /// </summary>
        [Fact]
        public void TestFieldFunc()
        {
            var templateContent = "${model.FieldFunc(100)}";
            TestCreateRunDoubleEngine(templateContent,
                "200",
                template => template.Set("model", new FuncInfo()
                {
                    FieldFunc = (i) => (i * 2).ToString()
                }));
        }


        /// <summary>
        /// 测试方法与为数学
        /// </summary>
        [Fact]
        public void TestFuncAndArithmetic()
        {
            var templateContent = "${calc(84+2,53)}";
            TestCreateRunDoubleEngine(templateContent,
                "33",
                template => template.Set<Func<int, int, int>>("calc", (x, y) => x - y));
        }

        /// <summary>
        /// 类型转换 字符串转OBJECT，引用类型之间转换， 
        /// </summary>
        [Fact]
        public void TestFuncStringToObject()
        {
            var templateContent = "${toValue(title)}";
            TestCreateRunDoubleEngine(templateContent,
                "jntemplate!",
                template =>
                {
                    template.Set<string>("title", "jntemplate");
                    template.Set<Func<object, string>>("toValue", o => o?.ToString() + "!");
                });
        }

        /// <summary>
        /// 类型转换 OBJECT转字符串，引用类型之间转换， 
        /// </summary>
        [Fact]
        public void TestFuncObjectTString()
        {
            var templateContent = "${toValue(obj)}";
            TestCreateRunDoubleEngine(templateContent,
                "System.Object",
                template =>
                {
                    template.Set("obj", new Object());
                    template.Set<Func<string, string>>("toValue", o => o);
                }); 
        }


        /// <summary>
        /// 类型转换 INT转OBJ，装箱， 
        /// </summary>
        [Fact]
        public void TestFuncIntToObject()
        {
            var templateContent = "${toValue(70)}";
            TestCreateRunDoubleEngine(templateContent,
                "70",
                template =>
                {
                    template.Set<Func<object, int>>("toValue", (x) => (int)x);
                }); 
        }



        /// <summary>
        /// 类型转换 Bool转OBJ，装箱， 
        /// </summary>
        [Fact]
        public void TestFuncBoolToObject()
        {
            var templateContent = "${toValue(true)}";
            TestCreateRunDoubleEngine(templateContent,
                "True",
                template =>
                {
                    template.Set<Func<object, bool>>("toValue", (x) => (bool)x);
                }); 
        }


        /// <summary>
        /// 类型转换 INT转DEC，值类型之间转换
        /// </summary>
        [Fact]
        public void TestFuncIntToDecimal()
        {
            var templateContent = "${toValue(86)}"; 
            TestCreateRunDoubleEngine(templateContent,
                "86.00",
                template =>
                {
                    template.Set<Func<decimal, string>>("toValue", (x) => x.ToString("#0.00"));
                }); 
        }


        /// <summary>
        /// 类型转换  Decimal转Int，值类型之间转换
        /// </summary>
        [Fact]
        public void TestFuncDecimalToInt()
        {
            var templateContent = "${toValue(var)}";
            var value = 86.13M;
            TestCreateRunDoubleEngine(templateContent,
                "86",
                template =>
                {
                    template.Set<Decimal>("var", value);
                    template.Set<Func<int, string>>("toValue", (x) => x.ToString());
                });
        }
    }
}
