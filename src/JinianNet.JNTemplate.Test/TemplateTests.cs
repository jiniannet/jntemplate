using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace JinianNet.JNTemplate.Test
{
    [TestClass]
    public class TemplateTests
    {

        /// <summary>
        /// 测试SET与字符串相加
        /// </summary>
        [TestMethod]
        public void TestSet()
        {
            var templateContent = "$set(n=868)\r\n${\"result:\"+n.ToString()}";
            var template = new Template(templateContent);

            var render = template.Render();
            
            Assert.AreEqual("\r\nresult:868", render);
        }


        /// <summary>
        /// 测试计算表达式
        /// </summary>
        [TestMethod]
        public void TestExpression()
        {
            var templateContent = "${8+2*5}";
            var template = new Template(templateContent);
            var render = template.Render();

            Assert.AreEqual("18", render);
        }

        /// <summary>
        /// 测试复杂的计算表达式
        /// </summary>
        [TestMethod]
        public void TestComplicatedExpression()
        {
            var templateContent = "${(8+2)*(5+5) - ((2+8)/2)}";
            var template = new Template(templateContent);
            var render = template.Render();
            Assert.AreEqual("95", render);
        }

        /// <summary>
        /// 测试逻辑表达式(逻辑运算符包括 ==,!=,<,>,>=,<=,||,&&)
        /// </summary>
        [TestMethod]
        public void TestLogicExpression()
        {
            var templateContent = "${4<=5}";
            var template = new Template(templateContent);
            var render = template.Render();

            Assert.AreEqual("True", render);
        }

        /// <summary>
        /// 测试逻辑或表达式 
        /// </summary>
        [TestMethod]
        public void TestLogicOrExpression()
        {
            var templateContent = "${4==5||5==5}";//如果是复杂的表达式，应优先考虑使用括号来表达优先级，比如 ${(4==5)||(5==5)} 比简单使用  ${4==5||5==5} 更好
            var template = new Template(templateContent);
            var render = template.Render();

            Assert.AreEqual("True", render);
        }

        /// <summary>
        /// 测试逻辑与表达式
        /// </summary>
        [TestMethod]
        public void TestLogicAndExpression()
        {
            var templateContent = "${4==5&&5==5}";
            var template = new Template(templateContent);
            var render = template.Render();

            Assert.AreEqual("False", render);
        }

        /// <summary>
        /// 测试IF
        /// </summary>
        [TestMethod]
        public void TestIf()
        {
            var templateContent = "${if(3==8)}3=8 success${elseif(3>8)}3>8 success${elseif(2<5)}2<5 success${else}null${end}";
            var template = new Template(templateContent);

            var render = template.Render();

            Assert.AreEqual("2<5 success", render);
        }

        /// <summary>
        /// 测试FOR
        /// </summary>
        [TestMethod]
        public void TestFor()
        {
            var templateContent = "$for(i=1;i<4;i=i+1)${i}$end";//"$for(i=1;i<4;i=i+1)${i}$end"
            var template = new Template(templateContent);
            var render = template.Render();

            Assert.AreEqual("123", render);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestFor1()
        {
            var templateContent = "$for(i=0;i<3;i++)${i}$end";//"$for(i=1;i<4;i=i+1)${i}$end"
            var template = new Template(templateContent);
            var render = template.Render();

            Assert.AreEqual("012", render);
        }

        /// <summary>
        /// 测试Forea 遍历对象时，Foreach应优先于for
        /// </summary>
        [TestMethod]
        public void TestForeach()
        {
            var templateContent = "$foreach(i in list)$i$end";
            var template = new Template(templateContent);
            template.Set("list", new int[] { 7, 0, 2, 0, 6 });
            var render = template.Render();
            Assert.AreEqual("70206", render);
        }

        /// <summary>
        /// 测试复合标签
        /// </summary>
        [TestMethod]
        public void TestReference()
        {
            var templateContent = "$date.Year.ToString().Length";
            var template = new Template(templateContent);
            template.Set("date", DateTime.Now);
            var render = template.Render();
            Assert.AreEqual("4", render);
        }

        /// <summary>
        /// 测试索引取值与方法标签
        /// </summary>
        [TestMethod]
        public void TestIndexValue()
        {
            var templateContent = "$data.get(0)"; //数组取值用get即可取到 List<Int32>用get_Item  原因见.NET的索引实现原理
            var template = new Template(templateContent);

            template.Set("data", new int[] { 7, 0, 2, 0, 6 });
            var render = template.Render();
            Assert.AreEqual("7", render);
        }

        /// <summary>
        /// 测试索引取值与方法标签
        /// </summary>
        [TestMethod]
        public void TestIndexValue1()
        {
            var templateContent = "$data.get_Item(\"name\")";
            var template = new Template(templateContent);
            var dic = new System.Collections.Generic.Dictionary<string, string>();
            dic["name"] = "你好！jntemplate";
            dic["age"] = "1";
            template.Set("data", dic);
            var render = template.Render();
            Assert.AreEqual("你好！jntemplate", render);
        }

        /// <summary>
        /// 测试索引取值与方法标签
        /// </summary>
        [TestMethod]
        public void TestIndexValue2()
        {
            var templateContent = "$data.name";//索引也可以和属性一样取值，不过推荐用get_Item，且如果索引是数字时，请尽量使用$data.get_Item(index)
            var template = new Template(templateContent);
            var dic = new System.Collections.Generic.Dictionary<string, string>();
            dic["name"] = "你好！jntemplate";
            dic["age"] = "1";
            template.Set("data", dic);
            var render = template.Render();
            Assert.AreEqual("你好！jntemplate", render);
        }

        /// <summary>
        /// 测试索引取值与方法标签
        /// </summary>
        [TestMethod]
        public void TestIndexValue3()
        {
            var templateContent = "$data.get_item(0)"; //数组取值用get即可取到 List<Int32>用get_Item  见.NET的索引实现原理
            var template = new Template(templateContent);

            template.Set("data", new System.Collections.Generic.List<int>(new int[] { 7, 0, 2, 0, 6 }));
            var render = template.Render();
            Assert.AreEqual("7", render);
        }

        [TestMethod]
        public void TestIf1()
        {
            var templateContent = "$if(CreteDate >= date.AddDays(-3))yes$end"; //数组取值用get即可取到 List<Int32>用get_Item  见.NET的索引实现原理
            var template = new Template(templateContent);
            template.Set("CreteDate",DateTime.Now);
            template.Set("date", DateTime.Now);
            var render = template.Render();
            Assert.AreEqual("yes", render);



        }

        
        [TestMethod]
        public void TestSet1()
        {
            var templateContent = "$set(hotItems= ArticleList(Category.CategoryEnglishName,10,1,30,2))$hotItems"; //数组取值用get即可取到 List<Int32>用get_Item  见.NET的索引实现原理
            var template = new Template(templateContent);
            template.Set("Category", new {
                CategoryEnglishName = "xx"
            });
            template.Set("date", DateTime.Now);
            template.Set("ArticleList",  new JinianNet.JNTemplate.FuncHandler(delegate(object[] args)
            {
                return string.Concat(args);
            }));
            var render = template.Render();
            Assert.AreEqual("xx101302", render);
            
        }

          [TestMethod]
        public void TestSet2()
        {
            var templateContent = "$set(table=\"HC_Article\")$set(list=db.Query(\"SELECT * FROM \"+table))$list";
            var template = new Template(templateContent);
            template.Set("db", new
            {
                Query = new JinianNet.JNTemplate.FuncHandler(delegate(object[] args)
                {
                    return string.Concat(args);
                })
            });
            var render = template.Render();
            Assert.AreEqual("SELECT * FROM HC_Article", render);

        }



          [TestMethod]
          public void test3()
          {
              var templateContent = "$if(dd==null) yes $else no $end";
              var template = new Template(templateContent);
              template.Set("dd",new System.Data.DataTable());

              var render = template.Render();
              Assert.AreEqual("0", render);

          }

          //[TestMethod]
          //public void TestExperience()
          //{
          //    var templateContent = "${(3+51}";
          //    var template = new Template(templateContent);
          //    var render = template.Render();
          //    Assert.AreEqual("SELECT * FROM HC_Article", render);

          //}



        ///// <summary>
        ///// 测试引擎
        ///// </summary>
        //[TestMethod]
        //public void TestPath()
        //{
        //    Resources.Paths.Add(@"F:\Work\JinianNet.JNTemplate1.2\src\JinianNet.JNTemplate.Test\templets");

        //    Engine e = new Engine();
        //    //Template t = (Template)e.CreateTemplate("default.txt");
        //    Template t = (Template)e.CreateTemplate("default.txt");
        //    var render = t.Render();
        //    Assert.AreEqual("这是头部文件", render);
        //}
    }


}