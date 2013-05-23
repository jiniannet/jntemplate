using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace JinianNet.JNTemplate.Demo
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            string path;
            if (System.Web.HttpRuntime.AppDomainAppPath.EndsWith("\\"))
                path = string.Concat(System.Web.HttpRuntime.AppDomainAppPath, "templets\\");
            else
                path = string.Concat(System.Web.HttpRuntime.AppDomainAppPath, "\\templets\\");
            JinianNet.JNTemplate.Engine engine = new JNTemplate.Engine(path, System.Text.Encoding.UTF8);
            JinianNet.JNTemplate.BuildManager.Engines.Clear();
            JinianNet.JNTemplate.BuildManager.Engines.Add(engine);
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