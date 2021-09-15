/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Hosting;
using JinianNet.JNTemplate.Nodes;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The base registrar
    /// </summary>
    public abstract class TagRegistrar<T> : IRegistrar
        where T : ITag
    {

        /// <inheritdoc />
        public virtual void Regiser(IHost host)
        {
            var parseMethod = BuildParseMethod();
            if (parseMethod != null)
            {
                host.RegisterParseFunc(parseMethod, -1);
            }
            var options = host.HostEnvironment.Options;
            if (options.Mode == EngineMode.Compiled)
            {
                var compileMethod = BuildCompileMethod();
                if (compileMethod != null)
                {
                    host.RegisterCompileFunc<T>(compileMethod);
                }

                var guessMethod = BuildGuessMethod();
                if (guessMethod != null)
                {
                    host.RegisterGuessFunc<T>(guessMethod);
                }
            }
            else
            {
                var excuteMethod = BuildExcuteMethod();
                if (excuteMethod != null)
                {
                    host.RegisterExecuteFunc<T>(excuteMethod);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Func<TemplateParser, TokenCollection, ITag> BuildParseMethod();
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Func<ITag, TemplateContext, object> BuildExcuteMethod();
    }
}