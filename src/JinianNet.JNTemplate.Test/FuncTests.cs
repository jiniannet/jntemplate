using JinianNet.JNTemplate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

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
            var template = Engine.CreateTemplate(templateContent);
            template.Set<Func<string, int, bool, string>>("test", (a, b, c) =>
            {
                return $"您输入的参数：{a}，{b}，{c}";
            });

            var render = template.Render();

            Assert.Equal("您输入的参数：字符串，1，True", render);
        }

        /// <summary>
        /// 测试类方法
        /// </summary>
        [Fact]
        public void TestClassFunction()
        {
            var templateContent = "$fun.Test(\"字符串\",1,true)";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("fun", new TemplateMethod());
            var render = template.Render();
            Assert.Equal("您输入的参数是有：字符串 1 True ", render);
        }


        ///// <summary>
        ///// 测试可选参数(实参以JSON格式支持)
        ///// </summary>
        //[Fact]
        //public void TestJson()
        //{
        //    //JSON标签在本次版本中仅仅是方便处理函数参数使用，不支持复杂格式
        //    //如果需要支持复杂JSON，可自行引用第三方JSON类库并以扩展标签的方式处理
        //    var templateContent = "${helper.Test({\"message\":\"你好\",\"id\":UserId})}";
        //    var template = Engine.CreateTemplate(templateContent);
        //    template.Set("UserId", 110);
        //    template.Set("helper", new TemplateMethod());
        //    var render = template.Render();
        //    //false是布尔类型的默认值
        //    Assert.Equal("您输入的参数是有：你好 110 False ", render);
        //}



        /// <summary>
        /// 测试实参带负数的函数
        /// </summary>
        [Fact]
        public void TestNegativeFunc()
        {

            var templateContent = "$test(8,-2)";
            var template = Engine.CreateTemplate(templateContent);
            template.Set<Func<int, int, string>>("test", (x, y) =>
            {
                var r = x + y;
                return r.ToString();
            });
            var render = template.Render();

            Assert.Equal("6", render);
        }


        /// <summary>
        /// 测试方法参数
        /// </summary>
        [Fact]
        public void TestFunctionParamer()
        {
            var templateContent = "${ArticleList(15,\"<div class=\\\"col-md-6 col-sm-12 col-12\\\"><div class=\\\"notice-k\\\"><div class=\\\"notice-h4\\\"><a href=\\\"/default/show/[ID]\\\">[F_Title]</a></div><p>[F_Summery]</p><div class=\\\"notice-bottom\\\"><div class=\\\"notice-bottom-left\\\">[F_AddTime]</div><div class=\\\"notice-bottom-right\\\">[F_Author]</div></div></div></div>\",4,20,60)}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set<Func<int, string, int, int, int, string>>("ArticleList", (id, html, page, rows, classId) =>
            {
                return id.ToString();
            });
            var render = template.Render();
            Assert.Equal("15", render);
        }

        /// <summary>
        /// 测试方法参数
        /// </summary>
        [Fact]
        public void TestFunctionGrammar()
        {
            //var templateContent = "${ArticleList(Class)}";
            var templateContent = "${ArticleList(Nav.Class.Sort,10,false,20,60,true,false)}";
            var template = Engine.CreateTemplate(templateContent);
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
            var render = template.Render();
            Assert.Equal("1", render);
        }


        /// <summary>
        /// 测试Func委托(1.4.0以上版本支持)
        /// </summary>
        [Fact]
        public void TestFunctionFunc()
        {
            var templateContent = "${test(\"test data\")}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set<Func<string, string>>("test", (text) =>
            {
                return "input:" + text;
            });
            var render = template.Render();
            Assert.Equal("input:test data", render);
        }

        /// <summary>
        /// 测试Func委托(1.4.0以上版本支持)
        /// </summary>
        [Fact]
        public void TestCoxFunc()
        {
            var templateContent = "${test(\"test data\",9527)}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set<Func<string, int, string>>("test", (text, id) =>
            {
                return "input:" + text + " id:" + id;
            });
            var render = template.Render();
            Assert.Equal("input:test data id:9527", render);
        }

        /// <summary>
        /// 测试Action委托(1.4.0以上版本支持)
        /// </summary>
        [Fact]
        public void TestFunctionAction()
        {
            var templateContent = "${action(\"test data\")}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set<Action<string>>("action", (text) =>
            {
                Console.WriteLine("你输入了:" + text);
            });
            var render = template.Render();
            Assert.Equal("", render);

        }

        /// <summary>
        /// 测试静态方法
        /// </summary>
        [Fact]
        public void TestStaticConcat()
        {
            var templateContent = "${string.Concat(\"str1\",\"str2\")}";
            var template = Engine.CreateTemplate(templateContent);
            template.SetStaticType("string", typeof(string));
            var render = template.Render();
            Assert.Equal("str1str2", render);
        }


        /// <summary>
        /// 测试属性方法
        /// </summary>
        [Fact]
        public void TestPropertyFunc()
        {
            var templateContent = "${model.PropertyFunc(100)}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("model", new FuncInfo()
            {
                PropertyFunc = (i) => (i * 2).ToString()
            });
            var render = template.Render();
            Assert.Equal("200", render);
        }


        /// <summary>
        /// 测试字段方法
        /// </summary>
        [Fact]
        public void TestFieldFunc()
        {
            var templateContent = "${model.FieldFunc(100)}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("model", new FuncInfo()
            {
                FieldFunc = (i) => (i * 2).ToString()
            });
            var render = template.Render();
            Assert.Equal("200", render);
        }


        /// <summary>
        /// 测试方法与为数学
        /// </summary>
        [Fact]
        public void TestFuncAndArithmetic()
        {
            var templateContent = "${calc(84+2,53)}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set<Func<int, int, int>>("calc", (x, y) => x - y);
            var render = template.Render();
            Assert.Equal("33", render);
        }
    }
}
