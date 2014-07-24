/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Parser;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Context
    /// </summary>
    public class TemplateContext : ContextBase
    {
        /// <summary>
        /// Context
        /// </summary>
        public TemplateContext()
        {
            this.Charset = System.Text.Encoding.Default;
        }

        private String _currentPath;
        /// <summary>
        /// 当前资源路径
        /// </summary>
        public String CurrentPath
        {
            get { return _currentPath; }
            set { _currentPath = value; }
        }

        private Encoding _charset;
        /// <summary>
        /// 当前资源编码
        /// </summary>
        public Encoding Charset
        {
            get { return _charset; }
            set { _charset = value; }
        }

        /// <summary>
        /// 模板资源路径
        /// </summary>
        [Obsolete("请使用Resources.Paths 来替代本对象")]
        public List<String> Paths
        {
            get { return Resources.Paths; }
            private set
            {
                //if (!Resources.Paths.Contains(value))
                //{
                    Resources.Paths.AddRange(value);
                //}
            }
        }

        /// <summary>
        /// 从指定TemplateContext创建一个类似的实例
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static TemplateContext CreateContext(TemplateContext context)
        {
            TemplateContext ctx = (TemplateContext)context.Clone();
            ctx.TempData = new VariableScope(context.TempData);
            return ctx;
        }

    }
}