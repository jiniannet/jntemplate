/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Compile
{
    /// <summary>
    /// Compile Context
    /// </summary>
    public class CompileContext : Context, IDisposable
    {
        private int seed = 0;
        /// <summary>
        /// ctox
        /// </summary>
        public CompileContext()
            : base()
        {
            Methods = new Dictionary<string, MethodInfo>();
        }
        /// <summary>
        /// template name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Type builder
        /// </summary>
        public TypeBuilder TypeBuilder { get; set; }
        /// <summary>
        /// IL Generator
        /// </summary>
        public ILGenerator Generator { get; set; }
        /// <summary>
        /// data
        /// </summary>
        public VariableScope Data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, MethodInfo> Methods { get; set; }

        /// <summary>
        /// 设置编译参数类型
        /// </summary>
        /// <param name="name">key</param>
        /// <param name="type">type</param>
        public void Set(string name, Type type)
        {
            Data.SetElement(name, new VariableElement(type, null));
        }

        /// <summary>
        /// 设置编译参数类型
        /// </summary>
        /// <param name="name">key</param>
        public void Set<T>(string name)
        {
            Set(name, typeof(T));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Methods?.Clear();
            TypeBuilder = null;
            Data = null;
            Methods = null;
            Generator = null;
        }

        /// <summary>
        /// seed 
        /// </summary>
        internal int Seed
        {
            get
            {
                if (seed == int.MaxValue)
                {
                    seed = 0;
                }
                return seed++;
            }
        }
    }
}
