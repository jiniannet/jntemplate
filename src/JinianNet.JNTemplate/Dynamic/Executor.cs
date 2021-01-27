/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Resources;
using System;

namespace JinianNet.JNTemplate.Dynamic
{
    /// <summary>
    /// 
    /// </summary>
    public class Executor
    {
        private static Lazy<ExecuteBuilder> builder;

        /// <summary>
        /// Compile builder
        /// </summary>
        public static ExecuteBuilder Builder
        {
            get { return builder.Value; }
        }

        /// <summary>
        /// ctor
        /// </summary>
        static Executor()
        {
            builder = new Lazy<ExecuteBuilder>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="ctx"></param>
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
        public static object Parse(string name, ITag tag, TemplateContext ctx)
        {
            var func = Builder.Build(name);
            return func(tag, ctx);
        }

        /// <summary>
        /// 注册执行方法
        /// </summary>
        /// <typeparam name="T">ITag</typeparam>
        /// <param name="func">func</param>
        public static void Register<T>(Func<ITag, TemplateContext, object> func) where T : ITag
        {
            Builder.Register<T>(func);
        }
    }
}
