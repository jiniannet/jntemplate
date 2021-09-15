﻿/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.CodeCompilation
{
    /// <summary>
    /// The compilation context .
    /// </summary>
    public class CompileContext : Context, IDisposable
    {
        private int seed = 0;
        /// <summary>
        /// Initializes a new instance of the <see cref="CompileContext"/> class
        /// </summary>
        /// <param name="hostEnvironment"></param> 
        public CompileContext(IHostEnvironment hostEnvironment)
            : base(hostEnvironment)
        {
            Methods = new Dictionary<string, MethodInfo>(StringComparer.OrdinalIgnoreCase);
        }
        /// <summary>
        /// Unique key of the template
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///  Gets or sets the <see cref="TypeBuilder"/> of the context.
        /// </summary>
        public TypeBuilder TypeBuilder { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="ILGenerator"/> of the context.
        /// </summary>
        public ILGenerator Generator { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="IVariableScope"/> of the context.
        /// </summary>
        public IVariableScope Data { get; set; }
        /// <summary>
        /// Used to cache some compiled methods . 
        /// </summary>
        public Dictionary<string, MethodInfo> Methods { get; set; }

        /// <summary>
        /// Gets the <see cref="CompileBuilder"/>
        /// </summary>
        public CompileBuilder CompileBuilder  => Environment.Builder;

        /// <summary>
        /// Gets the <see cref="CompileBuilder"/>
        /// </summary>
        public TypeGuesser TypeGuesser => Environment.Guesser;

        /// <summary>
        /// Set the type of compilation parameters 
        /// </summary>
        /// <param name="name">Unique key of the variable.</param>
        /// <param name="type">The type of the variable.</param>
        public void Set(string name, Type type)
        {
            Data.Set(name, null, type);
        }

        /// <summary>
        /// Set the type of compilation parameters 
        /// </summary>
        /// <typeparam name="T">The type of the tag.</typeparam>
        /// <param name="name">Unique key of the variable.</param>
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
        /// The seed  of the increase .
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
