/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
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
#if NET20 || NET40
        , ICloneable
#endif
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateContext"/> class
        /// </summary>
        /// <param name="data">The <see cref="VariableScope"/>.</param>  
        internal TemplateContext(VariableScope data) : base()
        {
            this.TempData = data;
            this.AllErrors = new List<System.Exception>();
        }

        /// <summary>
        /// Gets the <see cref="ExecutorBuilder"/>
        /// </summary>
        public ExecutorBuilder ExecutorBuilder => Options.ExecutorBuilder;

        /// <summary>
        /// Enable or Disenable the cache.
        /// </summary>
        public bool EnableTemplateCache { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TemplateContext"/> of the context.
        /// </summary>
        public VariableScope TempData { get; set; }

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
            TemplateContext ctx = new TemplateContext(new VariableScope(context.TempData,context.Options.TypeDetectPattern));
            ctx.Options = context.Options;
            ctx.Charset = context.Charset;
            ctx.CurrentPath = context.CurrentPath;
            ctx.ThrowExceptions = context.ThrowExceptions;
            ctx.StripWhiteSpace = context.StripWhiteSpace;
            return ctx;
        }
    }
}