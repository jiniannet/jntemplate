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
        private static Lazy<ExecuteBuilder> builder;

        /// <summary>
        /// Gets or sets the builder of the object.
        /// </summary>
        public static ExecuteBuilder Builder
        {
            get { return builder.Value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TagExecutor"/> class.
        /// </summary>
        static TagExecutor()
        {
            builder = new Lazy<ExecuteBuilder>();
        }
        /// <summary>
        /// Execute the tags.
        /// </summary>
        /// <param name="tag">The <see cref="ITag"/>.</param>
        /// <param name="ctx">The <see cref="TemplateContext"/>.</param>
        /// <returns></returns>
        public static object Exec(ITag tag, TemplateContext ctx)
        {
            var func = Builder.Build(tag);
            return func(tag, ctx);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tag"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private static object Exec(string name, ITag tag, TemplateContext ctx)
        {
            var func = Builder.Build(name);
            return func(tag, ctx);
        }

        /// <summary>
        /// Register a parse mehtod.
        /// </summary>
        /// <typeparam name="T">The type of itag.</typeparam>
        /// <param name="func">The parse method.</param>
        public static void Register<T>(Func<ITag, TemplateContext, object> func) where T : ITag
        {
            Builder.Register<T>(func);
        }
    }
}
