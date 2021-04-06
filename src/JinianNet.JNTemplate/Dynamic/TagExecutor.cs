/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Nodes;
using System;

namespace JinianNet.JNTemplate.Dynamic
{
    /// <summary>
    /// TagExecutor
    /// </summary>
    public class TagExecutor
    {

        /// <summary>
        /// Execute the tags.
        /// </summary>
        /// <param name="tag">The <see cref="ITag"/>.</param>
        /// <param name="ctx">The <see cref="TemplateContext"/>.</param>
        /// <returns></returns>
        public static object Execute(ITag tag, TemplateContext ctx)
        {
            var func = ctx.ExecutorBuilder.Build(tag);
            return func(tag, ctx);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tag"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private static object Execute(string name, ITag tag, TemplateContext ctx)
        {
            var func = ctx.ExecutorBuilder.Build(name);
            return func(tag, ctx);
        }

        /// <summary>
        /// Register a parse mehtod.
        /// </summary>
        /// <typeparam name="T">The type of itag.</typeparam>
        /// <param name="func">The parse method.</param>
        /// <param name="ctx">The <see cref="TemplateContext"/>.</param>
        public static void Register<T>(TemplateContext ctx, Func<ITag, TemplateContext, object> func) where T : ITag
        {
            ctx.ExecutorBuilder.Register<T>(func);
        }
    }
}
