/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Context for template execution.
    /// </summary>
    [Serializable]
    public class TemplateContext : Context
#if NF20 || NF40
        , ICloneable
#endif
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateContext"/> class
        /// </summary>
        /// <param name="data">The <see cref="IVariableScope"/>.</param>  
        /// <param name="hostEnvironment"></param> 
        internal TemplateContext(IVariableScope data,
            IHostEnvironment hostEnvironment)
            : base(hostEnvironment)
        {
            this.TempData = data;
            this.AllErrors = new List<Exception>();
        }

        /// <summary>
        /// Enable or Disenable the cache.
        /// </summary>
        [Obsolete("please use the `EnableCache`")]
        public bool EnableTemplateCache { get => EnableCache; set => EnableCache = value; }

        /// <summary>
        /// Gets or sets the <see cref="IVariableScope"/> of the context.
        /// </summary>
        public IVariableScope TempData { get; set; }

        /// <summary>
        /// Gets the <see cref="Exception"/> of the context.
        /// </summary>
        public virtual List<System.Exception> AllErrors { get; set; }

        /// <summary>
        /// Gets the first <see cref="Exception"/>.
        /// </summary>
        public virtual System.Exception Error => AllErrors.FirstOrDefault();

        /// <summary>
        /// Adds an <see cref="Exception"/> to the end of the context.
        /// </summary>
        /// <param name="e">The <see cref="Exception"/>.</param>
        public void AddError(System.Exception e)
        {
            if (this.ThrowExceptions)
            {
                throw e;
            }
            this.AllErrors.Add(e);
        }

        /// <summary>
        /// Removes all <see cref="Exception"/> from context.
        /// </summary>
        public void ClearError()
        {
            this.AllErrors.Clear();
        }

        /// <summary>
        /// Creates a shallow copy of the <see cref="TemplateContext"/>.
        /// </summary>
        /// <returns>A shallow copy of the current <see cref="TemplateContext"/>.</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Creates an instance from the specified <see cref="TemplateContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="TemplateContext"/>.</param>
        /// <returns>A new instance.</returns>
        public static TemplateContext CreateContext(TemplateContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("\"context\" cannot be null.");
            }
            var scope = context.CreateVariableScope(context.TempData);
            var ctx = new TemplateContext(scope, context.Environment);
            ctx.Charset = context.Charset;
            ctx.CurrentPath = context.CurrentPath;
            ctx.ThrowExceptions = context.ThrowExceptions;
            ctx.OutMode = context.OutMode;
            ctx.EnableCache = context.EnableCache;
            return ctx;
        }
    }
}