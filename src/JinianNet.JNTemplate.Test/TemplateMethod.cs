using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinianNet.JNTemplate.Test
{
    public class TemplateMethod
    {

        public int RecordCount { get; set; }

        public int PageSzie
        {
            get
            {
                return 20;
            }
        }

        public int GetPageIndex()
        {


            return 1;
        }


        /// <summary>
        /// 获取分页
        /// </summary>
        /// <returns></returns>
        public string GetPager()
        {
            string url = "?page={0}"; ;

            return GetPageLinker(null, url, 10, GetPageIndex(), RecordCount, PageSzie, "首页", "上一页", "下一页", "末页");
        }


        /// <summary>
        /// 通用方法分页
        /// </summary>
        /// <param name="HomePagerURL">首页URL可选 例：List.aspx</param>
        /// <param name="PagerURL">URL，必填，格式如:List.aspx?Page={0}</param>
        /// <param name="PagerStep">步长</param>
        /// <param name="PagerCur">当前页</param>
        /// <param name="PagerTotal">总记录条数</param>
        /// <param name="PagerSize">每页显示的数量</param>
        /// <param name="PagerFirstName">第一页名称</param>
        /// <param name="PagerPreName">上一页名称</param>
        /// <param name="PagerNextName">下一页名称</param>
        /// <param name="PagerLastName">最后一页名称</param>
        /// <returns></returns>
        public string GetPageLinker(string HomePagerURL, string PagerURL, int PagerStep, int PagerCur, int PagerTotal, int PagerSize, string PagerFirstName, string PagerPreName, string PagerNextName, string PagerLastName)
        {
            int MiddleNumber;
            int TotalPage;
            int i;
            System.Text.StringBuilder Result = new System.Text.StringBuilder();

            MiddleNumber = (int)Math.Round((double)(PagerStep / 2), 0);
            if ((PagerTotal % PagerSize) != 0)
            {
                TotalPage = (int)Math.Round((double)(PagerTotal / PagerSize), 0) + 1;
            }
            else
            {
                TotalPage = (int)Math.Round((double)(PagerTotal / PagerSize), 0);
            }
            if (PagerCur > TotalPage)
            {
                PagerCur = TotalPage;
            }
            if (PagerCur < 1)
            {
                PagerCur = 1;
            }


            if (PagerCur == 1)
            {
                Result.Append("<span>");
                Result.Append(PagerFirstName);
                Result.Append("</span> <span>");
                Result.Append(PagerPreName);
                Result.Append("</span>");
            }
            else
            {
                if (string.IsNullOrEmpty(HomePagerURL))
                {
                    Result.Append("<a href=\"");
                    Result.AppendFormat(PagerURL, "1");
                    Result.AppendFormat("\">{0}</a>", PagerFirstName);
                }
                else
                {
                    Result.AppendFormat("<a href=\"{0}\">{1}</a>", HomePagerURL, PagerFirstName);
                }
                Result.Append(" <a href=\"");
                Result.AppendFormat(PagerURL, PagerCur - 1);
                Result.AppendFormat("\">{0}</a>", PagerPreName);

            }

            if (TotalPage <= PagerStep)
            {
                for (i = 1; i <= TotalPage; i++)
                {
                    if (i == PagerCur)
                    {
                        Result.AppendFormat(" <span>{0}</span>", i);
                    }
                    else
                    {
                        Result.Append(" <a href=\"");
                        Result.AppendFormat(PagerURL, i);
                        Result.AppendFormat("\">{0}</a>", i);
                    }
                }
            }
            else
            {
                if (PagerCur <= MiddleNumber)
                {
                    for (i = 1; i <= PagerCur; i++)
                    {
                        if (i == PagerCur)
                        {
                            Result.AppendFormat(" <span>{0}</span>", i);
                        }
                        else
                        {
                            Result.Append(" <a href=\"");
                            Result.AppendFormat(PagerURL, i);
                            Result.AppendFormat("\">{0}</a>", i);
                        }
                    }
                    for (i = PagerCur + 1; i <= PagerStep; i++)
                    {
                        Result.Append(" <a href=\"");
                        Result.AppendFormat(PagerURL, i);
                        Result.AppendFormat("\">{0}</a>", i);
                    }
                }
                else
                {
                    for (i = (PagerCur - MiddleNumber + 1); i <= PagerCur; i++)
                    {
                        if (i == PagerCur)
                        {
                            Result.AppendFormat(" <span>{0}</span>", i);
                        }
                        else
                        {
                            Result.Append(" <a href=\"");
                            Result.AppendFormat(PagerURL, i);
                            Result.AppendFormat("\">{0}</a>", i);
                        }
                    }
                    if ((PagerCur + MiddleNumber - PagerStep) < TotalPage)
                    {
                        for (i = PagerCur + 1; i <= PagerCur + PagerStep - MiddleNumber; i++)
                        {
                            if (i <= TotalPage)
                            {
                                Result.Append(" <a href=\"");
                                Result.AppendFormat(PagerURL, i);
                                Result.AppendFormat("\">{0}</a>", i);
                            }

                        }
                    }
                    else
                    {
                        for (i = PagerCur + 1; i <= TotalPage; i++)
                        {
                            if (i <= TotalPage)
                            {
                                Result.Append(" <a href=\"");
                                Result.AppendFormat(PagerURL, i);
                                Result.AppendFormat("\">{0}</a>", i);
                            }
                        }
                    }
                }
            }
            if (PagerCur == TotalPage || TotalPage == 0)
            {
                Result.AppendFormat(" <span>{0}</span> <span>{1}</span>", PagerNextName, PagerLastName);
            }
            else
            {
                Result.Append(" <a href=\"");
                Result.AppendFormat(PagerURL, PagerCur + 1);
                Result.AppendFormat("\">{0}</a>", PagerNextName);
                Result.Append(" <a href=\"");
                Result.AppendFormat(PagerURL, TotalPage);
                Result.AppendFormat("\">{0}</a>", PagerLastName);
            }


            return Result.ToString();

        }


        public string Get(string key)
        {
            return key;
        }
        /// <summary>
        /// 获取一个数字类型的GET值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetInt(string key)
        {
            return int.Parse(key);
        }

        /// <summary>
        /// 获取一个数字类型的GET值 不存在就返回默认值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public int GetDefaultInt(string key, int defaultValue)
        {
            return defaultValue;
        }

        public List<ViewModel.Category> GetCategoryList(int mode)
        {
            List<ViewModel.Category> list = new List<ViewModel.Category>();
            list.Add(new ViewModel.Category()
            {
                CategoryId = 1,
                CategoryName = "栏目1",
                CreateDate = DateTime.Now,
                Depth = 1,
                EnglishName = "001",
                ModuleId = 1
            });


            list.Add(new ViewModel.Category()
            {
                CategoryId = 2,
                CategoryName = "栏目2",
                CreateDate = DateTime.Now,
                Depth = 1,
                EnglishName = "002",
                ModuleId = 1
            });

            list.Add(new ViewModel.Category()
            {
                CategoryId = 3,
                CategoryName = "栏目3",
                CreateDate = DateTime.Now,
                Depth = 1,
                EnglishName = "003",
                ModuleId = 1
            });

            list.Add(new ViewModel.Category()
            {
                CategoryId = 4,
                CategoryName = "栏目4",
                CreateDate = DateTime.Now,
                Depth = 1,
                EnglishName = "004",
                ModuleId = 1
            });

            list.Add(new ViewModel.Category()
            {
                CategoryId = 5,
                CategoryName = "栏目5",
                CreateDate = DateTime.Now,
                Depth = 1,
                EnglishName = "005",
                ModuleId = 1
            });

            list.Add(new ViewModel.Category()
            {
                CategoryId = 6,
                CategoryName = "栏目6",
                CreateDate = DateTime.Now,
                Depth = 1,
                EnglishName = "006",
                ModuleId = 1
            });
            return list;
        }

        public ViewModel.Product GetProductItem(string key)
        {
            ViewModel.Product model = new ViewModel.Product();
            model.BasisId = 1;
            model.CategoryId = 1;
            model.Content = @"<p style=""text-align: center; text-indent: 0;""><img src=""http://upload.chinaz.com/2015/0624/1435111490722.jpg"" border=""0"" alt=""视频网站 付费会员""></p>
<p>6月24日报道&nbsp;文/肖芳</p>
<p>近日，爱奇艺高调宣布其月度付费VIP会员数已达501.7万，并称视频付费业务台风已经到来。而阿里巴巴宣布进入视频付费市场，将推出付费视频服务TBO(Tmall&nbsp;Box&nbsp;Office)，它的模式将更接近美国在线影片租赁提供商Netflix，其中90%的TBO内容都将采用付费观看模式。</p>
<p>这不是业界首次探讨视频网站收费的可能性了。早在2008年，激动网就正式推出了付费点播品牌“激动派”，虽然2011年激动网声称80%的收入来自付费用户，如今却已转型淡出视频行业。其他的也基本都是“雷声大，雨点小”，付费没有形成足够的阵势。当时有说法称“谁第一个收费谁就第一个倒下”。</p>
<p>时隔5年视频网站再次呼唤用户付费。业内人士透露，目前视频网站或片方都已经在酝酿网络付费看剧，试图在网络付费领域上分一杯羹，最快明年就会试水。这一次的底气在哪？谈论这个问题之前不妨先从5年前的收费为何没有成气候说起。</p>
<p><strong>早年内容不成熟&nbsp;付费1~2元也无人问津</strong></p>
<p>2010年，迅雷看看推出向用户收费的“红宝石影院”业务，一期推广高清下载，二期将推广高清在线观看，一部电影收费大约1元-2元钱，高清在线观看的收费项目将成为现实。</p>
<p>由迅雷看看当时的宣传页面可以窥见“红宝石影院”的初衷：“买一张盗版碟至少要花5元钱，而在红宝石上下载一部正版高清电影最低只花2元钱。正版比盗版还便宜。”虽然在业务推出前期，迅雷看看展开声势浩大的宣传，但“红宝石影院”后来也销声匿迹，迅雷看看的营收依然是以传统的广告为主。今年年初，迅雷把一直处于亏损状态下的看看出售，免于拖累上市公司。</p>
<p>花2元看正版，比5元买盗版碟还便宜，这个初衷是好的，但也要考虑收费实施的基础。一方面是用户付费意愿，另一方面是视频网站的服务能否达到收费的水平。</p>
<p>在用户付费意愿上，2010年某门户网站曾经做过一项调查。结果显示，愿意为视频点播付费的网友只有383名，而不愿意的则达到6095名，后者是前者的15倍。由此可见，只有6%的网友愿意付费，没有用户的支持视频网站畅想的再美好都无济于事。</p>
<p>另一方面，2010年前后，在线视频的品质还不够好。由于带宽等因素的限制，视频很难达到高清的效果。同时，视频网站购买版权的意识也不如现在强，很多内容都来自网友上传，体验很差。</p>
<p>当时，另一家坚持免费观看的视频网站负责人道出了视频收费不宜大规模推广的原委。她指出，要想让用户掏钱看视频，首先要满足两个条件：一是网站要有独家的、不可替代的内容，否则网友不会“买账”；二是用户的使用习惯。对于前者，可以靠投入重金买版权来实现；但对于后者，她并不乐观地表示，让习惯了免费看视频的用户掏钱买收视权，短期内是不太现实的。</p>
<p><strong>服务升级后&nbsp;视频网站亟需付费扭转巨亏</strong></p>
<p>可以看到，2010年之后视频网站在朝着正版化、高清化发展。视频网站在不断砸钱购买内容，同时也在改善视频播放技术，让网友获得更好的观看体验。</p>
<p>对比2010年优酷网和如今优酷土豆的财报便可以发现端倪。2010年第四季度，优酷的内容成本为1250万美元，比2009年增长11%，净亏损为570万美元。2014年第四季度，优酷土豆的内容成本为9,720万美元，是2010年同期的8倍，净亏损为5130万美元，接近2010年同期的10倍。</p>
<p>越是投入越是亏得厉害，不只是优酷，这是近5年来视频行业发展的缩影。可以看到多家视频网站因资金问题“卖身”，而现在留下的视频网站背后都背靠大树。没有巨头的支持，视频“烧钱”的游戏很难再持续下去。</p>
<p style=""text-align: center; text-indent: 0;""><img src=""http://upload.chinaz.com/2015/0624/1435111500344.jpg"" border=""0"" alt=""视频网站 付费会员""></p>
<p>视频网站付费会员增长超700% 苦熬7年再度掀付费潮</p>
<p>归根到底，这是由于广告收入的增速远远不及内容成本的增速（图为2014年优酷土豆内容成本和广告收入成本的同比增长），依靠内容投入拉动营收就如同一个无底洞，只会将自己陷得越来越深。</p>                                                                ";
            model.CreateDate = DateTime.Now;
            model.DatePrice = 940;
            model.DefaultPicture = "http://upload.chinaz.com/2015/0624/1435111500344.jpg";
            model.Description = "近日，爱奇艺高调宣布其月度付费VIP会员数已达501.7万，并称视频付费业务台风已经到来。而阿里巴巴宣布进入视频付费市场，将推出付费视频服务TBO(Tmall&nbsp;Box&nbsp;Office)，它的模式将更接近美国在线影片租赁提供商Netflix，其中90%的TBO内容都将采用付费观看模式。";
            model.EditDate = DateTime.Now;
            model.EnglishName = "001";
            model.ExampleUrl = "http://www.baidu.com";
            model.FileSize = "54KB";
            model.Title = "视频网站付费会员增长超700% 苦熬7年再度掀付费潮";
            return model;
        }

        public string GetProductUrl(Model.Product model)
        {

            if (!string.IsNullOrEmpty(model.EnglishName))
            {
                return string.Concat("/Product/", model.EnglishName, ".aspx");//Article/About.aspx
            }
            else
            {
                return string.Concat("/Product.aspx?id=", model.BasisId.ToString());
            }
        }

        public string GetArticleUrl(Model.Article model)
        {

            if (!string.IsNullOrEmpty(model.EnglishName))
            {
                return string.Concat("/Article/", model.EnglishName, ".aspx");//Article/About.aspx
            }
            else
            {
                return string.Concat("/Article.aspx?id=", model.BasisId.ToString());
            }
        }



        public string GetHelpUrl(Model.Help model)
        {

            if (!string.IsNullOrEmpty(model.EnglishName))
            {
                return string.Concat("/Help/", model.EnglishName, ".aspx");//Article/About.aspx
            }
            else
            {
                return string.Concat("/Help.aspx?id=", model.BasisId.ToString());
            }
        }


        public List<Model.ProductModule> GetProductModule(int product)
        {
            var list = new List<Model.ProductModule>();
            list.Add(new Model.ProductModule()
            {
                BasisId = 101,
                ModuleName = "测试模块",
                ProductModuleId = product

            });

            list.Add(new Model.ProductModule()
            {
                BasisId = 102,
                ModuleName = "订单模块",
                ProductModuleId = product

            });

            list.Add(new Model.ProductModule()
            {
                BasisId = 103,
                ModuleName = "产品模块",
                ProductModuleId = product

            });

            list.Add(new Model.ProductModule()
            {
                BasisId = 104,
                ModuleName = "新闻模块",
                ProductModuleId = product

            });
            return list;
        }

        public List<Model.Product> GetProductList()
        {
            var list = new List<Model.Product>();
            list.Add(new Model.Product()
            {
                BasisId = 201,
                CategoryId = 1,
                Content = "",
                CreateDate = DateTime.Now,
                DatePrice = 245,
                DefaultPicture = "",
                Description = "",
                DownloadUrl = "",
                EditDate = DateTime.Now,
                EnglishName = "pro001",
                ExampleUrl = "",
                FileSize = "564kb",
                Gateway = "",
                ModuleId = 2,
                Title = "产品1"
            });

            list.Add(new Model.Product()
            {
                BasisId = 202,
                CategoryId = 1,
                Content = "",
                CreateDate = DateTime.Now,
                DatePrice = 245,
                DefaultPicture = "",
                Description = "",
                DownloadUrl = "",
                EditDate = DateTime.Now,
                EnglishName = "pro001",
                ExampleUrl = "",
                FileSize = "564kb",
                Gateway = "",
                ModuleId = 2,
                Title = "产品2"
            });

            list.Add(new Model.Product()
            {
                BasisId = 203,
                CategoryId = 1,
                Content = "",
                CreateDate = DateTime.Now,
                DatePrice = 245,
                DefaultPicture = "",
                Description = "",
                DownloadUrl = "",
                EditDate = DateTime.Now,
                EnglishName = "pro001",
                ExampleUrl = "",
                FileSize = "564kb",
                Gateway = "",
                ModuleId = 2,
                Title = "产品3"
            });

            list.Add(new Model.Product()
            {
                BasisId = 204,
                CategoryId = 1,
                Content = "",
                CreateDate = DateTime.Now,
                DatePrice = 245,
                DefaultPicture = "",
                Description = "",
                DownloadUrl = "",
                EditDate = DateTime.Now,
                EnglishName = "pro001",
                ExampleUrl = "",
                FileSize = "564kb",
                Gateway = "",
                ModuleId = 2,
                Title = "产品4"
            });
            return list;
        }

        public List<Model.Article> GetArticleList(int id)
        {
            var list = new List<Model.Article>();
            list.Add(new Model.Article()
            {
                BasisId = 301,
                CategoryId = 1,
                Content = "",
                CreateDate = DateTime.Now,
                DefaultPicture = "",
                Description = "",
                EditDate = DateTime.Now,
                EnglishName = "art001",
                ModuleId = 1,
                Title = "购物流程"
            });
            list.Add(new Model.Article()
            {
                BasisId = 301,
                CategoryId = 1,
                Content = "",
                CreateDate = DateTime.Now,
                DefaultPicture = "",
                Description = "",
                EditDate = DateTime.Now,
                EnglishName = "art001",
                ModuleId = 1,
                Title = "会员介绍"
            });
            list.Add(new Model.Article()
            {
                BasisId = 301,
                CategoryId = 1,
                Content = "",
                CreateDate = DateTime.Now,
                DefaultPicture = "",
                Description = "",
                EditDate = DateTime.Now,
                EnglishName = "art001",
                ModuleId = 1,
                Title = "生活旅行/团购"
            });
            list.Add(new Model.Article()
            {
                BasisId = 301,
                CategoryId = 1,
                Content = "",
                CreateDate = DateTime.Now,
                DefaultPicture = "",
                Description = "",
                EditDate = DateTime.Now,
                EnglishName = "art001",
                ModuleId = 1,
                Title = "常见问题"
            });
            list.Add(new Model.Article()
            {
                BasisId = 301,
                CategoryId = 1,
                Content = "",
                CreateDate = DateTime.Now,
                DefaultPicture = "",
                Description = "",
                EditDate = DateTime.Now,
                EnglishName = "art001",
                ModuleId = 1,
                Title = "联系客服"
            });
            return list;
        }

        /// <summary>
        /// 获取帮助列表
        /// </summary>
        /// <param name="category"></param>
        /// <param name="product"></param>
        /// <param name="module"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<Model.Help> GetHelpList(int category, int product, int module, int type)
        {

            RecordCount = 4;
            var list = new List<Model.Help>();
            list.Add(new Model.Help()
            {
                BasisId = 301,
                CategoryId = 1,
                Content = "",
                CreateDate = DateTime.Now,
                DefaultPicture = "",
                Description = "",
                EditDate = DateTime.Now,
                EnglishName = "art001",
                ModuleId = 1,
                Title = "下单后可以修改订单吗？"
            });
            list.Add(new Model.Help()
            {
                BasisId = 301,
                CategoryId = 1,
                Content = "",
                CreateDate = DateTime.Now,
                DefaultPicture = "",
                Description = "",
                EditDate = DateTime.Now,
                EnglishName = "art001",
                ModuleId = 1,
                Title = "无货商品几天可以到货？"
            });
            list.Add(new Model.Help()
            {
                BasisId = 301,
                CategoryId = 1,
                Content = "",
                CreateDate = DateTime.Now,
                DefaultPicture = "",
                Description = "",
                EditDate = DateTime.Now,
                EnglishName = "art001",
                ModuleId = 1,
                Title = "合约机资费如何计算？"
            });
            list.Add(new Model.Help()
            {
                BasisId = 301,
                CategoryId = 1,
                Content = "",
                CreateDate = DateTime.Now,
                DefaultPicture = "",
                Description = "",
                EditDate = DateTime.Now,
                EnglishName = "art001",
                ModuleId = 1,
                Title = "可以开发票吗？"
            });
            return list;

        }

        #region 标签

        #endregion

    }
}
