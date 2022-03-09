using JinianNet.JNTemplate;
using JinianNet.JNTemplate.Test.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 模拟测试 MOCK TEST
    /// </summary>
    public partial class TagsTests : TagsTestBase
    {

        /// <summary>
        /// 模拟生成一个真实页面
        /// </summary>
        [Fact]
        public void TestMockPage1()
        {
            var e = Configuration.EngineConfig.CreateDefault();
            e.ThrowExceptions = false;
            var engine = BuildEngine();
            engine.Configure(e);
            var template = engine.CreateTemplate("<li>1</li><li>$model.id</li><li>3</li>");
            template.Set("model",new object());
            var result = "<li>1</li><li></li><li>3</li>";
            var html = template.Render();;
            Assert.Equal(result, html);
        }


        /// <summary>
        /// 模拟生成一个真实页面
        /// </summary>
        [Fact]
        public void TestMockPage()
        {
            var path = string.Join(Path.DirectorySeparatorChar, new string[] { Environment.CurrentDirectory, "templets", "default", "questionlist.html" });
            var template = Engine.LoadTemplate("TestMockPage", path);
            template.Context.OutMode = OutMode.Auto;
            FillData(template);
            var result = template.Render();;
            //将生成的文件放到result目录下可以查看实际生成效果
            //File.WriteAllText(string.Join(Path.DirectorySeparatorChar, new string[] { Environment.CurrentDirectory, "templets", "default", $"result_{Guid.NewGuid()}.html" }), result);
            Assert.Equal(result.Replace("\r", "").Replace("\t", "").Replace("\n", "").Replace(" ", ""), RESULT);
        } 

        private void FillData(ITemplate t)
        {
            var site = new SiteInfo();
            site.Copyright = "&copy;2014 - 2015";
            site.Description = "";
            site.Host = "localhost";
            site.KeyWords = "";
            site.Logo = "";
            site.Name = "xxx";
            site.SiteDirectory = "";
            site.Theme = "Blue";
            site.ThemeDirectory = "theme";
            site.Title = "jntemplate测试页";
            site.Url = string.Concat("http://localhost");

            if (!string.IsNullOrEmpty(site.SiteDirectory) && site.SiteDirectory != "/")
            {
                site.Url += "/" + site.SiteDirectory;
            }
            site.ThemeUrl = string.Concat(site.Url, "/", site.ThemeDirectory, "/", site.Theme); 
            t.Set("Site", site);
            t.Set("func", new TemplateMethod()); 
        }


        const string RESULT = "<!DOCTYPEhtml><htmllang=\"zh-cn\"><head><title>jntemplate测试页</title><metacharset=\"utf-8\"><metahttp-equiv=\"X-UA-Compatible\"content=\"IE=edge\"/><metaname=\"viewport\"content=\"width=device-width,initial-scale=1\"/><metaname=\"description\"content=\"\"/><metaname=\"author\"content=\"\"/><linkhref=\"Library/bootstrap/css/bootstrap.min.css\"rel=\"stylesheet\"type=\"text/css\"/><linkhref=\"Library/bootstrap/css/bootstrap-theme.min.css\"rel=\"stylesheet\"type=\"text/css\"/><linkhref=\"css/layout.css\"rel=\"stylesheet\"type=\"text/css\"/></head><bodyrole=\"document\"><divclass=\"header\"><divclass=\"topbar\"><divclass=\"wrapper\"><ahref=\"/Login.aspx\">登录</a><ahref=\"/Register.aspx\">注册</a></div></div><divclass=\"head-hd\"><divclass=\"head-logo\"><imgsrc=\"http://www.jiniannet.com/Attachment/10017\"alt=\"\"/></div><divclass=\"head-banner\"><scripttype=\"text/javascript\">/*全部顶部广告*/varcpro_id=\"u1681269\";</script><scriptsrc=\"http://cpro.baidustatic.com/cpro/ui/c.js\"type=\"text/javascript\"></script></div><divclass=\"head-text\">.Net技术交流群(5089240)<br/><atarget=\"_blank\"href=\"http://bbs.jiniannet.com\">极念论坛</a><atarget=\"_blank\"href=\"http://www.jiniannet.com/Channel/jntemplate\"style=\"color:#f00\">JNTemplate手册</a><br/><atarget=\"_blank\"href=\"http://bbs.jiniannet.com/forum.php?mod=viewthread&tid=1\">ASP.NET模板引擎源码下载</a><br/></div></div><divclass=\"nav\"><ulclass=\"wrapper\"><liclass=\"active\"><ahref=\"/default.aspx\">首页</a></li><li><ahref=\"/Product/pro001.aspx\">产品1</a></li><li><ahref=\"/Product/pro001.aspx\">产品2</a></li><li><ahref=\"/Product/pro001.aspx\">产品3</a></li><li><ahref=\"/Product/pro001.aspx\">产品4</a></li><li><ahref=\"/Support.aspx\">服务支持</a></li></ul></div></div><divclass=\"wrapper\"><divclass=\"help-search\">快速帮助:<selectid=\"SearchProduct\"><optionvalue=\"201\">产品1</option><optionvalue=\"202\">产品2</option><optionvalue=\"203\">产品3</option><optionvalue=\"204\">产品4</option></select><inputtype=\"text\"value=\"\"id=\"SearchKey\"/><buttonclass=\"btnbtn-success\"id=\"btnSupportSearch\">搜索</button></div><divclass=\"help-sidebar\"><dl><dt><ahref=\"/Support.aspx?product=201\">产品1</a></dt><dd><ahref=\"/Category/001.aspx?product=201\">栏目1</a></dd><dd><ahref=\"/Category/002.aspx?product=201\">栏目2</a></dd><dd><ahref=\"/Category/003.aspx?product=201\">栏目3</a></dd><dd><ahref=\"/Category/004.aspx?product=201\">栏目4</a></dd><dd><ahref=\"/Category/005.aspx?product=201\">栏目5</a></dd><dd><ahref=\"/Category/006.aspx?product=201\">栏目6</a></dd></dl><dl><dt><ahref=\"/Support.aspx?product=202\">产品2</a></dt><dd><ahref=\"/Category/001.aspx?product=202\">栏目1</a></dd><dd><ahref=\"/Category/002.aspx?product=202\">栏目2</a></dd><dd><ahref=\"/Category/003.aspx?product=202\">栏目3</a></dd><dd><ahref=\"/Category/004.aspx?product=202\">栏目4</a></dd><dd><ahref=\"/Category/005.aspx?product=202\">栏目5</a></dd><dd><ahref=\"/Category/006.aspx?product=202\">栏目6</a></dd></dl><dl><dt><ahref=\"/Support.aspx?product=203\">产品3</a></dt><dd><ahref=\"/Category/001.aspx?product=203\">栏目1</a></dd><dd><ahref=\"/Category/002.aspx?product=203\">栏目2</a></dd><dd><ahref=\"/Category/003.aspx?product=203\">栏目3</a></dd><dd><ahref=\"/Category/004.aspx?product=203\">栏目4</a></dd><dd><ahref=\"/Category/005.aspx?product=203\">栏目5</a></dd><dd><ahref=\"/Category/006.aspx?product=203\">栏目6</a></dd></dl><dl><dt><ahref=\"/Support.aspx?product=204\">产品4</a></dt><dd><ahref=\"/Category/001.aspx?product=204\">栏目1</a></dd><dd><ahref=\"/Category/002.aspx?product=204\">栏目2</a></dd><dd><ahref=\"/Category/003.aspx?product=204\">栏目3</a></dd><dd><ahref=\"/Category/004.aspx?product=204\">栏目4</a></dd><dd><ahref=\"/Category/005.aspx?product=204\">栏目5</a></dd><dd><ahref=\"/Category/006.aspx?product=204\">栏目6</a></dd></dl></div><divclass=\"help-hd\"><divclass=\"hd-mod\"><h1>视频网站付费会员增长超700%苦熬7年再度掀付费潮</h1><dl><dt>功能模块：</dt><dd><ahref='4'>测试模块</a></dd><dd><ahref='4'>订单模块</a></dd><dd><ahref='4'>产品模块</a></dd><dd><ahref='4'>新闻模块</a></dd></dl></div><divclass=\"hd-tbl\"><ulclass=\"hd-tbl-head\"id=\"tblHead\"><li><ahref=\"?tag=1\">热门问题</a></li><li><ahref=\"?tag=2\">最新推荐</a></li><li><ahref=\"?tag=3\">最新提问</a></li></ul><divclass=\"help-items\"><ul><li><ahref=\"/Help/art001.aspx\">下单后可以修改订单吗？</a></li><li><ahref=\"/Help/art001.aspx\">无货商品几天可以到货？</a></li><li><ahref=\"/Help/art001.aspx\">合约机资费如何计算？</a></li><li><ahref=\"/Help/art001.aspx\">可以开发票吗？</a></li></ul></div></div><divclass=\"aspnet-pager\"><span>首页</span><span>上一页</span><span>1</span><span>下一页</span><span>末页</span></div></div><divclass=\"clearfix\"></div></div><divclass=\"help-modwrapper\"><dl><dt>新手帮助</dt><dd><ahref=\"/Article/art001.aspx\">购物流程</a></dd><dd><ahref=\"/Article/art001.aspx\">会员介绍</a></dd><dd><ahref=\"/Article/art001.aspx\">生活旅行/团购</a></dd><dd><ahref=\"/Article/art001.aspx\">常见问题</a></dd><dd><ahref=\"/Article/art001.aspx\">联系客服</a></dd></dl><dl><dt>极念网络</dt><dd><ahref=\"/Article/art001.aspx\">购物流程</a></dd><dd><ahref=\"/Article/art001.aspx\">会员介绍</a></dd><dd><ahref=\"/Article/art001.aspx\">生活旅行/团购</a></dd><dd><ahref=\"/Article/art001.aspx\">常见问题</a></dd><dd><ahref=\"/Article/art001.aspx\">联系客服</a></dd></dl><dl><dt>免费应用</dt><dd><ahref=\"/Article/art001.aspx\">购物流程</a></dd><dd><ahref=\"/Article/art001.aspx\">会员介绍</a></dd><dd><ahref=\"/Article/art001.aspx\">生活旅行/团购</a></dd><dd><ahref=\"/Article/art001.aspx\">常见问题</a></dd><dd><ahref=\"/Article/art001.aspx\">联系客服</a></dd></dl><dl><dt>其他服务</dt><dd><ahref=\"/Article/art001.aspx\">购物流程</a></dd><dd><ahref=\"/Article/art001.aspx\">会员介绍</a></dd><dd><ahref=\"/Article/art001.aspx\">生活旅行/团购</a></dd><dd><ahref=\"/Article/art001.aspx\">常见问题</a></dd><dd><ahref=\"/Article/art001.aspx\">联系客服</a></dd></dl><dl><dt>关于我们</dt><dd><ahref=\"/Article/art001.aspx\">购物流程</a></dd><dd><ahref=\"/Article/art001.aspx\">会员介绍</a></dd><dd><ahref=\"/Article/art001.aspx\">生活旅行/团购</a></dd><dd><ahref=\"/Article/art001.aspx\">常见问题</a></dd><dd><ahref=\"/Article/art001.aspx\">联系客服</a></dd></dl></div><divclass=\"footer\"><pclass=\"text-muted\">极念网络版权所有©2015</p></div></body></html>";
    }
}
