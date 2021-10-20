using JinianNet.JNTemplate;
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Resources;
using JinianNet.JNTemplate.Test.ViewModel;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Xunit;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 更多高极用法测试
    /// </summary>
    public partial class TagsTests : TagsTestBase
    {
        /// <summary>
        /// 测试自定义标签
        /// </summary>
        [Fact]
        public void YestUserTag()
        {
            //这是一个简单的自定义标签
            if (Engine.Mode == EngineMode.Compiled)
            {
                Engine.Register<TestTag>((p, tc) =>
                {
                    if (tc.Count == 2 && tc.First.Text == ":")
                    {
                        return new TestTag
                        {
                            Document = tc[1].Text
                        };
                    }
                    return null;
                },
                    (tag, c) =>
                    {
                        var t = tag as TestTag;
                        var mb = c.CreateReutrnMethod<TestTag>(typeof(string));
                        var il = mb.GetILGenerator();
                        il.Emit(OpCodes.Ldstr, "say " + t.Document);
                        il.Emit(OpCodes.Ret);
                        return mb.GetBaseDefinition();
                    },
                    (tag, c) => typeof(string));
            }
            else
            {
                Engine.Current.RegisterParseFunc((p, tc) =>
                {
                    if (tc.Count == 2 && tc.First.Text == ":")
                    {
                        return new TestTag
                        {
                            Document = tc[1].Text
                        };
                    }
                    return null;
                });
                Engine.Current.RegisterExecuteFunc<TestTag>((tag, c) =>
                {
                    return $"say {(tag as TestTag).Document}";
                });
            }

            var templateContent = "${:hello}";
            var template = Engine.CreateTemplate(templateContent);
            var render = template.Render();

            Assert.Equal("say hello", render);
        }

        /// <summary>
        /// 测试加载器
        /// </summary>
        [Fact]
        public void TestLoader()
        {
            var engine = new EngineBuilder()
                .Build();

            engine.UseLoader(new TestLoader());
            var template = engine.LoadTemplate("11111");
            template.Set("name", "jntemplate");
            var render = template.Render();

            Assert.Equal($"当前是模板：11111 hello,jntemplate", render);
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
			<a href='${goodsModel.id}'>
				<img src=""${goodsModel.img_url}"" alt="""">
				<p class=""goodsName goodsText"">${goodsModel.title}</p>
				$if(goodsModel.fields[""act_txt""]!=null)
				<p class=""goodsDescrib goodsText"">${goodsModel.fields[""act_txt""]}</p>
				${end}
				<p class=""price goodsText"">￥${goodsModel.fields[""sell_price""]}</p>
			</a>
		</div>
		${else}
			<div class=""goods almostGoodsBox"">
			    <a href='${goodsModel.id}'>
					<img src=""${goodsModel.img_url}"" alt="""">
					<p class=""goodsName goodsText"">${goodsModel.title}</p>
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
            var template = Engine.CreateTemplate(templateContent);
            template.Set<Func<List<Goods>>>("GetGoodList", () =>
            {
                var list = new List<Goods>();
                list.Add(new Goods()
                {
                    id = 1,
                    img_url = "pic.png",
                    title = "商品名称一",
                    Fields = new Dictionary<string, object>() {
                         { "sell_price","200" },
                         { "act_txt","衣" },
                     }
                });
                list.Add(new Goods()
                {
                    id = 2,
                    img_url = "pic.png",
                    title = "商品名称二",
                    Fields = new Dictionary<string, object>() {
                         { "sell_price","120" },
                         { "act_txt","外" },
                     }
                });
                list.Add(new Goods()
                {
                    id = 3,
                    img_url = "pic.png",
                    title = "商品名称三",
                    Fields = new Dictionary<string, object>() {
                         { "sell_price","15.80" },
                         { "act_txt","中" },
                     }
                });
                return list;
            });
            var render = template.Render();
            var result = @"<divclass=""goods1almostGoodsBox""><ahref='1'><imgsrc=""pic.png""alt=""""><pclass=""goodsNamegoodsText"">商品名称一</p><pclass=""goodsDescribgoodsText"">衣</p><pclass=""pricegoodsText"">￥200</p></a></div><divclass=""goodsalmostGoodsBox""><ahref='2'><imgsrc=""pic.png""alt=""""><pclass=""goodsNamegoodsText"">商品名称二</p><pclass=""goodsDescribgoodsText"">外</p><pclass=""pricegoodsText"">￥120</p></a></div><divclass=""goodsalmostGoodsBox""><ahref='3'><imgsrc=""pic.png""alt=""""><pclass=""goodsNamegoodsText"">商品名称三</p><pclass=""goodsDescribgoodsText"">中</p><pclass=""pricegoodsText"">￥15.80</p></a></div>";
            Assert.Equal(result, render.Replace("\r", "").Replace("\t", "").Replace("\n", "").Replace(" ", ""));
        }
    }
}
