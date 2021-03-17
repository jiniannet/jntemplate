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
    /// Context for template execution.
    /// </summary>
    [Serializable]
    public class TemplateContext : Context
#if NET20 || NET40
        , ICloneable
#endif
    {
        private VariableScope variableScope;
        private bool enableTemplateCache;
        private List<System.Exception> errors;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateContext"/> class
        /// </summary>
        /// <param name="data">The <see cref="VariableScope"/>.</param>  
        public TemplateContext(VariableScope data) : base()
        {
            this.variableScope = data ?? new VariableScope(null);
            this.errors = new List<System.Exception>();
            this.enableTemplateCache = Runtime.Storage.EnableTemplateCache;
        }

        /// <summary>
        /// Enable or Disenable the cache.
        /// </summary>
        public bool EnableTemplateCache
        {
            get { return enableTemplateCache; }
            set { this.enableTemplateCache = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="TemplateContext"/> of the context.
        /// </summary>
        public VariableScope TempData
        {
            get { return this.variableScope; }
            set { this.variableScope = value; }
        }

        /// <summary>
        /// Gets the <see cref="Exception"/> of the context.
        /// </summary>
        public virtual System.Exception[] AllErrors
        {
            get { return this.errors.ToArray(); }
        }

        /// <summary>
        /// Gets the first <see cref="Exception"/>.
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
        /// Adds an <see cref="Exception"/> to the end of the context.
        /// </summary>
        /// <param name="e">The <see cref="Exception"/>.</param>
        public void AddError(System.Exception e)
        {
            if (this.ThrowExceptions)
            {
                throw e;
            }
            this.errors.Add(e);
        }

        /// <summary>
        /// Removes all <see cref="Exception"/> from context.
        /// </summary>
        public void ClearError()
        {
            this.errors.Clear();
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
            TemplateContext ctx = new TemplateContext(new VariableScope(context.TempData));
            ctx.Charset = context.Charset;
            ctx.CurrentPath = context.CurrentPath;
            ctx.ThrowExceptions = context.ThrowExceptions;
            ctx.StripWhiteSpace = context.StripWhiteSpace;
            return ctx;
        }

        /// <summary>
        /// Creates a shallow copy of the <see cref="TemplateContext"/>.
        /// </summary>
        /// <returns>A shallow copy of the current <see cref="TemplateContext"/>.</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}