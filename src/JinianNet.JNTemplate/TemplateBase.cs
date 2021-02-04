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
    /// 基本模板
    /// </summary>
    public abstract class TemplateBase 
    {
        /// <summary>
        /// 模板KEY(用于缓存，默认为文路径)
        /// </summary>
        public string TemplateKey { get; set; }

        /// <summary>
        /// 模板上下文
        /// </summary>
        public TemplateContext Context { get; set; }

        /// <summary>
        /// 模板内容
        /// </summary>
        public string TemplateContent { get; set; }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Set<T>(string key, T value)
        {
            Context.TempData.Set<T>(key, value);
        }

        /// <summary>
        /// 设置静态对象
        /// </summary>
        /// <param name="key">对象名</param>
        /// <param name="type">类型</param>
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
