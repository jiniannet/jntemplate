/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using System;
using System.IO;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// The base class of template.
    /// </summary>
    public abstract class TemplateBase
    {
        /// <summary>
        /// Gets or sets the key(unique) of the template.
        /// </summary>
        public string TemplateKey
        {
            get => this.Context?.Name;
            set => this.Context.Name = value;
        }

        /// <summary>
        /// Gets or sets the context of the template.
        /// </summary>
        public TemplateContext Context { get; set; }


        /// <summary>
        /// Set a new value for variables.
        /// </summary>
        /// <param name="key">The key of the element to get</param> 
        /// <param name="value">The element with the specified key.</param>
        public void Set<T>(string key, T value)
        {
            this.Context.Set(key, value, typeof(T));
        }
    }
}
