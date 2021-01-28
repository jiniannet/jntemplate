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
    /// Template Extensions
    /// </summary>
    public static class TemplateExtensions
    {

        /// <summary>
        /// 模板解析结果呈现
        /// </summary>
        /// <param name="template">template</param>
        /// <returns>string</returns>
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
