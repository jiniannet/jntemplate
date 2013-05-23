using System;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;

namespace JinianNet.JNTemplate.Demo
{
    public class HttpModule : System.Web.IHttpModule
    {
        #region IHttpModule 成员

        public void Dispose()
        {

        }

        public void Init(System.Web.HttpApplication context)
        {
            context.BeginRequest += new EventHandler(BeginRequest);
        }

        private void BeginRequest(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;
            if (context.Request.Url.AbsolutePath.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
            {
                string[] urls;

                if (context.Request.Url.Segments.Length == 1)
                {
                    urls = new string[] { "Default.aspx" };
                }
                else
                {
                    urls = context.Request.Url.AbsolutePath.Trim('/').Split('/');
                }
                if (urls.Length == 1)
                {
                    JinianNet.JNTemplate.ITemplate template;
                    if (urls.Length == 1)
                    {
                        string path = context.Server.MapPath(string.Concat("~/templets/", urls[0].Substring(0, urls[0].LastIndexOf('.')), ".html"));
                        if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
                        {
                            template = JNTemplate.BuildManager.CreateTemplate(path);
                            //template.Context.TempData["cmd"] = new TemplateFunction(context);
                            template.Context.TempData["cmd"] = new Command(context);
                            template.Render(context.Response.Output);
                            context.Response.End();
                        }
                    }
                }
            }
        }


        public void Application_OnError(Object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;
            context.Response.Write("<html><body>");
            context.Response.Write("<h4>Code Error:</h4>");
            context.Response.Write("<div style=\"width:80%; height:200px; word-break:break-all\">");
            context.Response.Write(System.Web.HttpUtility.HtmlEncode(context.Server.GetLastError().ToString()));
            context.Response.Write("</div>");
            context.Response.Write("</body></html>");
            context.Response.End();

        }

        #endregion
    }

}
