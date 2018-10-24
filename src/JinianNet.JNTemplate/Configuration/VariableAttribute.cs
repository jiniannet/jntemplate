/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Configuration
{
    /// <summary>
    /// 变量标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class VariableAttribute : System.Attribute
    {
        public string Name { get; set; }
        public VariableAttribute()
        {

        }
        public VariableAttribute(string name)
        {
            this.Name = name;
        }
    }
}