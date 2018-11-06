//#define VER_EQ_TEST
//#define NVELOCITY_TEST
//#define BASE_PAGE_TEST
//#define WIN  
#if WIN
using System.IO;
using System.Reflection;
using System;
using JinianNet.JNTemplate.Test.Model;
using System.Diagnostics;
using Xunit;

namespace JinianNet.JNTemplate.Test
{
  
    /// <summary>
    /// 实际WEB页面模板测试
    /// </summary>
    public class WebPageTests
    {
        const string HTML_RESULT = "<!DOCTYPEhtml><htmllang=\"zh-cn\"><head><title>jntemplate测试页</title><metacharset=\"utf-8\"><metahttp-equiv=\"X-UA-Compatible\"content=\"IE=edge\"/><metaname=\"viewport\"content=\"width=device-width,initial-scale=1\"/><metaname=\"description\"content=\"\"/><metaname=\"author\"content=\"\"/><linkhref=\"Library/bootstrap/css/bootstrap.min.css\"rel=\"stylesheet\"type=\"text/css\"/><linkhref=\"Library/bootstrap/css/bootstrap-theme.min.css\"rel=\"stylesheet\"type=\"text/css\"/><linkhref=\"css/layout.css\"rel=\"stylesheet\"type=\"text/css\"/></head><bodyrole=\"document\"><divclass=\"header\"><divclass=\"topbar\"><divclass=\"wrapper\"><ahref=\"/Login.aspx\">登录</a><ahref=\"/Register.aspx\">注册</a></div></div><divclass=\"head-hd\"><divclass=\"head-logo\"><imgsrc=\"http://www.jiniannet.com/Attachment/10017\"alt=\"\"/></div><divclass=\"head-banner\"><scripttype=\"text/javascript\">/*全部顶部广告*/varcpro_id=\"u1681269\";</script><scriptsrc=\"http://cpro.baidustatic.com/cpro/ui/c.js\"type=\"text/javascript\"></script></div><divclass=\"head-text\">.Net技术交流群(5089240)<br/><atarget=\"_blank\"href=\"http://bbs.jiniannet.com\">极念论坛</a><atarget=\"_blank\"href=\"http://www.jiniannet.com/Channel/jntemplate\"style=\"color:#f00\">JNTemplate手册</a><br/><atarget=\"_blank\"href=\"http://bbs.jiniannet.com/forum.php?mod=viewthread&tid=1\">ASP.NET模板引擎源码下载</a><br/></div></div><divclass=\"nav\"><ulclass=\"wrapper\"><liclass=\"active\"><ahref=\"/default.aspx\">首页</a></li><li><ahref=\"/Product/pro001.aspx\">产品1</a></li><li><ahref=\"/Product/pro001.aspx\">产品2</a></li><li><ahref=\"/Product/pro001.aspx\">产品3</a></li><li><ahref=\"/Product/pro001.aspx\">产品4</a></li><li><ahref=\"/Support.aspx\">服务支持</a></li></ul></div></div><divclass=\"wrapper\"><divclass=\"help-search\">快速帮助:<selectid=\"SearchProduct\"><optionvalue=\"201\">产品1</option><optionvalue=\"202\">产品2</option><optionvalue=\"203\">产品3</option><optionvalue=\"204\">产品4</option></select><inputtype=\"text\"value=\"\"id=\"SearchKey\"/><buttonclass=\"btnbtn-success\"id=\"btnSupportSearch\">搜索</button></div><divclass=\"help-sidebar\"><dl><dt><ahref=\"/Support.aspx?product=201\">产品1</a></dt><dd><ahref=\"/Category/001.aspx?product=201\">栏目1</a></dd><dd><ahref=\"/Category/002.aspx?product=201\">栏目2</a></dd><dd><ahref=\"/Category/003.aspx?product=201\">栏目3</a></dd><dd><ahref=\"/Category/004.aspx?product=201\">栏目4</a></dd><dd><ahref=\"/Category/005.aspx?product=201\">栏目5</a></dd><dd><ahref=\"/Category/006.aspx?product=201\">栏目6</a></dd></dl><dl><dt><ahref=\"/Support.aspx?product=202\">产品2</a></dt><dd><ahref=\"/Category/001.aspx?product=202\">栏目1</a></dd><dd><ahref=\"/Category/002.aspx?product=202\">栏目2</a></dd><dd><ahref=\"/Category/003.aspx?product=202\">栏目3</a></dd><dd><ahref=\"/Category/004.aspx?product=202\">栏目4</a></dd><dd><ahref=\"/Category/005.aspx?product=202\">栏目5</a></dd><dd><ahref=\"/Category/006.aspx?product=202\">栏目6</a></dd></dl><dl><dt><ahref=\"/Support.aspx?product=203\">产品3</a></dt><dd><ahref=\"/Category/001.aspx?product=203\">栏目1</a></dd><dd><ahref=\"/Category/002.aspx?product=203\">栏目2</a></dd><dd><ahref=\"/Category/003.aspx?product=203\">栏目3</a></dd><dd><ahref=\"/Category/004.aspx?product=203\">栏目4</a></dd><dd><ahref=\"/Category/005.aspx?product=203\">栏目5</a></dd><dd><ahref=\"/Category/006.aspx?product=203\">栏目6</a></dd></dl><dl><dt><ahref=\"/Support.aspx?product=204\">产品4</a></dt><dd><ahref=\"/Category/001.aspx?product=204\">栏目1</a></dd><dd><ahref=\"/Category/002.aspx?product=204\">栏目2</a></dd><dd><ahref=\"/Category/003.aspx?product=204\">栏目3</a></dd><dd><ahref=\"/Category/004.aspx?product=204\">栏目4</a></dd><dd><ahref=\"/Category/005.aspx?product=204\">栏目5</a></dd><dd><ahref=\"/Category/006.aspx?product=204\">栏目6</a></dd></dl></div><divclass=\"help-hd\"><divclass=\"hd-mod\"><h1>视频网站付费会员增长超700%苦熬7年再度掀付费潮</h1><dl><dt>功能模块：</dt><dd><ahref='4'>测试模块</a></dd><dd><ahref='4'>订单模块</a></dd><dd><ahref='4'>产品模块</a></dd><dd><ahref='4'>新闻模块</a></dd></dl></div><divclass=\"hd-tbl\"><ulclass=\"hd-tbl-head\"id=\"tblHead\"><li><ahref=\"?tag=1\">热门问题</a></li><li><ahref=\"?tag=2\">最新推荐</a></li><li><ahref=\"?tag=3\">最新提问</a></li></ul><divclass=\"help-items\"><ul><li><ahref=\"/Help/art001.aspx\">下单后可以修改订单吗？</a></li><li><ahref=\"/Help/art001.aspx\">无货商品几天可以到货？</a></li><li><ahref=\"/Help/art001.aspx\">合约机资费如何计算？</a></li><li><ahref=\"/Help/art001.aspx\">可以开发票吗？</a></li></ul></div></div><divclass=\"aspnet-pager\"><span>首页</span><span>上一页</span><span>1</span><span>下一页</span><span>末页</span></div></div><divclass=\"clearfix\"></div></div><divclass=\"help-modwrapper\"><dl><dt>新手帮助</dt><dd><ahref=\"/Article/art001.aspx\">购物流程</a></dd><dd><ahref=\"/Article/art001.aspx\">会员介绍</a></dd><dd><ahref=\"/Article/art001.aspx\">生活旅行/团购</a></dd><dd><ahref=\"/Article/art001.aspx\">常见问题</a></dd><dd><ahref=\"/Article/art001.aspx\">联系客服</a></dd></dl><dl><dt>极念网络</dt><dd><ahref=\"/Article/art001.aspx\">购物流程</a></dd><dd><ahref=\"/Article/art001.aspx\">会员介绍</a></dd><dd><ahref=\"/Article/art001.aspx\">生活旅行/团购</a></dd><dd><ahref=\"/Article/art001.aspx\">常见问题</a></dd><dd><ahref=\"/Article/art001.aspx\">联系客服</a></dd></dl><dl><dt>免费应用</dt><dd><ahref=\"/Article/art001.aspx\">购物流程</a></dd><dd><ahref=\"/Article/art001.aspx\">会员介绍</a></dd><dd><ahref=\"/Article/art001.aspx\">生活旅行/团购</a></dd><dd><ahref=\"/Article/art001.aspx\">常见问题</a></dd><dd><ahref=\"/Article/art001.aspx\">联系客服</a></dd></dl><dl><dt>其他服务</dt><dd><ahref=\"/Article/art001.aspx\">购物流程</a></dd><dd><ahref=\"/Article/art001.aspx\">会员介绍</a></dd><dd><ahref=\"/Article/art001.aspx\">生活旅行/团购</a></dd><dd><ahref=\"/Article/art001.aspx\">常见问题</a></dd><dd><ahref=\"/Article/art001.aspx\">联系客服</a></dd></dl><dl><dt>关于我们</dt><dd><ahref=\"/Article/art001.aspx\">购物流程</a></dd><dd><ahref=\"/Article/art001.aspx\">会员介绍</a></dd><dd><ahref=\"/Article/art001.aspx\">生活旅行/团购</a></dd><dd><ahref=\"/Article/art001.aspx\">常见问题</a></dd><dd><ahref=\"/Article/art001.aspx\">联系客服</a></dd></dl></div><divclass=\"footer\"><pclass=\"text-muted\">极念网络版权所有©2015</p></div></body></html>";

        const int MAX_RUN_COUNT = 100;
#if BASE_PAGE_TEST
        [Fact]
        public void TestILVsReflectionPage()
        {

            JinianNet.JNTemplate.TemplateContext ctx = new JinianNet.JNTemplate.TemplateContext();
            ctx.TempData.Push("func", new TemplateMethod());
            SiteInfo site = new SiteInfo();
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
            ctx.TempData.Push("Site", site);

            string basePath =
#if NET20 || NET40
                new System.IO.DirectoryInfo(System.Environment.CurrentDirectory).Parent.Parent.FullName;
#else
                new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
#endif
            string path = basePath + "\\templets\\default";
            // JinianNet.JNTemplate.Dynamic.IDynamicHelpers h;
            Configuration.EngineConfig conf;

            string text1 = null, text2 = null;
            string result = "";
            Stopwatch s = new Stopwatch();
            s.Start();
            s.Stop();
            ////////////////////////////////////////////////////////////////////////////////////
            //h = new JinianNet.JNTemplate.Dynamic.ILHelpers();
            conf = Configuration.EngineConfig.CreateDefault();
            Engine.Configure(conf);
            s.Restart();

            for (var i = 0; i < MAX_RUN_COUNT; i++)
            {
                JinianNet.JNTemplate.Template t = new JinianNet.JNTemplate.Template(ctx, System.IO.File.ReadAllText(path + "\\questionlist.html"));

                t.Context.CurrentPath = path;
                text1 = t.Render();

            }
            s.Stop();
            result += "\r\n耗时：" + s.ElapsedMilliseconds + "毫秒 - 反射(" + MAX_RUN_COUNT + "次)运行";
            ////////////////////////////////////////////////////////////////////////////////////

            GC.Collect();


            ////////////////////////////////////////////////////////////////////////////////////
            //h = new JinianNet.JNTemplate.Dynamic.ReflectionHelpers();
            conf = Configuration.EngineConfig.CreateDefault();
            conf.CacheProvider = new UserCache();
            Engine.Configure(conf);
            s.Restart();
            for (var i = 0; i < MAX_RUN_COUNT; i++)
            {
                JinianNet.JNTemplate.Template t = new JinianNet.JNTemplate.Template(ctx, System.IO.File.ReadAllText(path + "\\questionlist.html"));

                t.Context.CurrentPath = path;
                text2 = t.Render();
                //h.ExcuteMethod(DateTime.Now, "AddDays", new object[] { 30 });
            }
            s.Stop();
            ////////////////////////////////////////////////////////////////////////////////////

            result += "\r\n耗时：" + s.ElapsedMilliseconds + "毫秒 - IL(" + MAX_RUN_COUNT + "次)运行";

            System.IO.File.WriteAllText(basePath + "\\result\\ILVsReflection.txt", result);
            Assert.Equal(text1, text2);

        }


        [Fact]
        public void TestILage()
        {

            var c = Configuration.EngineConfig.CreateDefault();
            //开始严格大小写模式 默认忽略大小写
            //conf.IgnoreCase = false;
            Engine.Configure(c);

            JinianNet.JNTemplate.TemplateContext ctx = new JinianNet.JNTemplate.TemplateContext();
            ctx.TempData.Push("func", new TemplateMethod());
            SiteInfo site = new SiteInfo();
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
            //ctx.TempData.Push("Model", );
            ctx.TempData.Push("Site", site);

            string basePath =
#if NET20 || NET40
                new System.IO.DirectoryInfo(System.Environment.CurrentDirectory).Parent.Parent.FullName;
#else
                new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
#endif
            string path = basePath + "\\templets\\default";
            string html = null;
            var conf = Configuration.EngineConfig.CreateDefault();
            conf.CacheProvider = new UserCache();
            conf.DynamicProvider = new JinianNet.JNTemplate.Dynamic.ILDynamicProvider();// new TestExecutor();
            Engine.Configure(conf);
            Stopwatch s = new Stopwatch();
            s.Start();
            for (var i = 0; i < MAX_RUN_COUNT; i++)
            {
                JinianNet.JNTemplate.Template t = new JinianNet.JNTemplate.Template(ctx, System.IO.File.ReadAllText(path + "\\questionlist.html"));
                t.Context.CurrentPath = path;
                if (i == MAX_RUN_COUNT - 1)
                {
                    System.IO.File.WriteAllText(basePath + "\\result\\IL.html", t.Render());
                }
                else
                {
                    html=t.Render();
                }

            }
            s.Stop();
            string result = "\r\n运行耗时：" + s.ElapsedMilliseconds + "毫秒 IL(" + MAX_RUN_COUNT + "次)";
            System.IO.File.AppendAllText(basePath + "\\result\\ILVsReflection.txt", result);

            Assert.Equal(HTML_RESULT, html.Replace("\r", "").Replace("\t", "").Replace("\n", "").Replace(" ", ""));


        }
        [Fact]
        public void TestReflectionPage()
        {

            JinianNet.JNTemplate.TemplateContext ctx = new JinianNet.JNTemplate.TemplateContext();
            ctx.TempData.Push("func", new TemplateMethod());
            SiteInfo site = new SiteInfo();
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
            //ctx.TempData.Push("Model", );
            ctx.TempData.Push("Site", site);

            string basePath =
#if NET20 || NET40
                new System.IO.DirectoryInfo(System.Environment.CurrentDirectory).Parent.Parent.FullName;
#else
                new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
#endif
            string path = basePath + "\\templets\\default";

            string html = null;
            var conf = Configuration.EngineConfig.CreateDefault();
            Engine.Configure(conf);

            Stopwatch s = new Stopwatch();
            s.Start();
            for (var i = 0; i < MAX_RUN_COUNT; i++)
            {
                JinianNet.JNTemplate.Template t = new JinianNet.JNTemplate.Template(ctx, System.IO.File.ReadAllText(path + "\\questionlist.html"));
                t.Context.CurrentPath = path;
                if (i == MAX_RUN_COUNT - 1)
                {
                    System.IO.File.WriteAllText(basePath + "\\result\\REFLECTION.html", t.Render());
                }
                else
                {
                    html=t.Render();
                }

            }
            s.Stop();
            string result = "\r\n运行耗时：" + s.ElapsedMilliseconds + "毫秒 反射(" + MAX_RUN_COUNT + "次)";
            System.IO.File.AppendAllText(basePath + "\\result\\ILVsReflection.txt", result);
            Assert.Equal(HTML_RESULT, html.Replace("\r", "").Replace("\t", "").Replace("\n", "").Replace(" ", ""));


        }


        [Fact]
        public void TestPage()
        {
            var conf = Configuration.EngineConfig.CreateDefault();
            Engine.Configure(conf);

            JinianNet.JNTemplate.TemplateContext ctx = new JinianNet.JNTemplate.TemplateContext();

            ctx.TempData.Push("func", new TemplateMethod());

            SiteInfo site = new SiteInfo();
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
            //ctx.TempData.Push("Model", );
            ctx.TempData.Push("Site", site);
            string basePath =
#if NET20 || NET40
                new System.IO.DirectoryInfo(System.Environment.CurrentDirectory).Parent.Parent.FullName;
#else
                new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
#endif
            string path = basePath + "\\templets\\default";

            JinianNet.JNTemplate.Template t = new JinianNet.JNTemplate.Template(ctx, System.IO.File.ReadAllText(path + "\\questionlist.html"));
            t.Context.CurrentPath = path;

            string result = t.Render();

            //可直接查看项目录下的html/jnt.html 文件效果
            System.IO.File.WriteAllText(basePath + "\\result\\jnt.html", result);

            
            Assert.Equal(HTML_RESULT, result.Replace("\r","").Replace("\t", "").Replace("\n", "").Replace(" ", ""));

        }
#endif
#if VER_EQ_TEST
        /// <summary>
        /// 多版本比较测试
        /// </summary>
        [Fact]
        public void TestVER_EQ_TESTsion()
        {
            var tm = new TemplateMethod();
            SiteInfo site = new SiteInfo();
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
            string basePath = new System.IO.DirectoryInfo(System.Environment.CurrentDirectory).Parent.Parent.FullName;
            string path = basePath + "\\templets\\default";

            string content = System.IO.File.ReadAllText(path + "\\questionlist.html");

            FileInfo[] assFlies = new DirectoryInfo(basePath + "\\dll").GetFiles("JinianNet.JNTemplate*.dll");
            string result = DateTime.Now.ToString();
            Stopwatch s = new Stopwatch();
            for (int i = 0; i < assFlies.Length; i++)
            {
                Assembly ass = System.Reflection.Assembly.LoadFile(assFlies[i].FullName);
                object ctx = ass.CreateInstance("JinianNet.JNTemplate.TemplateContext");
                object data = ctx.GetType().GetProperty("TempData").GetValue(ctx, null);
                MethodInfo mi = data.GetType().GetMethod("Push");
                mi.Invoke(data, new object[] { "func", tm });
                mi.Invoke(data, new object[] { "Site", site });
                ctx.GetType().GetProperty("CurrentPath").SetValue(ctx, path, null);

                s.Restart();
                for (int j = 0; j < MAX_RUN_COUNT; j++)
                {
                    object t = ass.CreateInstance("JinianNet.JNTemplate.Template"); ;
                    t.GetType().GetProperty("Context").SetValue(t, ctx, null);
                    t.GetType().GetProperty("TemplateContent").SetValue(t, content, null);
                    object r = t.GetType().GetMethod("Render", new Type[0]).Invoke(t, new object[0] { });

                    if (j == MAX_RUN_COUNT-1)
                    {
                        System.IO.File.WriteAllText(basePath + "\\result\\"+ assFlies[i].Name +".html", r.ToString());
                    }
                }
                s.Stop();
                result += "\r\n:耗时：" + s.ElapsedMilliseconds.ToString() + "毫秒 次数:"+ MAX_RUN_COUNT + " 文件:" + assFlies[i].Name + " 版本号：" + ass.GetName().Version;
                System.Threading.Thread.Sleep(200);
            }
            if (System.IO.File.Exists(basePath + "\\result\\result.txt"))
            {
                if (System.IO.File.GetLastWriteTime(basePath + "\\result\\result.txt").Date == DateTime.Now.Date)
                {
                    result = System.IO.File.ReadAllText(basePath + "\\result\\result.txt") + "\r\n" + result;
                }
            }

            System.IO.File.WriteAllText(basePath + "\\result\\result.txt", result);
        }
#endif

#if NVELOCITY_TEST
        [Fact]
        public void TestJuxtaposePage()
        {
            SiteInfo site = new SiteInfo();
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


            string basePath = new System.IO.DirectoryInfo(System.Environment.CurrentDirectory).Parent.Parent.FullName;
            string path = basePath + "\\templets\\nv";


            NVelocity.Context.IContext ctx = new NVelocity.VelocityContext();
            ctx.Put("func", new TemplateMethod());
            ctx.Put("Site", site);



            NVelocity.App.VelocityEngine velocity = new NVelocity.App.VelocityEngine();
            Commons.Collections.ExtendedProperties props = new Commons.Collections.ExtendedProperties();
            props.AddProperty(NVelocity.Runtime.RuntimeConstants.RESOURCE_LOADER, "file");
            props.AddProperty(NVelocity.Runtime.RuntimeConstants.FILE_RESOURCE_LOADER_PATH, path);
            props.AddProperty(NVelocity.Runtime.RuntimeConstants.INPUT_ENCODING, "utf-8");
            props.AddProperty(NVelocity.Runtime.RuntimeConstants.OUTPUT_ENCODING, "utf-8");
            velocity.Init(props);
            NVelocity.Template t = velocity.GetTemplate("questionlist.html");
            string result;
            using (System.IO.StringWriter write = new StringWriter())
            {
                t.Merge(ctx, write);
                result = write.ToString();
            }

            //可直接查看项目录下的html/nv.html 文件效果
            System.IO.File.WriteAllText(basePath + "\\result\\nv.html", result);
        }
#endif
    }
}
#endif