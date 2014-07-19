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
    public class TemplateContext : ContextBase
    {
        public TemplateContext()
        {
            this.Charset = System.Text.Encoding.Default;
            this.Paths = new List<String>();
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

        //private List<ITagParser> _parser;
        //public List<ITagParser> Parser
        //{
        //    get { return _parser; }
        //    private set { _parser = value; }
        //}

        private List<String> _paths;
        [Obsolete("请使用Resources.Paths 来替代本对象")]
        /// <summary>
        /// 模板搜索路径
        /// </summary>
        public List<String> Paths
        {
            get { return _paths; }
            private set { _paths = value; }
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