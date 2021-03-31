/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.CodeCompilation
{
    /// <summary>
    /// 
    /// </summary>
    public class CompileParameter
    {
        /// <summary>
        /// 
        /// </summary>
        public ITag Tag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ILGenerator ILGenerator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Type ReturnType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, object> Properties { get; set; }
    }
}
