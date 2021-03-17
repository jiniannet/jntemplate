/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;

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
    }
}
