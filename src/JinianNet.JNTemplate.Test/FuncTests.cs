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
        public async Task TestDelegateFunction()
        {
            var templateContent = "$test(\"字符串\",1,true)";
            var template = Engine.CreateTemplate(templateContent);
            template.Context.TempData["test"] = (new JinianNet.JNTemplate.FuncHandler(args =>
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("您输入的参数是有：");
                foreach (var node in args)
                {
                    sb.Append(node);
                    sb.Append(" ");
                }
                return sb.ToString();
            }));

            var render = await Excute(template);

            Assert.Equal("您输入的参数是有：字符串 1 True ", render);
        }

        /// <summary>
        /// 测试类方法
        /// </summary>
        [Fact]
        public async Task TestClassFunction()
        {
            var templateContent = "$fun.Test(\"字符串\",1,true)";
            var template = Engine.CreateTemplate(templateContent);
            template.Context.TempData["fun"] = (new TemplateMethod());
            var render = await Excute(template);
            Assert.Equal("您输入的参数是有：字符串 1 True ", render);
        }


        /// <summary>
        /// 测试可选参数(实参以JSON格式支持)
        /// </summary>
        [Fact]
        public async Task TestJson()
        {
            //JSON标签在本次版本中仅仅是方便处理函数参数使用，不支持复杂格式
            //如果需要支持复杂JSON，可自行引用第三方JSON类库并以扩展标签的方式处理
            var templateContent = "${helper.Test({\"message\":\"你好\",\"id\":UserId})}";
            var template = Engine.CreateTemplate(templateContent);
            template.Context.TempData["UserId"] = (110);
            template.Context.TempData["helper"] = (new TemplateMethod());
            var render = await Excute(template);
            //false是布尔类型的默认值
            Assert.Equal("您输入的参数是有：你好 110 False ", render);
        }



        /// <summary>
        /// 测试实参带负数的函数
        /// </summary>
        [Fact]
        public async Task TestNegativeFunc()
        {

            var templateContent = "$test(8,-2)";
            var template = Engine.CreateTemplate(templateContent);
            template.Context.TempData["test"] = (new JinianNet.JNTemplate.FuncHandler(args =>
            {
                var r = int.Parse(args[0].ToString()) + int.Parse(args[1].ToString());
                return r.ToString();
            }));
            var render = await Excute(template);

            Assert.Equal("6", render);
        }


        /// <summary>
        /// 测试方法参数
        /// </summary>
        [Fact]
        public async Task TestFunctionParamer()
        {
            var templateContent = "${ArticleList(15,\"<div class=\\\"col-md-6 col-sm-12 col-12\\\"><div class=\\\"notice-k\\\"><div class=\\\"notice-h4\\\"><a href=\\\"/default/show/[ID]\\\">[F_Title]</a></div><p>[F_Summery]</p><div class=\\\"notice-bottom\\\"><div class=\\\"notice-bottom-left\\\">[F_AddTime]</div><div class=\\\"notice-bottom-right\\\">[F_Author]</div></div></div></div>\",4,20,60)}";
            var template = Engine.CreateTemplate(templateContent);
            template.Context.TempData["ArticleList"] = (new JinianNet.JNTemplate.FuncHandler(args =>
            {
                return args[0];
            }));
            var render = await Excute(template);
            Assert.Equal("15", render);
        }

    }
}
