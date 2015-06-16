using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 实际WEB页面模板测试
    /// </summary>
    [TestClass]
    public class WebPageTests
    {
        public void TestPage()
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

            JinianNet.JNTemplate.Template t = new JinianNet.JNTemplate.Template(ctx, System.IO.File.ReadAllText((@"questionlist.html")));
            t.Context.CurrentPath = @"D:\";
        }
    }

    #region 其它类
    public class SiteInfo
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string KeyWords { get; set; }
        public string Description { get; set; }
        public string Theme { get; set; }
        public string Logo { get; set; }
        public string Copyright { get; set; }
        public string Host { get; set; }
        public string ThemeDirectory { get; set; }
        public string SiteDirectory { get; set; }
        public string Url { get; set; }
        public string ThemeUrl { get; set; }
    }

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

            return TestProject.Framework.Utils.GetPageLinker(null, url, 10, GetPageIndex(), RecordCount, PageSzie, "首页", "上一页", "下一页", "末页");
        }
         <summary>
         获取一个GET参数值
         </summary>
         <param name="key"></param>
         <returns></returns>
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

        public List<TestProject.ViewModel.Category> GetCategoryList(int mode)
        {
            return new TestProject.DAL.Category().GetList(mode, 0, true, true, null);
        }

        public TestProject.ViewModel.Product GetProductItem(string key)
        {
            return new TestProject.DAL.Product().GetItem(int.Parse(key));
        }

        public string GetProductUrl(TestProject.Model.Product model)
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

        public string GetHelpUrl(TestProject.Model.Help model)
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


        public List<TestProject.Model.ProductModule> GetProductModule(int product)
        {
            return new TestProject.DAL.ProductModule().GetList(product);
        }

        public List<TestProject.Model.Product> GetProductList()
        {
            return new TestProject.DAL.Product().GetList();
        }

        public List<TestProject.Model.Article> GetArticleList(int id)
        {
            return new TestProject.DAL.Article().GetList(id);
        }

        /// <summary>
        /// 获取帮助列表
        /// </summary>
        /// <param name="category"></param>
        /// <param name="product"></param>
        /// <param name="module"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<TestProject.Model.Help> GetHelpList(int category, int product, int module, int type)
        {
            int? categoryId = null;
            int? productId = null;
            int? productModuleId = null;

            bool? isHot = null;
            bool? isRecommend = null;
            //,int pageIndex, int pageSize, out int recordCount;
            if (category > 0)
                categoryId = category;
            if (product > 0)
                productId = product;
            if (module > 0)
                productModuleId = module;
            switch (type)
            {
                case 1:
                    isHot = true;
                    break;
                case 2:
                    isRecommend = true;
                    break;
            }

            int count;
            var list = new TestProject.DAL.Help().GetList(categoryId, productId, productModuleId, null, isHot, isRecommend,
                GetPageIndex(), PageSzie, out count);
            RecordCount = count;

            return list;

        }

        #region 标签
        
        //public List<TestProject.Model.Help> GetHelpList(string name, int size, int product)
        //{
        //    var c = new TestProject.DAL.Category().GetItem(name);
        //    int returnBack;
        //    if (c != null)
        //    {
        //        return new TestProject.DAL.Help().GetList(c.CategoryId, product,
        //            null, null, null, null, 1, size, out returnBack);
        //    }
        //    else
        //    {
        //        return new List<TestProject.Model.Help>();
        //    }
        //}


        //public List<TestProject.Model.Article> GetAllArticleList(int id)
        //{
        //    return new TestProject.DAL.Article().GetList(id);
        //}


        //public string Label(string name)
        //{
        //    return "标签内容";
        //}
        
        //public DataTable Query(string sql)
        //{
        //    return new TestProject.DAL.SqlHelper().ExecuteTable(sql);
        //}

        //public string GetArticleUrl(TestProject.Model.Article model)
        //{
        //    if (model.IsUrl)
        //    {
        //        return model.TurnUrl;
        //    }
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(model.EnglishName))
        //        {
        //            return string.Concat("/Article/", model.EnglishName, ".aspx");//Article/About.aspx
        //        }
        //        else
        //        {
        //            return string.Concat("/Article.aspx?id=", model.BasisId.ToString());
        //        }
        //    }
        //}


        //public string GetCategoryUrl(TestProject.Model.Category model)
        //{
        //    if (model.IsUrl)
        //    {
        //        return model.TurnUrl;
        //    }
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(model.EnglishName))
        //        {
        //            return string.Concat("/Category/", model.EnglishName, ".aspx");//Article/About.aspx
        //        }
        //        else
        //        {
        //            return string.Concat("/Category.aspx?id=", model.CategoryId.ToString());
        //        }
        //    }
        //}

        //public List<TestProject.ViewModel.Category> GetChannelList()
        //{
        //    return new TestProject.DAL.Category().GetList(null, 0, true, true, null);
        //}


        //public string GetRequestUrl(string key)
        //{
        //    return "?key=" + key;
        //}


        //public string Error()
        //{
        //    return "抱歉，您要访问的信息出现异常了";
        //}

        //public string GetDateLength(int month)
        //{
        //    if (month >= 12)
        //    {
        //        var last = month % 12;
        //        if (last == 0)
        //        {
        //            return string.Concat((month / 12).ToString(), "年");
        //        }
        //        else
        //        {
        //            if (last == 6)
        //            {
        //                return string.Concat((month / 12).ToString(), "年半");
        //            }
        //            else
        //            {
        //                return string.Concat((month / 12).ToString(), "年", last.ToString(), "个月");
        //            }
        //        }
        //    }
        //    else if (month == 6)
        //    {
        //        return string.Concat(month.ToString(), "半年");
        //    }
        //    else
        //    {
        //        return string.Concat(month.ToString(), "个月");
        //    }

        //}

        //public string Config(string value)
        //{
        //    return TestProject.Framework.ConfigManager.ReadValue("Site", value) ?? "0";
        //}

        ///// <summary>
        ///// 获取一个请求参数值
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public string Post(string key)
        //{
        //    return key;
        //}
        #endregion

    }
    #endregion
}
