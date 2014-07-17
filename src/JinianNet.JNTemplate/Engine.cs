/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/

using System;
using System.Text;
using System.Collections.Generic;
using JinianNet.JNTemplate.Parser;
using JinianNet.JNTemplate.Context;

namespace JinianNet.JNTemplate
{
    public class Engine : IEngine
    {

        #region IEngine 成员

        private TemplateContext context;


        public Engine()
            : this(null)
        {

        }

        public Engine(String path, Encoding charset)
            : this(new TemplateContext())
        {
            if (!String.IsNullOrEmpty(path))
            {
                context.Paths.Add(path);
            }
            context.Charset = charset;
        }

        public Engine(TemplateContext ctx)
        {
            if (ctx == null)
            {
                context = new TemplateContext();
            }
            else
            {
                context = ctx;
            }
        }

        public ITemplate CreateTemplate()
        {
            return new Template(TemplateContext.CreateContext(context), null);
        }

        public ITemplate CreateTemplate(String path)
        {
            return CreateTemplate(path, context.Charset);
        }

        public ITemplate CreateTemplate(String path, Encoding encoding)
        {
            TemplateContext ctx = TemplateContext.CreateContext(context);
            Template template = new Template(ctx, null);
            if (encoding != null)
            {
                template.Context.Charset = encoding;
            }
            if (!String.IsNullOrEmpty(path))
            {
                String fullPath = path;
                Int32 index = fullPath.IndexOf(System.IO.Path.VolumeSeparatorChar);
                if (index == -1)
                {
                    if (Resources.FindPath(template.Context.Paths.ToArray(), path, out fullPath) == -1)
                    {
                        return template;
                    }
                }
                ctx.CurrentPath = System.IO.Path.GetDirectoryName(fullPath);
                template.TemplateContent = Resources.Load(fullPath, template.Context.Charset);
            }

            return template;
        }
        #endregion
    }
}
