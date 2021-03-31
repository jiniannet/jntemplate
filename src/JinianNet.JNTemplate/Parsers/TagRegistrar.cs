/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The base registrar
    /// </summary>
    public abstract class TagRegistrar<T> : IRegistrar
        where T:ITag
    {

        /// <inheritdoc />
        public virtual void Regiser(IEngine engine)
        {
            var parseMethod = BuildParseMethod();
            if (parseMethod != null)
            {
                engine.Register(parseMethod, -1);
            }

            var compileMethod = BuildCompileMethod();
            if (compileMethod != null)
            {
                engine.Register<T>(compileMethod);
            }

            var guessMethod = BuildGuessMethod();
            if (guessMethod != null)
            {
                engine.Register<T>(guessMethod);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Func<TemplateParser, TokenCollection,ITag> BuildParseMethod();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Func<ITag, CompileContext, MethodInfo> BuildCompileMethod();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Func<ITag, CompileContext, Type> BuildGuessMethod();
    }
}