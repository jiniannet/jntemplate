/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Configuration
{
    /// <summary>
    /// 配置属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PropertyAttribute : System.Attribute
    {
        /// <summary>
        /// 对应名称
        /// </summary>
        /// <value></value>
        public string Name { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public PropertyAttribute()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        public PropertyAttribute(string name)
        {
            this.Name = name;
        }
    }
}