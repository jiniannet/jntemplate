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

#if NET20 || NET40
        /// <summary>
        /// Template
        /// </summary>
        public Template()
            : this(null)
        {

        }
#endif
        /// <summary>
        /// Template
        /// </summary>
        /// <param name="text">模板内容</param>
        public Template(string text)
            : this(Engine.CreateContext(), text)
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

        /// <summary>
        /// 模板解析结果呈现
        /// </summary>
        /// <returns>string</returns>
        public string Render()
        {
            string document;

            using (StringWriter writer = new StringWriter())
            {
                Render(writer);
                document = writer.ToString();
            }

            return document;
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Set(string key, object value)
        {
            Context.TempData[key] = value;
        }

        /// <summary>
        /// 批量设置数据
        /// </summary>
        /// <param name="dic">字典</param>
        public void Set(Dictionary<string, object> dic)
        {
            foreach (KeyValuePair<string, object> value in dic)
            {
                Set(value.Key, value.Value);
            }
        }

    }
}