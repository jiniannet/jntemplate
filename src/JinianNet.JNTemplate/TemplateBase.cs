/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Dynamic;
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
        public string TemplateKey { get; set; }

        /// <summary>
        /// Gets or sets the context of the template.
        /// </summary>
        public TemplateContext Context { get; set; }

        /// <summary>
        /// Gets or sets the content of the template.
        /// </summary>
        public string TemplateContent { get; set; }

        /// <summary>
        /// Set a new value for variables.
        /// </summary>
        /// <param name="key">The key of the element to get</param> 
        /// <param name="value">The element with the specified key.</param>
        public void Set<T>(string key, T value)
        {
            Context.TempData.Set<T>(key, value);
        }

        /// <summary>
        /// Set a static type for variables.
        /// </summary>
        /// <param name="key">The key of the element to get</param> 
        /// <param name="type">The type with the specified key.</param>
        public void SetStaticType(string key, Type type)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = type.Name;
            }
            Context.TempData.Set(key, null, type);
        }
    }
}
