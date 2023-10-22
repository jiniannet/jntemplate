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
    public interface ITagVisitor : ITagParser
    {
        /// <summary>
        /// 
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        MethodInfo Compile(ITag tag, CompileContext context);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Type GuessType(ITag tag, CompileContext context);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        object Excute(ITag tag, TemplateContext context);
    }
}
