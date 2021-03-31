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
    /// The compilation method builder 
    /// </summary>
    public partial class CompileBuilder
    {
        Dictionary<string, Func<ITag, CompileContext, MethodInfo>> returnDict;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompileBuilder"/> class
        /// </summary>
        public CompileBuilder()
        {
            returnDict = new Dictionary<string, Func<ITag, CompileContext, MethodInfo>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Register a rendering method.
        /// </summary>
        /// <typeparam name="T">Type type of the tag.</typeparam>
        /// <param name="func">The method.</param>
        public void Register<T>(Func<ITag, CompileContext, MethodInfo> func) where T : ITag
        {
            Register(typeof(T).Name, func);
        }
        /// <summary>
        /// Register a rendering method.
        /// </summary>
        /// <param name="name">The name of the tag.</param>
        /// <param name="func">The method.</param>
        public void Register(string name, Func<ITag, CompileContext, MethodInfo> func)
        {
            returnDict[name] = func;
        }
        /// <summary>
        /// Build a method with has return value
        /// </summary>
        /// <param name="name">The name of the tag.</param>
        /// <returns></returns>
        public Func<ITag, CompileContext, MethodInfo> Build(string name)
        {
            if (returnDict.TryGetValue(name, out var func))
            {
                return func;
            }
            throw new Exception.CompileException($"The tag \"{name}\" is not supported .");
        }

        /// <summary>
        /// Build a method with has return value
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public Func<ITag, CompileContext, MethodInfo> Build(ITag tag)
        {
            return Build(tag.GetType().Name);
        }

        /// <summary>
        /// Build a method with has return value
        /// </summary>
        /// <typeparam name="T">Type type of the tag.</typeparam>
        /// <returns></returns>
        public Func<ITag, CompileContext, MethodInfo> Build<T>()
            where T : ITag
        {
            return Build(typeof(T).Name);
        }
    }
}