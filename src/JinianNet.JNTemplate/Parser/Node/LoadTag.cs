/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Text;
using JinianNet.JNTemplate.Context;
using System.IO;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class LoadTag : BlockTag
    {
        private Tag path;
        public Tag Path
        {
            get { return path; }
            set { path = value; }
        }

        public override Object Parse(TemplateContext context)
        {
            Object path = this.Path.Parse(context);
            LoadResource(path, context);
            return base.Parse(context);
        }

        public override Object Parse(Object baseValue, TemplateContext context)
        {
            Object path = this.Path.Parse(baseValue, context);
            LoadResource(path, context);
            return base.Parse(context);
        }

        public override void Parse(TemplateContext context, TextWriter write)
        {
            Object path = this.Path.Parse(context);
            LoadResource(path, context);
            base.Parse(context, write);
        }

        private void LoadResource(Object path, TemplateContext context)
        {
            if (path != null)
            {
                if (String.IsNullOrEmpty(context.CurrentPath))
                {
                    this.TemplateContent = Resources.LoadResource(path.ToString(), context.Charset);
                }
                else
                {
                    this.TemplateContent = Resources.LoadResource(new String[] { context.CurrentPath }, path.ToString(), context.Charset);
                }
            }
        }
    }
}