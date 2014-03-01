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

namespace JinianNet.JNTemplate
{
    public class Engine : IEngine
    {

        #region IEngine 成员

        [Obsolete("模板初始数据，请直接在TemplateContext中配置该属性！")]
        public VariableScope VariableScope
        {
            get { return _ctx.TempData; }
            private set { _ctx.TempData = value; }
        }

        [Obsolete("模板路径，请直接在TemplateContext中配置该属性！")]
        public String CurrentPath
        {
            get { return _ctx.CurrentPath; }
            set { _ctx.CurrentPath = value; }
        }

        [Obsolete("默认模板编码，请直接在TemplateContext中配置该属性！")]
        public Encoding Charset
        {
            get { return _ctx.Charset; }
            set { _ctx.Charset = value; }
        }

        private TemplateContext _ctx;


        public Engine()
            : this(null)
        {

        }

        public Engine(String path, Encoding charset)
            : this(new TemplateContext())
        {
            if (!string.IsNullOrEmpty(path))
            {
                _ctx.Paths.Add(path);
            }
            _ctx.Charset = charset;
        }

        public Engine(TemplateContext context)
        {
            if (context == null)
            {
                _ctx = new TemplateContext();
            }
            else
            {
                _ctx = context;
            }
        }

        public ITemplate CreateTemplate()
        {
            return new Template(TemplateContext.CreateContext(_ctx), null);
        }

        public ITemplate CreateTemplate(String path)
        {
            return CreateTemplate(path,_ctx.Charset);
        }

        public ITemplate CreateTemplate(String path, Encoding encoding)
        {
            Template template = new Template(TemplateContext.CreateContext(_ctx), null);
            if (encoding != null)
            {
                template.Context.Charset = encoding;
            }
            if (!string.IsNullOrEmpty(path))
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

                template.TemplateContent = Resources.Load(fullPath, template.Context.Charset);
            }

            return template;
        }

        #endregion
    }
}
