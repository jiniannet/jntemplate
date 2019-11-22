using Xunit;
using System;
using System.Collections.Generic;
using System.IO;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 模板标签功能性测试
    /// 及使用测试
    /// </summary>
    public partial class TagsTests : TagsTestBase
    {
        public TagsTests()
        {
            var conf = Configuration.EngineConfig.CreateDefault();
            //开始严格大小写模式 默认忽略大小写
            //conf.IgnoreCase = false;
            Engine.Configure(conf);
        }

        /// <summary>
        /// 综合测试
        /// </summary>
        [Fact]
        public void TestAll()
        {
            var templateContent = @"
$foreach(model in list)
    <div>list:${model.Id}</div>
    <ul>
    $if(model.Id == 2)

${set(aa = getList(model.id))}

            <d>${aa}<d>
        $foreach(m in aa)
            <li>$m.Text<li>
        $end
    $else
        <Not>list:${model.Id}</Not>
    $end
    </ul>
$end
";
            var template = Engine.CreateTemplate(templateContent);
            template.Context.TempData["list"] = new[] {
                new {
                    Id=1
                },
                new {
                    Id=2
                },
                new {
                    Id=3
                }
            };
            template.Context.TempData["getList"] = (new TemplateMethod());
            var render = Excute(template).Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "");
            Assert.Equal("<li><a href=\"/Help/art001.aspx\">下单后可以修改订单吗？</a></li><li><a href=\"/Help/art001.aspx\">无货商品几天可以到货？</a></li><li><a href=\"/Help/art001.aspx\">合约机资费如何计算？</a></li><li><a href=\"/Help/art001.aspx\">可以开发票吗？</a></li>".Replace(" ", ""), render);
        }



        /// <summary>
        /// 测试属性
        /// </summary>
        [Fact]
        public void TestOther()
        {
            var templateContent = @"$foreach(row in list)
                        <li><a href=""$func.GetHelpUrl(row)"">$row.Title</a></li>
                        $end";
            var template = Engine.CreateTemplate(templateContent);
            template.Context.TempData["list"] = (new TemplateMethod().GetHelpList(0, 0, 0, 0));
            template.Context.TempData["func"] = (new TemplateMethod());
            var render = Excute(template).Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "");
            Assert.Equal("<li><a href=\"/Help/art001.aspx\">下单后可以修改订单吗？</a></li><li><a href=\"/Help/art001.aspx\">无货商品几天可以到货？</a></li><li><a href=\"/Help/art001.aspx\">合约机资费如何计算？</a></li><li><a href=\"/Help/art001.aspx\">可以开发票吗？</a></li>".Replace(" ", ""), render);
        }

        /// <summary>
        /// 测试属性
        /// </summary>
        [Fact]
        public void TestProperty()
        {
            var templateContent = "$Site.Url";
            var template = Engine.CreateTemplate(templateContent);
            template.Context.TempData["Site"] = (new
            {
                Url = "jiniannet.com"
            });
            var render = Excute(template);
            Assert.Equal("jiniannet.com", render);
        }




        /// <summary>
        /// 测试SET与字符串相加
        /// </summary>
        [Fact]
        public void TestSet()
        {
            var templateContent = "$set(aGroupName = \"Begin\"+value)$aGroupName";
            var template = Engine.CreateTemplate(templateContent);
            template.Context.TempData["value"] = (30);
            var render = Excute(template);

            Assert.Equal("Begin30", render);
        }



        /// <summary>
        /// 测试复合标签
        /// </summary>
        [Fact]
        public void TestReference()
        {
            var templateContent = "$date.Year.ToString().Length";
            var template = Engine.CreateTemplate(templateContent);
            template.Context.TempData["date"] = (DateTime.Now);
            var render = Excute(template);
            Assert.Equal("4", render);
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
            template.Context.TempData["name"] = ("jntemplate");
            var render = Excute(template);
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
            var template = Engine.CreateTemplate(templateContent);
            template.Context.TempData["name"] = ("jntemplate");
            var render = Excute(template);
            Assert.Equal("你好,欢迎使用", render);
        }



#if NET20 || NET40
        /// <summary>
        /// 测试DataTable
        /// </summary>
        [Fact]
        public void TestTable1()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("name", typeof(string));
            var dr = dt.NewRow();
            dr["name"] = "Han Meimei";

            dt.Rows.Add(dr);


            var templateContent = "$dt.Rows.get_Item(0).get_Item(\"name\")";
            var template = Engine.CreateTemplate(templateContent);
            template.Context.TempData["dt"]=( dt);
            var render = Excute(template).Trim();
            Assert.Equal("Han Meimei", render);
        }

        /// <summary>
        /// 测试DataTable
        /// </summary>
        [Fact]
        public void TestTable2()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("name", typeof(string));
            var dr = dt.NewRow();
            dr["name"] = "Han Meimei";

            dt.Rows.Add(dr);


            var templateContent = @"
$foreach(dr in dt.Rows) 
    $dr.get_Item(""name"")
$end 
";
            var template = Engine.CreateTemplate(templateContent);
            template.Context.TempData["dt"]=( dt);
            var render = Excute(template).Trim();
            Assert.Equal("Han Meimei", render);
        }

        /// <summary>
        /// 测试DataTable
        /// </summary>
        [Fact]
        public void TestTable3()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("name", typeof(string));
            var dr = dt.NewRow();
            dr["name"] = "Han Meimei";

            dt.Rows.Add(dr);
            var templateContent = @" 
$foreach(dr in dt.Rows) 
    $foreach(data in dr.ItemArray)
        值:$data
    $end 
$end
";
            var template = Engine.CreateTemplate(templateContent);
            template.Context.TempData["dt"]=( dt);
            var render = Excute(template).Trim();
            Assert.Equal("值:Han Meimei", render);
        }
#endif


#if !NETCOREAPP2_0
        ///// <summary>
        ///// 测试方法的params参数  .NET core中不支持,从.V1.3.2以上版本不再支持
        ///// </summary>
        //[Fact]
        //public void TestFunctionParams()
        //{
        //    var templateContent = "$fun.TestParams(\"字符串\",1,true)";
        //    var template = Engine.CreateTemplate(templateContent);
        //    template.Context.TempData["fun"]=( new TemplateMethod());
        //    var render = Excute(template);
        //    Assert.Equal("您输入的参数是有：字符串 1 True ", render);
        //}

        ///// <summary>
        ///// 测试方法的params参数2 .NET core中不支持,从.V1.3.2以上版本不再支持
        ///// </summary>
        //[Fact]
        //public void TestFunctionParams2()
        //{
        //    var templateContent = "$fun.TestParams2(\"您输入的参数是有：\",\"字符串\",1,true)";
        //    var template = Engine.CreateTemplate(templateContent);
        //    template.Context.TempData["fun"]=( new TemplateMethod());
        //    var render = Excute(template);
        //    Assert.Equal("您输入的参数是有：字符串 1 True ", render);
        //}

#endif

        /// <summary>
        /// 测试变量
        /// </summary>
        [Fact]
        public void TestVariable()
        {
            var templateContent = "($a)人";
            var template = Engine.CreateTemplate(templateContent);
            template.Context.TempData["a"] = ("1");
            var render = Excute(template);

            Assert.Equal("(1)人", render);
        }

        /// <summary>
        /// 测试字符串转义
        /// </summary>
        [Fact]
        public void TestString()
        {
            var templateContent = "$set(str=\"3845254\\\\\\\"3366845\\\\\")$str";
            var template = Engine.CreateTemplate(templateContent);
            var render = Excute(template);
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
            var template = Engine.CreateTemplate(templateContent);
            template.Context.StripWhiteSpace = true;
            var render = Excute(template);
            Assert.Equal("your data is:5", render);
        }



        ///// <summary>
        ///// 测试标签大小写
        ///// </summary>
        //[Fact]
        //public void TestIgnoreCase()
        //{
        //    var templateContent  = "$date.Year";
        //    var template = Engine.CreateTemplate(templateContent);
        //    template.Context.TempData["date"]=(DateTime.Now);
        //    var render = Excute(template);
        //    Assert.Equal(DateTime.Now.Year.ToString(), render);
        //}
    }
}