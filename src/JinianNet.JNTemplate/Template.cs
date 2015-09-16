/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using JinianNet.JNTemplate.Parser;
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 模板实例类
    /// </summary>
    public class Template : TemplateRender,ITemplate
    {
        /// <summary>
        /// Template
        /// </summary>
        public Template()
            : this(null)
        {

        }

        /// <summary>
        /// Template
        /// </summary>
        /// <param name="text">模板内容</param>
        public Template(String text)
            : this(new TemplateContext(), text)
        {

        }

        /// <summary>
        /// Template
        /// </summary>
        /// <param name="ctx">TemplateContext 对象</param>
        /// <param name="text">模板内容</param>
        public Template(TemplateContext ctx, String text)
        {
            if (ctx == null)
            {
                throw new System.ArgumentNullException("ctx");
            }
            Context = ctx;
            TemplateContent = text;
        }

        /// <summary>
        /// 模板解析结果呈现
        /// </summary>
        /// <returns>String</returns>
        public String Render()
        {
            String document;

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
        public void Set(String key, Object value)
        {
            Context.TempData[key] = value;
        }

        /// <summary>
        /// 批量设置数据
        /// </summary>
        /// <param name="dic">字典</param>
        public void Set(Dictionary<String, Object> dic)
        {
            foreach (KeyValuePair<String, Object> value in dic)
            {
                Set(value.Key, value.Value);
            }
        }

    }
}