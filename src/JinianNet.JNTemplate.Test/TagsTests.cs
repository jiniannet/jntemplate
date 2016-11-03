﻿
using System;
using Xunit;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 模板标签功能性测试
    /// </summary>
    //[TestClass]
    public class TagsTests
    {
        public TagsTests()
        {
            var conf = Configuration.EngineConfig.CreateDefault();
            //开始严格大小写模式 默认忽略大小写
            conf.IgnoreCase = false;
            Engine.Configure(conf);
        }

        public void Constructor()
        {

            string message = "参考用";
            var templateContent = "$message";
            var template = new Template(templateContent);
            template.Set("message", message);
            var render = template.Render();

            Assert.Equal("参考用", render);
        }

        /// <summary>
        /// 测试SET与字符串相加
        /// </summary>
        [Fact]
        public void TestSet()
        {
            var templateContent = "$set(aGroupName = \"Begin\"+value)$aGroupName";
            var template = new Template(templateContent);
            template.Set("value", 30);
            var render = template.Render();

            Assert.Equal("Begin30", render);
        }

        /// <summary>
        /// Dynamic定义
        /// </summary>
        public void TestDynamic()
        {
            var templateContent = "$ViewBag.test";
            dynamic viewBag = new JinianNet.JNTemplate.DynamicObject();
            viewBag.test = "来吧！一起狂欢吧！";
            var template = new Template(templateContent);
            template.Set("ViewBag", viewBag);

            var render = template.Render();
        }
        
        /// <summary>
        /// 测试计算表达式
        /// </summary>
        [Fact]
        public void TestExpression()
        {
            var templateContent = "${8+2*5}";
            var template = new Template(templateContent);
            var render = template.Render();

            Assert.Equal("18", render);
        }

        /// <summary>
        /// 测试复杂的计算表达式
        /// </summary>
        [Fact]
        public void TestComplicatedExpression()
        {
            var templateContent = "${(8+2)*(5+5) - ((2+8)/2)}";
            var template = new Template(templateContent);
            var render = template.Render();
            Assert.Equal("95", render);
        }

        /// <summary>
        /// 测试逻辑表达式(逻辑运算符包括 ==,!=,<,>,>=,<=,||,&&)
        /// </summary>
        [Fact]
        public void TestLogicExpression()
        {
            var templateContent = "${4<=5}";
            var template = new Template(templateContent);
            var render = template.Render();

            Assert.Equal("True", render);
        }

        /// <summary>
        /// 测试逻辑或表达式 
        /// </summary>
        [Fact]
        public void TestLogicOrExpression()
        {
            var templateContent = "${4==5||5==5}";//如果是复杂的表达式，应优先考虑使用括号来表达优先级，比如 ${(4==5)||(5==5)} 比简单使用  ${4==5||5==5} 更好
            var template = new Template(templateContent);
            var render = template.Render();

            Assert.Equal("True", render);
        }

        /// <summary>
        /// 测试逻辑与表达式
        /// </summary>
        [Fact]
        public void TestLogicAndExpression()
        {
            var templateContent = "${4==5&&5==5}";
            var template = new Template(templateContent);
            var render = template.Render();

            Assert.Equal("False", render);
        }

        /// <summary>
        /// 测试IF
        /// </summary>
        [Fact]
        public void TestIf()
        {
            var templateContent = "${if(3==8)}3=8 success${elseif(3>8)}3>8 success${elseif(2<5)}2<5 success${else}null${end}";
            var template = new Template(templateContent);

            var render = template.Render();

            Assert.Equal("2<5 success", render);
        }

        [Fact]
        public void TestIf1()
        {
            var templateContent = "$if(CreteDate >= date.AddDays(-3))yes$end"; //数组取值用get即可取到 List<Int32>用get_Item  见.NET的索引实现原理
            var template = new Template(templateContent);
            template.Set("CreteDate", DateTime.Now);
            template.Set("date", DateTime.Now);
            var render = template.Render();
            Assert.Equal("yes", render);
        }

        /// <summary>
        /// 判断对象是否为null
        /// </summary>
        [Fact]
        public void TestIf2()
        {
            var templateContent = "${if(dd)}yes${else}no$end";
            var template = new Template(templateContent);
            template.Set("dd", new System.Data.DataTable());

            var render = template.Render();
            Assert.Equal("yes", render);

        }

        [Fact]
        public void TestIf3()
        {
            var templateContent = "$if(3>2 && 5<2)yes${else}no${end}";
            var template = new Template(templateContent); ;
            var render = template.Render();
            Assert.Equal("no", render);
        }

        [Fact]
        public void TestIf4()
        {
            //v1 为空 false,5<2为false，整体结果 false || false 为false
            var templateContent = "$if(v1 || 5<2)yes${else}no${end}";
            var template = new Template(templateContent); ;
            var render = template.Render();
            Assert.Equal("no", render);
        }

        [Fact]
        public void TestIf5()
        {
            //v1 为空 false,v2等于9，数字不等于0即为true,整体结果 false || true 为true
            var templateContent = "$if(v1 || v2)yes${else}no${end}";
            var template = new Template(templateContent);
            template.Set("v2", 9);
            var render = template.Render();
            Assert.Equal("yes", render);
        }


        /// <summary>
        /// 测试FOR
        /// </summary>
        [Fact]
        public void TestFor()
        {
            var templateContent = "$for(i=1;i<4;i=i+1)${i}$end";//"$for(i=1;i<4;i=i+1)${i}$end"
            var template = new Template(templateContent);
            var render = template.Render();

            Assert.Equal("123", render);
        }

        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void TestFor1()
        {
            var templateContent = "$for(i=0;i<3;i++)${i}$end";//"$for(i=1;i<4;i=i+1)${i}$end"
            var template = new Template(templateContent);
            var render = template.Render();

            Assert.Equal("012", render);
        }

        /// <summary>
        /// 测试Forea 遍历对象时，Foreach应优先于for
        /// </summary>
        [Fact]
        public void TestForeach()
        {
            var templateContent = "$foreach(i in list)$i$end";
            var template = new Template(templateContent);
            template.Set("list", new int[] { 7, 0, 2, 0, 6 });
            var render = template.Render();
            Assert.Equal("70206", render);
        }

        /// <summary>
        /// 测试复合标签
        /// </summary>
        [Fact]
        public void TestReference()
        {
            var templateContent = "$date.Year.ToString().Length";
            var template = new Template(templateContent);
            template.Set("date", DateTime.Now);
            var render = template.Render();
            Assert.Equal("4", render);
        }

        /// <summary>
        /// 测试索引取值与方法标签
        /// </summary>
        [Fact]
        public void TestIndexValue()
        {
            var templateContent = "$data.Get(0)"; //数组取值用get即可取到 List<Int32>用get_Item  原因见.NET的索引实现原理
            var template = new Template(templateContent);

            template.Set("data", new int[] { 7, 0, 2, 0, 6 });
            var render = template.Render();
            Assert.Equal("7", render);
        }

        /// <summary>
        /// 测试索引取值与方法标签
        /// </summary>
        [Fact]
        public void TestIndexValue1()
        {
            var templateContent = "$data.get_Item(\"name\")";
            var template = new Template(templateContent);
            var dic = new System.Collections.Generic.Dictionary<string, string>();
            dic["name"] = "你好！jntemplate";
            dic["age"] = "1";
            template.Set("data", dic);
            var render = template.Render();
            Assert.Equal("你好！jntemplate", render);
        }

        /// <summary>
        /// 测试索引取值与方法标签
        /// </summary>
        [Fact]
        public void TestIndexValue2()
        {
            var templateContent = "$data.name";//索引也可以和属性一样取值，不过推荐用get_Item，且如果索引是数字时，请尽量使用$data.get_Item(index)
            var template = new Template(templateContent);
            var dic = new System.Collections.Generic.Dictionary<string, string>();
            dic["name"] = "你好！jntemplate";
            dic["age"] = "1";
            template.Set("data", dic);
            var render = template.Render();
            Assert.Equal("你好！jntemplate", render);
        }

        /// <summary>
        /// 测试索引取值与方法标签
        /// </summary>
        [Fact]
        public void TestIndexValue3()
        {
            var templateContent = "$data.get_Item(0)"; //数组取值用get即可取到 List<Int32>用get_Item  见.NET的索引实现原理
            var template = new Template(templateContent);

            template.Set("data", new System.Collections.Generic.List<int>(new int[] { 7, 0, 2, 0, 6 }));
            var render = template.Render();
            Assert.Equal("7", render);
        }

        /// <summary>
        /// 测试索引取值与方法标签
        /// </summary>
        [Fact]
        public void TestLoad()
        {
            var templateContent = "$load(\"include/header.txt\")";
            var template = new Template(templateContent);
            template.Set("name", "jntemplate");
            template.Context.CurrentPath = new System.IO.DirectoryInfo(System.Environment.CurrentDirectory).Parent.Parent.FullName + System.IO.Path.DirectorySeparatorChar.ToString() + "templets" + System.IO.Path.DirectorySeparatorChar.ToString() + "default";
            var render = template.Render();
            Assert.Equal("你好，jntemplate", render);
        }

        /// <summary>
        /// 测试索引取值与方法标签
        /// </summary>
        [Fact]
        public void TestInclub()
        {
            var templateContent = "$include(\"include/header.txt\")";
            var template = new Template(templateContent);
            template.Set("name", "jntemplate");
            template.Context.CurrentPath = new System.IO.DirectoryInfo(System.Environment.CurrentDirectory).Parent.Parent.FullName + System.IO.Path.DirectorySeparatorChar.ToString() + "templets"+ System.IO.Path.DirectorySeparatorChar.ToString() + "default";
            var render = template.Render();
            Assert.Equal("你好，$name", render);
        }

        /// <summary>
        /// 自定义标签前后缀测试
        /// </summary>
        [Fact]
        public void TestConfig()
        {
            var conf = Configuration.EngineConfig.CreateDefault();
            conf.TagFlag = '@';
            conf.TagSuffix = "}";
            conf.TagPrefix = "{$";

            Engine.Configure(conf);

            var templateContent = "你好，@name,欢迎来到{$name}的世界";
            var template = (Template)Engine.CreateTemplate(templateContent);
            template.Set("name", "jntemplate");
            var render = template.Render();
            Assert.Equal("你好，jntemplate,欢迎来到jntemplate的世界", render);

            Engine.Configure(Configuration.EngineConfig.CreateDefault());
            //Assert.Equal("111", "111");
        }

        /// <summary>
        /// 注释
        /// </summary>
        [Fact]
        public void TestComent()
        {
            var templateContent = "你好,$*使用简写符加星号可对代码注释*$欢迎使用";
            var template = new Template(templateContent);
            template.Set("name", "jntemplate");
            var render = template.Render();
            Assert.Equal("你好,欢迎使用", render);
        }


        /// <summary>
        /// 测试委托方法
        /// </summary>
        [Fact]
        public void TestDelegateFunction()
        {
            var templateContent = "$test(\"字符串\",1,true)";
            var template = new Template(templateContent);
            template.Set("test", new JinianNet.JNTemplate.FuncHandler(args =>
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

            var render = template.Render();

            Assert.Equal("您输入的参数是有：字符串 1 True ", render);
        }

        /// <summary>
        /// 测试类方法
        /// </summary>
        [Fact]
        public void TestClassFunction()
        {
            var templateContent = "$fun.Test(\"字符串\",1,true)";
            var template = new Template(templateContent);
            template.Set("fun", new TemplateMethod());
            var render = template.Render();
            Assert.Equal("您输入的参数是有：字符串 1 True ", render);
        }

        /// <summary>
        /// 测试方法的params参数
        /// </summary>
        [Fact]
        public void TestFunctionParams()
        {
            var templateContent = "$fun.TestParams(\"字符串\",1,true)";
            var template = new Template(templateContent);
            template.Set("fun", new TemplateMethod());
            var render = template.Render();
            Assert.Equal("您输入的参数是有：字符串 1 True ", render);
        }

        /// <summary>
        /// 测试方法的params参数2
        /// </summary>
        [Fact]
        public void TestFunctionParams2()
        {
            var templateContent = "$fun.TestParams2(\"您输入的参数是有：\",\"字符串\",1,true)";
            var template = new Template(templateContent);
            template.Set("fun", new TemplateMethod());
            var render = template.Render();
            Assert.Equal("您输入的参数是有：字符串 1 True ", render);
        }

        /// <summary>
        /// 测试变量
        /// </summary>
        [Fact]
        public void TestVariable()
        {
            var templateContent = "($a)人";
            var template = new Template(templateContent);
            template.Set("a", "1");
            var render = template.Render();

            Assert.Equal("(1)人", render);
        }

        /// <summary>
        /// 测试字符串转义
        /// </summary>
        [Fact]
        public void TestString()
        {
            var templateContent = "$set(str=\"3845254\\\\\\\"3366845\\\\\")$str";
            var template = new Template(templateContent);
            var render = template.Render();
            Assert.Equal("3845254\\\"3366845\\", render);

        }

        /// <summary>
        /// 测试标签前后空白字符串处理
        /// </summary>
        [Fact]
        public void TestStripWhiteSpace()
        {
            var templateContent = @"
your data is:
$set(key1=1)
$set(key2=2)
$set(key3=3)
$set(key4=4)
$set(key5=5)
$set(key6=6)
$key5";
            var template = new Template(templateContent);
            template.Context.StripWhiteSpace = true;
            var render = template.Render();
            Assert.Equal("your data is:5", render);
        }

        ///// <summary>
        ///// 测试标签大小写
        ///// </summary>
        //[Fact]
        //public void TestIgnoreCase()
        //{
        //    var templateContent  = "$date.Year";
        //    var template = new Template(templateContent);
        //    template.Set("date",DateTime.Now);
        //    var render = template.Render();
        //    Assert.Equal(DateTime.Now.Year.ToString(), render);
        //}
    }
}
