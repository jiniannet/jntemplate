/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Parser;
using JinianNet.JNTemplate.Configuration;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Context
    /// </summary>
    public class TemplateContext : ICloneable
    {
        private VariableScope variableScope;
        private String currentPath;
        private Encoding charset;
        private Boolean throwErrors;
        private Boolean stripWhiteSpace;
        private List<System.Exception> errors;

        /// <summary>
        /// 模板上下文
        /// </summary>
        public TemplateContext()
            : this(new VariableScope())
        {

        }

        /// <summary>
        /// 模板上下文
        /// </summary>
        /// <param name="data">数据</param>
        public TemplateContext(VariableScope data)
        {
            if (data == null)
            {
                throw new ArgumentException("data");
            };
        }
        /// <summary>
        /// 配置模板
        /// </summary>
        /// <param name="config"></param>
        protected void Configure(ITemplateConfig config)
        {
            this.ThrowExceptions = config.ThrowExceptions;
            this.StripWhiteSpace = config.StripWhiteSpace;
            this.errors = new List<System.Exception>();
        }

        /// <summary>
        /// 处理标签前后空格
        /// </summary>
        public Boolean StripWhiteSpace
        {
            get { return stripWhiteSpace; }
            set { stripWhiteSpace = value; }
        }


        /// <summary>
        /// 模板数据
        /// </summary>
        public VariableScope TempData
        {
            get { return variableScope; }
            set { variableScope = value; }
        }

        /// <summary>
        /// 当前资源路径
        /// </summary>
        public String CurrentPath
        {
            get { return currentPath; }
            set { currentPath = value; }
        }

        /// <summary>
        /// 当前资源编码
        /// </summary>
        public Encoding Charset
        {
            get { return charset; }
            set { charset = value; }
        }


        /// <summary>
        /// 是否抛出异常(默认为true)
        /// </summary>
        public Boolean ThrowExceptions
        {
            get { return throwErrors; }
            set { throwErrors = value; }
        }

        public virtual System.Exception[] AllErrors
        {
            get { return errors.ToArray(); }
        }

        /// <summary>
        /// 获取当前第一个异常信息
        /// </summary>
        public virtual System.Exception Error
        {
            get
            {
                if (this.AllErrors.Length > 0)
                {
                    return this.AllErrors[0];
                }

                return null;
            }
        }

        /// <summary>
        /// 将异常添加到当前 异常集合中。
        /// </summary>
        /// <param name="e">异常</param>
        public void AddError(System.Exception e)
        {
            this.errors.Add(e);
        }

        /// <summary>
        /// 清除所有异常
        /// </summary>
        public void ClearError()
        {
            this.errors.Clear();
        }

        /// <summary>
        /// 从指定TemplateContext创建一个类似的实例
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static TemplateContext CreateContext(TemplateContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            TemplateContext ctx = new TemplateContext();
            ctx.TempData = new VariableScope(context.TempData);
            ctx.Charset = context.Charset;
            ctx.CurrentPath = context.CurrentPath;
            ctx.ThrowExceptions = context.ThrowExceptions;
            return ctx;
        }

        #region ICloneable 成员
        /// <summary>
        /// 浅克隆当前实例
        /// </summary>
        /// <returns></returns>
        public Object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}