/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Reflection;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TagVisitor<T> where T : ITag
    {
        private string name;
        /// <summary>
        /// 
        /// </summary>
        public TagVisitor()
        {
            name = typeof(T).Name;
        }
        /// <inheritdoc />
        public string Name => name; 
    }
}
