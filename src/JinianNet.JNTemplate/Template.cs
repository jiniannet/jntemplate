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
    /// 模板实例类
    /// </summary>
    public class Template : TemplateRender, ITemplate
    {
        /// <summary>
        /// Template
        /// </summary>
        public Template()
            : this(Engine.CreateContext(), string.Empty)
        {

        }

        /// <summary>
        /// Template
        /// </summary>
        /// <param name="ctx">TemplateContext 对象</param>
        /// <param name="text">模板内容</param>
        public Template(TemplateContext ctx, string text)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException("\"ctx\" cannot be null.");
            }
            Context = ctx;
            TemplateContent = text;
        }
    }
}