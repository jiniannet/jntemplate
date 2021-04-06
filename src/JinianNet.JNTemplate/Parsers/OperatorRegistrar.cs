/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Nodes;
using System.Reflection;
using System.Reflection.Emit;
using System;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The <see cref="OperatorRegistrar"/> registrar
    /// </summary>
    public class OperatorRegistrar : TagRegistrar<OperatorTag>, IRegistrar
    {
        /// <inheritdoc />
        public override Func<TemplateParser, TokenCollection, ITag> BuildParseMethod()
        {
            return null;
        }
        /// <inheritdoc />
        public override Func<ITag, CompileContext, MethodInfo> BuildCompileMethod()
        {
            return null;
        }
        /// <inheritdoc />
        public override Func<ITag, CompileContext, Type> BuildGuessMethod()
        {
            return null;
        }
        /// <inheritdoc />
        public override Func<ITag, TemplateContext, object> BuildExcuteMethod()
        {
            return (tag, context) =>
            {
                var t = tag as OperatorTag;
                return t.Value;
            };
        }
    }
}