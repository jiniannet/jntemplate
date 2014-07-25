using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using JinianNet.JNTemplate;

namespace example
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            string path;
            if (System.Web.HttpRuntime.AppDomainAppPath.EndsWith("\\"))
                path = string.Concat(System.Web.HttpRuntime.AppDomainAppPath, "templets\\Green\\");
            else
                path = string.Concat(System.Web.HttpRuntime.AppDomainAppPath, "\\templets\\Green\\");

            //设定资源路径(原1.1中是TemplateContext.Paths)
            Resources.Paths.Add(path);

            //设置基本数据 在这里配置好，无须每个页面再配置
            TemplateContext ctx = new TemplateContext();
            ctx.TempData.Push("Site",new { 
                Name = "jntemplate", //网站名称
                Title = "jntemplate 演示站点",//首页TITLE
                Keywords = "jntemplate",//首页Keywords
                Description = "asp.net 开源模板引擎", //首页Description
                Url = "/",//网站URL
                TemplateUrl = "/templets/Green/"
            });

            Engine engine = new Engine(ctx);

            BuildManager.Engines.Add(engine);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}