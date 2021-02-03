using Xunit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 模板标签功能性测试
    /// 及使用测试
    /// </summary>
    public partial class TagsTests : TagsTestBase
    {
        public static int Value { get; set; } = 8888;


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
        $foreach(m in getList(model.id))
            <li>$m</li>
        $end
    $end
    </ul>
$end
";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("list", new Entity[] {
                new Entity{
                    Id=1
                },
                new Entity {
                    Id=2
                },
                new  Entity{
                    Id=3
                }
            });
            template.Set<Func<int, string[]>>("getList", (id) =>
            {
                return new string[] { $"a{id}", $"b{id}", $"c{id}" };
            });
            var render = (template.Render()).Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "");
            Assert.Equal("<div>list:1</div><ul></ul><div>list:2</div><ul><li>a2</li><li>b2</li><li>c2</li></ul><div>list:3</div><ul></ul>", render);
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
            template.Set("list", new TemplateMethod().GetHelpList(0, 0, 0, 0));
            template.Set("func", new TemplateMethod());
            var render = (template.Render()).Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "");
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
            template.Set("Site", new Entity
            {
                Url = "jiniannet.com"
            });
            var render = template.Render();
            Assert.Equal("jiniannet.com", render);
        }

        /// <summary>
        /// 测试静态方法
        /// </summary>
        [Fact]
        public void TestStaticProperty()
        {
            var templateContent = "${TagsTests.Value}";
            var template = Engine.CreateTemplate(templateContent);
            template.SetStaticType("TagsTests", typeof(TagsTests));
            var render = template.Render();
            Assert.Equal("8888", render);
        }



        /// <summary>
        /// 测试枚举
        /// </summary>
        [Fact]
        public void TestEnum()
        {
            var templateContent = "$Plat";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("Plat", PlatformID.Win32NT);
            var render = template.Render();
            Assert.Equal("Win32NT", render);
        }

        /// <summary>
        /// 测试SET与字符串相加
        /// </summary>
        [Fact]
        public void TestSet()
        {
            var templateContent = "$set(aGroupName = \"Begin\"+value)$aGroupName";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("value", 30);
            var render = template.Render();

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
            template.Set("date", DateTime.Now);
            var render = template.Render();
            Assert.Equal("4", render);
        }

        /// <summary>
        /// 自定义标签前后缀测试
        /// </summary>
        [Fact]
        public void TestConfig()
        {
            //var conf = Configuration.EngineConfig.CreateDefault();
            //conf.TagFlag = '@';
            //conf.TagSuffix = "}";
            //conf.TagPrefix = "{$";

            //Engine.Configure(conf);

            //var templateContent = "你好，@name,欢迎来到{$name}的世界";
            //var template = Engine.CreateTemplate(templateContent);
            //template.Set("name","jntemplate");
            //var render = template.Render();
            //Assert.Equal("你好，jntemplate,欢迎来到jntemplate的世界", render);

            //Engine.Configure(Configuration.EngineConfig.CreateDefault());
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
            template.Set("name", "jntemplate");
            var render = template.Render();
            Assert.Equal("你好,欢迎使用", render);
        }

        /// <summary>
        /// 测试SET的作用域
        /// </summary>
        [Fact]
        public void TestForSetOnBody()
        {
            var templateContent = @"$foreach(l in LinksList(true))$set(i=0)$set(i=i+1)${i}$end";
            var template = Engine.CreateTemplate(templateContent);
            template.Set<Func<bool, string[]>>("LinksList", (x) => new string[] { "1", "2", "3", "4" });
            var render = template.Render();
            Assert.Equal("1111", render);
        }

        /// <summary>
        /// 测试SET的作用域
        /// </summary>
        [Fact]
        public void TestForSet()
        {
            var templateContent = @"$set(i=0)$foreach(l in LinksList(true))$set(i=i+1)${i}$end${i}";
            var template = Engine.CreateTemplate(templateContent);
            template.Set<Func<bool, string[]>>("LinksList", (x) => new string[] { "1", "2", "3", "4" });
            var render = template.Render();
            Assert.Equal("12344", render);
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
            dr["name","Han Meimei";

            dt.Rows.Add(dr);


            var templateContent = "$dt.Rows.get_Item(0).get_Item(\"name\")";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("dt"]=( dt);
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
            dr["name","Han Meimei";

            dt.Rows.Add(dr);


            var templateContent = @"
$foreach(dr in dt.Rows) 
    $dr.get_Item(""name"")
$end 
";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("dt"]=( dt);
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
            dr["name","Han Meimei";

            dt.Rows.Add(dr);
            var templateContent = @" 
$foreach(dr in dt.Rows) 
    $foreach(data in dr.ItemArray)
        值:$data
    $end 
$end
";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("dt"]=( dt);
            var render = Excute(template).Trim();
            Assert.Equal("值:Han Meimei", render);
        }
#endif


#if !NETCOREAPP
        ///// <summary>
        ///// 测试方法的params参数  .NET core中不支持,从.V1.3.2以上版本不再支持
        ///// </summary>
        //[Fact]
        //public void TestFunctionParams()
        //{
        //    var templateContent = "$fun.TestParams(\"字符串\",1,true)";
        //    var template = Engine.CreateTemplate(templateContent);
        //    template.Set("fun"]=( new TemplateMethod());
        //    var render = template.Render();
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
        //    template.Set("fun"]=( new TemplateMethod());
        //    var render = template.Render();
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
            var template = Engine.CreateTemplate(templateContent);
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
            var template = Engine.CreateTemplate(templateContent);
            template.Context.StripWhiteSpace = true;
            var render = template.Render();
            Assert.Equal("your data is:5", render);
        }


        /// <summary>
        /// 测试基本使用方法
        /// </summary>
        [Fact]
        public void TestBaseLoadTemplate()
        {
            var paths = new[] { 
#if NETCOREAPP
                new System.IO.DirectoryInfo(System.AppContext.BaseDirectory).Parent.Parent.Parent.FullName,
#else
                System.Environment.CurrentDirectory,
#endif
                "templets",
                "default",
                "include",
                "header.txt"
        };

            var fileName = string.Join(System.IO.Path.DirectorySeparatorChar.ToString(), paths);
            if (!System.IO.File.Exists(fileName))
            {
                throw new System.Exception($"{fileName} 不存在");
            }
            var template = Engine.LoadTemplate(fileName);
            template.Set("name", "jntemplate");
            var render = template.Render();
            Assert.Equal("你好，jntemplate", render);
        }

        ///// <summary>
        ///// 测试标签大小写
        ///// </summary>
        //[Fact]
        //public void TestIgnoreCase()
        //{
        //    var templateContent  = "$date.Year";
        //    var template = Engine.CreateTemplate(templateContent);
        //    template.Set("date"]=(DateTime.Now);
        //    var render = template.Render();
        //    Assert.Equal(DateTime.Now.Year.ToString(), render);
        //}
    }
}