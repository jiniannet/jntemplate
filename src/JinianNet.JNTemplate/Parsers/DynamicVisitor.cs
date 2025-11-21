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
    public class DynamicVisitor : ITagVisitor
    {

        private readonly Func<TemplateParser, TokenCollection, ITag> parseMethod;
        private readonly Func<ITag, CompileContext, MethodInfo> compileMethod;
        private readonly Func<ITag, CompileContext, Type> guessMethod;
        private readonly Func<ITag, TemplateContext, object> excuteMethod;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parse"></param>
        /// <param name="compile"></param>
        /// <param name="guess"></param>
        public DynamicVisitor(Type type, 
            Func<TemplateParser, TokenCollection, ITag> parse,
            Func<ITag, CompileContext, MethodInfo> compile,
            Func<ITag, CompileContext, Type> guess)
        {
            Name = type.Name;
            parseMethod = parse;
            compileMethod = compile;
            guessMethod = guess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="func"></param>
        public DynamicVisitor(Type type,
            Func<ITag, TemplateContext, object> func)
        {
            Name = type.Name; 
            excuteMethod = func;
        }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public MethodInfo Compile(ITag tag, CompileContext context)
        {
            return compileMethod?.Invoke(tag, context); 
        }

        /// <inheritdoc />
        public object Excute(ITag tag, TemplateContext context)
        {
            return excuteMethod?.Invoke(tag, context);
        }

        /// <inheritdoc />
        public Type GuessType(ITag tag, CompileContext context)
        {
            return guessMethod?.Invoke(tag, context);
        }

        /// <inheritdoc />
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {
            return parseMethod?.Invoke(parser, tc);
        }
    }
}
