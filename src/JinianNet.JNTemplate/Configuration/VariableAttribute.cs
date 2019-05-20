/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Configuration
{
    /// <summary>
    /// 环境变量配置属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class VariableAttribute : System.Attribute
    {
        /// <summary>
        /// 对应名称
        /// </summary>
        /// <value></value>
        public string Name { get; set; }
        /// <summary>
        /// 变量类型
        /// </summary>
        public VariableType Type { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public VariableAttribute() : this(null, VariableType.Environment)
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="type">类型</param>
        public VariableAttribute(string name, VariableType type)
        {
            this.Name = name;
            this.Type = type;
        }
    }
}