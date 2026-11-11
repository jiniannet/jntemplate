using JinianNet.JNTemplate.Test.Model;
using JinianNet.JNTemplate.Test.ViewModel;
using static System.Net.Mime.MediaTypeNames;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 模板标签功能性测试
    /// 及使用测试
    /// </summary>
    public partial class TagsTests : TagsTestBase
    {
        public static int Value = 70206;
        /// <summary>
        /// 测试属性
        /// </summary>
        [Fact]
        public void TestForeachAndFunc()
        {
            var templateContent = @"$foreach(row in list)
                        <li><a href=""$func.GetHelpUrl(row)"">$row.Title</a></li>
                        $end";
            var template = Engine.CreateTemplate(templateContent);
            template.Set("list", new TemplateMethod().GetHelpList(0, 0, 0, 0));
            template.Set("func", new TemplateMethod());
            var html = template.Render();
            var render = html.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "");
            Assert.Equal("<li><a href=\"/Help/art001.aspx\">下单后可以修改订单吗？</a></li><li><a href=\"/Help/art001.aspx\">无货商品几天可以到货？</a></li><li><a href=\"/Help/art001.aspx\">合约机资费如何计算？</a></li><li><a href=\"/Help/art001.aspx\">可以开发票吗？</a></li>".Replace(" ", ""), render);
        }

        /// <summary>
        /// 测试属性
        /// </summary>
        [Fact]
        public void TestProperty()
        {
            var templateContent = "$Site.Url";
            var text = "jiniannet.com";
            TestCreateRunDoubleEngine(templateContent,
               text,
               template =>
               {
                   template.Set("Site", new Entity
                   {
                       Url = "jiniannet.com"
                   });
               });
        }

        /// <summary>
        /// 测试枚举属性
        /// </summary>
        [Fact]
        public void TestEnumProperty()
        {
            var templateContent = "$model.Type";
            var text = "Yes";
            TestCreateRunDoubleEngine(templateContent,
               text,
               template =>
               {
                   template.Set("model", new DemoEntity
                   {
                       Id = 100,
                       Title = "jiniannet.com",
                       Type = DemoType.Yes
                   });
               });
        }

        /// <summary>
        /// 测试静态方法
        /// </summary>
        [Fact]
        public void TestStaticProperty()
        {
            var templateContent = "${TagsTests.Value}";
            var text = TagsTests.Value.ToString();
            TestCreateRunDoubleEngine(templateContent,
               text,
               template =>
               {
                   template.SetStaticType("TagsTests", typeof(TagsTests));
               });
        }



        /// <summary>
        /// 测试枚举
        /// </summary>
        [Fact]
        public void TestEnum()
        {
            var templateContent = "$Plat";
            var text = "Win32NT";
            TestCreateRunDoubleEngine(templateContent,
               text,
               template =>
               {
                   template.Set("Plat", PlatformID.Win32NT);
               });
        }

        /// <summary>
        /// 测试SET与字符串相加
        /// </summary>
        [Fact]
        public void TestSet()
        {
            var templateContent = "$set(aGroupName = \"Begin\"+value)$aGroupName";
            var text = "Begin30";
            TestCreateRunDoubleEngine(templateContent,
               text,
               template =>
               {
                   template.Set("value", 30);
               });
        }



        /// <summary>
        /// 测试复合标签
        /// </summary>
        [Fact]
        public void TestReference()
        {
            var templateContent = "$date.Year.ToString()";
            var text = DateTime.Now.Year.ToString();
            TestCreateRunDoubleEngine(templateContent,
               text,
               template =>
               {
                   template.Set("date", DateTime.Now);
               });
        }

        /// <summary>
        /// 注释
        /// </summary>
        [Fact]
        public void TestComent()
        {
            var templateContent = "你好,$*使用简写符加星号可对代码注释*$欢迎使用";
            var text = "你好,欢迎使用";
            TestCreateRunDoubleEngine(templateContent,
               text);
        }

        /// <summary>
        /// 测试SET的作用域
        /// </summary>
        [Fact]
        public void TestForSetOnBody()
        {
            var templateContent = @"$foreach(l in LinksList(true))$set(i=0)$set(i=i+1)${i}$end";
            var text = "1111";
            TestCreateRunDoubleEngine(templateContent,
               text,
               template =>
               {
                   template.Set<Func<bool, string[]>>("LinksList", (x) => new string[] { "1", "2", "3", "4" });
               });
        }

        /// <summary>
        /// 测试SET的作用域
        /// </summary>
        [Fact]
        public void TestForSet()
        {
            var templateContent = @"$set(i=0)$foreach(l in LinksList(true))$set(i=i+1)${i}$end${i}";
            var text = "12344";
            TestCreateRunDoubleEngine(templateContent,
               text,
               template =>
               {
                   template.Set<Func<bool, string[]>>("LinksList", (x) => new string[] { "1", "2", "3", "4" });
               });
        }


        /// <summary>
        /// 测试DataTable
        /// </summary>
        [Fact]
        public void TestDataTableIntIndex()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("name", typeof(string));
            var dr = dt.NewRow();
            dr["name"] = "Han Meimei";

            dt.Rows.Add(dr);


            var templateContent = "$dt.Rows.get_Item(0).get_Item(\"name\")";
            var text = "Han Meimei";
            TestCreateRunDoubleEngine(templateContent,
               text,
               template => template.Set("dt", dt));
        }

        /// <summary>
        /// 测试DataTable
        /// </summary>
        [Fact]
        public void TestDataTableStringIndex()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("name", typeof(string));
            var dr = dt.NewRow();
            dr["name"] = "Han Meimei";

            dt.Rows.Add(dr);


            var templateContent = @"${foreach(dr in dt.Rows)}${dr.get_Item(""name"")}${end}";
            var text = "Han Meimei";
            TestCreateRunDoubleEngine(templateContent,
               text,
               template => template.Set("dt", dt));
        }

        /// <summary>
        /// 测试DataTable
        /// </summary>
        [Fact]
        public void TestDataTableItemArray()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("name", typeof(string));
            var dr = dt.NewRow();
            dr["name"] = "Han Meimei";

            dt.Rows.Add(dr);
            var templateContent = @"${foreach(dr in dt.Rows)}${foreach(data in dr.ItemArray)}值:${data}${end}${end}";
            var text = "值:Han Meimei";
            TestCreateRunDoubleEngine(templateContent,
               text,
               template => template.Set("dt", dt));
        }

        /// <summary>
        /// 测试变量
        /// </summary>
        [Fact]
        public void TestVariable()
        {
            var templateContent = "($a)人";
            var text = "(1)人";
            TestCreateRunDoubleEngine(templateContent,
               text,
               template => template.Set("a", "1"));
        }

        /// <summary>
        /// 测试字符串转义
        /// </summary>
        [Fact]
        public void TestString()
        {
            var templateContent = "$set(str=\"3845254\\\\\\\"3366845\\\\\")$str";

            var text = "3845254\\\"3366845\\";
            TestCreateRunDoubleEngine(templateContent,
               text);

        }

        /// <summary>
        /// 测试字符串2（可以当前端转义使用）
        /// </summary>
        [Fact]
        public void TestEncodeString()
        {
            var templateContent = "var ${\"$\"}a =34;";
            var text = "var $a =34;";
            TestCreateRunDoubleEngine(templateContent,
               text);
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
            var text = "你好，jntemplate";
            TestLoadRunDoubleEngine(fileName,
               text,
               template => template.Set("name", "jntemplate"));
        }


        /// <summary>
        /// 测试数字标签
        /// </summary>
        [Fact]
        public void TestIntOutput()
        {
            var templateContent = "${12}";
            var text = "12";
            TestCreateRunDoubleEngine(templateContent,
               text);

        }

        /// <summary>
        /// 测试字典
        /// </summary>
        [Fact]
        public void TestListOfTOutput()
        {
            var templateContent = "$data";
            var text = "System.Collections.Generic.List`1";
            TestCreateRunDoubleEngine(templateContent,
                null,
                template => template.Set("data", new List<object>()),
                result => Assert.StartsWith(text, result));
        }

        /// <summary>
        /// 测试Anonymous object
        /// </summary>
        [Fact]
        public void TestAnonymousObject()
        {
            var templateContent = "${model.Id}";
            var text = "48";
            TestCreateRunDoubleEngine(templateContent,
               text,
               template => template.SetAnonymousObject("model", new
               {
                   Name = "user",
                   Id = 48
               }));
        }


        /// <summary>
        /// 测试数组标签
        /// </summary>
        [Fact]
        public void TestArrayTag()
        {
            var templateContent = @"${set(arr={1,2,3,4})}len:$arr.Length type:$arr";
            TestCreateRunDoubleEngine(templateContent,
               "len:4 type:System.Object[]");
        }



        /// <summary>
        /// 测试数组标签
        /// </summary>
        [Fact]
        public void TestMixArrayTag()
        {
            var templateContent = @"$set(arr={1,2,""jntemplate"",4,true})$arr[2].ToString()";
            TestCreateRunDoubleEngine(templateContent,
               "jntemplate");
        }


        /// <summary>
        /// 测试数组标签
        /// </summary>
        [Fact]
        public void TestArrayForTag()
        {
            var templateContent = @"
$set(arr={7,0,2,0,6})
$for(i in arr)
$i
$end";
            TestCreateRunDoubleEngine(templateContent, null, null,
              render => Assert.Equal("70206", render.Replace("\r", "").Replace("\n", "").TrimEnd()));
        }


        /// <summary>
        /// 测试换行符的识别
        /// </summary>
        [Fact]
        public void TestNewline()
        {
            var templateContent = @"replace(/\n/g,<br>);";
            TestCreateRunDoubleEngine(templateContent,
               templateContent);
        }



        [Fact]
        public void TestForAndSetOr()
        {
            var templateContent = @"
$set(goodsList=GetGoodList())
${if(goodsList.Count>0)}
	${set(i=0)}
	${foreach(goodsModel in goodsList)}
		${if(i==0)}
		<div class=""goods1 almostGoodsBox"">
			<a href='${goodsModel.Id}'>
				<img src=""${goodsModel.ImgUrl}"" alt="""">
				<p class=""goodsName goodsText"">${goodsModel.Title}</p>
				$if(goodsModel.fields[""act_txt""]!=null)
				<p class=""goodsDescrib goodsText"">${goodsModel.fields[""act_txt""]}</p>
				${end}
				<p class=""price goodsText"">￥${goodsModel.fields[""sell_price""]}</p>
			</a>
		</div>
		${else}
			<div class=""goods almostGoodsBox"">
			    <a href='${goodsModel.Id}'>
					<img src=""${goodsModel.ImgUrl}"" alt="""">
					<p class=""goodsName goodsText"">${goodsModel.Title}</p>
					$if(goodsModel.fields[""act_txt""]!=null)
					<p class=""goodsDescrib goodsText"">${goodsModel.fields[""act_txt""]}</p>
					${end}
					<p class=""price goodsText"">￥${goodsModel.fields[""sell_price""]}</p>
				</a>
			</div>
		${end}
		${i++}
	${end}
${end}";

            var result = @"<divclass=""goods1almostGoodsBox""><ahref='1'><imgsrc=""pic.png""alt=""""><pclass=""goodsNamegoodsText"">商品名称一</p><pclass=""goodsDescribgoodsText"">衣</p><pclass=""pricegoodsText"">￥200</p></a></div><divclass=""goodsalmostGoodsBox""><ahref='2'><imgsrc=""pic.png""alt=""""><pclass=""goodsNamegoodsText"">商品名称二</p><pclass=""goodsDescribgoodsText"">外</p><pclass=""pricegoodsText"">￥120</p></a></div><divclass=""goodsalmostGoodsBox""><ahref='3'><imgsrc=""pic.png""alt=""""><pclass=""goodsNamegoodsText"">商品名称三</p><pclass=""goodsDescribgoodsText"">中</p><pclass=""pricegoodsText"">￥15.80</p></a></div>";
            TestCreateRunDoubleEngine(templateContent,
                null,
                template => template.Set<Func<List<Goods>>>("GetGoodList", () =>
                {
                    var list = new List<Goods>();
                    list.Add(new Goods()
                    {
                        Id = 1,
                        ImgUrl = "pic.png",
                        Title = "商品名称一",
                        Fields = new Dictionary<string, object>() {
                         { "sell_price","200" },
                         { "act_txt","衣" },
                     }
                    });
                    list.Add(new Goods()
                    {
                        Id = 2,
                        ImgUrl = "pic.png",
                        Title = "商品名称二",
                        Fields = new Dictionary<string, object>() {
                         { "sell_price","120" },
                         { "act_txt","外" },
                     }
                    });
                    list.Add(new Goods()
                    {
                        Id = 3,
                        ImgUrl = "pic.png",
                        Title = "商品名称三",
                        Fields = new Dictionary<string, object>() {
                         { "sell_price","15.80" },
                         { "act_txt","中" },
                     }
                    });
                    return list;
                }),
                render => Assert.Equal(result, render.Replace("\r", "").Replace("\t", "").Replace("\n", "").Replace(" ", "")));
        }




        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void TestNotThrowExceptions()
        {
            var templateContent = "<li>1</li><li>$model.id</li><li>3</li>";
            var text = "<li>1</li><li></li><li>3</li>";

            var config = Configuration.EngineConfig.CreateDefault();
            config.ThrowExceptions = false;

            TestCreateRunDoubleEngine(templateContent,
                text,
                engine => engine.Configure(config),
                template => template.Set("model", new object()));
        }
    }
}