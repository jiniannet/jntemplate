/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using System;
using System.IO;
using System.Threading.Tasks;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Extensions methods for <see cref="ITemplate"/>.
    /// </summary>
    public static class TemplateExtensions
    {

        /// <summary>
        /// Renders the template.
        /// </summary>
        /// <param name="template">The <see cref="ITemplate"/>.</param>
        /// <returns>The template contents.</returns>
        public static string Render(this ITemplate template)
        {
            string document;

            using (var writer = new StringWriter())
            {
                template.Render(writer);
                document = writer.ToString();
            }

            return document;
        }
        /// <summary>
        /// Set a new value for variables.
        /// </summary>
        /// <param name="template">The <see cref="ITemplate"/>.</param>
        /// <param name="key">The key of the element to get</param> 
        /// <param name="value">The element with the specified key.</param>
        public static void Set<T>(this ITemplate template, string key, T value)
        {
            template.Context.TempData.Set<T>(key, value);
        }

        /// <summary>
        /// Set a static type for variables.
        /// </summary>
        /// <param name="template">The <see cref="ITemplate"/>.</param>
        /// <param name="key">The key of the element to get</param> 
        /// <param name="type">The type with the specified key.</param>
        public static void SetStaticType(this ITemplate template, string key, Type type)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = type.Name;
            }
            template.Context.TempData.Set(key, null, type);
        }


        /// <summary>
        /// Set a anonymous object for variables.
        /// </summary>
        /// <param name="template">The <see cref="ITemplate"/>.</param>
        /// <param name="key">The key of the element to get</param> 
        /// <param name="value">The value with the specified key.</param>
        public static void SetAnonymousObject(this ITemplate template, string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (template.EnableCompile)
            {
                var anonymousType = value.GetType();
                var typeName = $"{Const.ANONYMOUS_TYPE_CACHE}{anonymousType.GetHashCode()}";
                var type = template.Context.Cache.Get<Type>(typeName);
                if (type == null)
                {
                    type = ObjectBuilder.GenerateTypeFrom(anonymousType);
                    template.Context.Cache.Set(typeName, type);
                }
                var newObj = ObjectBuilder.FromAnonymousObject(value, type);
                template.Context.TempData.Set(key, newObj, type);
            }
            else
            {
                template.Context.TempData.Set(key, value, null);
            }
        }

#if !NF40 && !NF45
        /// <summary>
        /// Renders the template.
        /// </summary>
        /// <param name="template">The <see cref="ITemplate"/>.</param>
        /// <returns>The template contents.</returns>
        public static async Task<string> RenderAsync(this ITemplate template)
        {
            string document;

            using (var writer = new StringWriter())
            {
                await template.RenderAsync(writer);
                document = writer.ToString();
            }

            return document;
        }
#endif
    }
}
