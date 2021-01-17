/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Caching;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Parsers;
using JinianNet.JNTemplate.Resources;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Context
    /// </summary>
    [Serializable]
    public class TemplateContext : Context
#if NET20 || NET40
        , ICloneable
#endif
    {
        private VariableScope variableScope;
        private ICache cache;
        private IActuator actuator;
        private IResourceLoader loader;
        private TagParser parsers;
        private bool enableTemplateCache;
        private List<System.Exception> errors;

        /// <summary>
        /// 模板上下文
        /// </summary>
        /// <param name="data">模板数据</param>
        /// <param name="actuator">动态访问器</param>
        /// <param name="loader">资源加载器</param>
        /// <param name="parsers">标签解析器</param> 
        /// <param name="cache">缓存</param>
        public TemplateContext(VariableScope data
            , IActuator actuator
            , IResourceLoader loader
            , TagParser parsers
            , ICache cache) : base()
        {
            this.actuator = actuator;
            this.loader = loader;
            this.parsers = parsers;
            this.cache = cache;
            this.variableScope = data ?? new VariableScope();
            this.errors = new List<System.Exception>();
            this.enableTemplateCache = Utility.StringToBoolean(Engine.Runtime.GetEnvironmentVariable("EnableTemplateCache"));

        }



        /// <summary>
        /// 启用模板缓存
        /// </summary>
        public bool EnableTemplateCache
        {
            get { return enableTemplateCache; }
            set { this.enableTemplateCache = value; }
        } 

        /// <summary>
        /// 模板数据
        /// </summary>
        public VariableScope TempData
        {
            get { return this.variableScope; }
            set { this.variableScope = value; }
        } 

        /// <summary>
        /// 当前异常集合（当ThrowExceptions为false时有效）
        /// </summary>
        public virtual System.Exception[] AllErrors
        {
            get { return this.errors.ToArray(); }
        }

        /// <summary>
        /// 获取当前第一个异常信息（当ThrowExceptions为false时有效）
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
            if (this.ThrowExceptions)
            {
                throw e;
            }
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
        /// 标签分析器
        /// </summary>
        public TagParser TagParser
        {
            get { return parsers; }
        }

        /// <summary>
        /// 加载器
        /// </summary>
        public IResourceLoader Loader
        {
            get { return loader; }
        }
        /// <summary>
        /// 缓存
        /// </summary>
        public Caching.ICache Cache
        {
            get { return cache; }
        }
        /// <summary>
        /// 动态调用代理
        /// </summary>
        public IActuator Actuator
        {
            get { return actuator; }
        }

        /// <summary>
        /// 从指定TemplateContext创建一个类似的实例
        /// </summary>
        /// <param name="context"></param>
        /// <returns>TemplateContext</returns>
        public static TemplateContext CreateContext(TemplateContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("\"context\" cannot be null.");
            }
            TemplateContext ctx = new TemplateContext(new VariableScope(context.TempData)
                , context.Actuator
                , context.Loader
                , context.TagParser
                , context.Cache
                );
            ctx.Charset = context.Charset;
            ctx.CurrentPath = context.CurrentPath;
            ctx.ThrowExceptions = context.ThrowExceptions;
            ctx.StripWhiteSpace = context.StripWhiteSpace;
            return ctx;
        }

        #region ICloneable 成员
        /// <summary>
        /// 浅克隆当前实例
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}