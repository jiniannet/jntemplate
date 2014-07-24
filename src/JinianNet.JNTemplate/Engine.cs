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
    /// <summary>
    /// 引擎
    /// </summary>
    public class Engine : IEngine
    {

        #region IEngine 成员

        private TemplateContext context;

        /// <summary>
        /// 创建 Engine 实例 
        /// </summary>
        public Engine()
            : this(null)
        {

        }

        /// <summary>
        /// 创建 Engine 实例 
        /// </summary>
        /// <param name="path">当前模板(主题)路径</param>
        /// <param name="encoding">编码</param>
        public Engine(String path, Encoding encoding)
            : this(new TemplateContext())
        {
            if (!String.IsNullOrEmpty(path) && !Resources.Paths.Contains(path))
            {
                Resources.Paths.Add(path);
            }
            context.Charset = encoding;
        }
        /// <summary>
        /// 创建 Engine 实例 
        /// </summary>
        /// <param name="ctx">TemplateContext</param>
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
        /// <summary>
        /// 创建 Template 实例
        /// </summary>
        /// <returns></returns>
        public ITemplate CreateTemplate()
        {
            return new Template(TemplateContext.CreateContext(context), null);
        }
        /// <summary>
        /// 根据指定路径创建Template实例
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public ITemplate CreateTemplate(String path)
        {
            return CreateTemplate(path, context.Charset);
        }

        /// <summary>
        /// 根据指定路径创建Template实例
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
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
                    if (Resources.FindPath(path, out fullPath) == -1)
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
