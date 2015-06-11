/*****************************************************
   Copyright (c) 2013-2015 jiniannet (http://www.jiniannet.com)

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

   Redistributions of source code must retain the above copyright notice
 *****************************************************/
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
        private ITemplateConfiguration config;
        private String currentPath;
        private Encoding charset;
        private Boolean throwErrors;

        /// <summary>
        /// 模板上下文
        /// </summary>
        public TemplateContext()
            : this(new DefaultConfiguration())
        {

        }

        /// <summary>
        /// 模板上下文
        /// </summary>
        /// <param name="config"></param>
        public TemplateContext(ITemplateConfiguration config)
        {
            this.Charset = System.Text.Encoding.Default;
            this.ThrowExceptions = true;
            this.config = config;
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
        /// 模板资源路径
        /// </summary>
        [Obsolete("请使用Config.Paths 来替代本对象")]
        public ICollection<String> Paths
        {
            get { return Config.Paths; }
        }

        /// <summary>
        /// 是否抛出异常(默认为true)
        /// </summary>
        public Boolean ThrowExceptions
        {
            get { return throwErrors; }
            set { throwErrors = value; }
        }

        /// <summary>
        /// 模板配置数据
        /// </summary>
        public ITemplateConfiguration Config
        {
            get { return config; }
            set { config = value; }
        }

        //public virtual System.Exception[] AllErrors
        //{
        //    get
        //    {
        //        return null;       //功能暂未实现
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
            //功能暂未实现
        }

        /// <summary>
        /// 清除所有异常
        /// </summary>
        public void ClearError()
        {
            //功能暂未实现
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