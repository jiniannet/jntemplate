/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Text;
using JinianNet.JNTemplate.Context;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class IncludeTag : SimpleTag
    {
        private Tag path;
        public Tag Path
        {
            get { return path; }
            set { path = value; }
        }

        private String LoadResource(Object path, TemplateContext context)
        {
            if (path != null)
            {
                if (String.IsNullOrEmpty(context.CurrentPath))
                {
                    return Resources.LoadResource(path.ToString(), context.Charset);
                }
                else
                {
                    return Resources.LoadResource(new String[] { context.CurrentPath }, path.ToString(), context.Charset);
                }
            }
            return null;
        }

        public override Object Parse(TemplateContext context)
        {
            Object path = this.Path.Parse(context);
            return LoadResource(path.ToString(), context);

        }

        public override Object Parse(object baseValue, TemplateContext context)
        {
            Object path = this.Path.Parse(baseValue, context);
            return LoadResource(path.ToString(), context);
        }
    }
}