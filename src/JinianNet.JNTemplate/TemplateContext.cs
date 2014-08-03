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
            this.ThrowExceptions = true;
        }

        private String currentPath;
        /// <summary>
        /// 当前资源路径
        /// </summary>
        public String CurrentPath
        {
            get { return currentPath; }
            set { currentPath = value; }
        }

        private Encoding charset;
        /// <summary>
        /// 当前资源编码
        /// </summary>
        public Encoding Charset
        {
            get { return charset; }
            set { charset = value; }
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

        private bool throwErrors;

        /// <summary>
        /// 是否抛出异常(默认为true)
        /// </summary>
        public bool ThrowExceptions
        {
            get { return throwErrors; }
            set { throwErrors = value; }
        }

        //public virtual System.Exception[] AllErrors
        //{
        //    get
        //    {
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// 获取当前第一个异常信息
        ///// </summary>
        //public virtual System.Exception Error
        //{
        //    get
        //    {
        //        if (this.AllErrors.Length > 0)
        //        {
        //            return this.AllErrors[0];
        //        }

        //        return null;
        //    }
        //}

        /// <summary>
        /// 将异常添加到当前 异常集合中。
        /// </summary>
        /// <param name="e">异常</param>
        public void AddError(System.Exception e)
        {

        }

        /// <summary>
        /// 清除所有异常
        /// </summary>
        public void ClearError()
        {

        }

        /// <summary>
        /// 从指定TemplateContext创建一个类似的实例
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static TemplateContext CreateContext(TemplateContext context)
        {
            TemplateContext ctx = new TemplateContext();
            ctx.TempData = new VariableScope(context.TempData);
            ctx.Charset = context.Charset;
            ctx.CurrentPath = context.CurrentPath;
            ctx.ThrowExceptions = context.ThrowExceptions;
            return ctx;
        }
    }
}