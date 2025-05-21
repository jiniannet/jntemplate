/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    public class TemplateCache : ResultCollection<ITemplateResult>, ITemplateCache
    {

    }

    /// <summary>
    /// thre template cache
    /// </summary>
    public interface ITemplateCache
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        ITemplateResult GetOrAdd(string key, Func<string, ITemplateResult> valueFactory);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        bool Remove(string text);


        /// <summary>
        /// Gets or sets the element at the specified name.
        /// </summary>
        /// <param name="name">The name of the element to get or set.</param>
        /// <returns>The element at the specified name.</returns>
        ITemplateResult this[string name] { get; set; }


        /// <summary>
        /// Removes all elements from the <see cref="ResultCollection{T}"/>.
        /// </summary>
        void Clear();
    }
}
