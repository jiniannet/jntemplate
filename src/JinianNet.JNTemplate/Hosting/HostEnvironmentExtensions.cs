/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate;
using JinianNet.JNTemplate.Caching;
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Parsers;
using JinianNet.JNTemplate.Resources;
using JinianNet.JNTemplate.Runtime;
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Exceptions;

namespace JinianNet.JNTemplate.Hosting
{
    /// <summary>
    /// 
    /// </summary>
    public static class HostEnvironmentExtensions
    {
        /// <summary>
        /// Generate Context
        /// </summary>
        /// <param name="name">template name</param>
        /// <param name="environment"> </param>
        /// <returns></returns>
        private static CompileContext GenerateContext(this IHostEnvironment environment, string name)
        {
            return GenerateContext(environment, name, null);
        }

        /// <summary>
        /// Create a compilation context.
        /// </summary>
        /// <param name="name">Unique key of the template</param>
        /// <param name="scope">The template data.</param>
        /// <param name="environment"></param>
        /// <returns></returns>
        private static CompileContext GenerateContext(this IHostEnvironment environment, string name, IVariableScope scope)
        {
            var ctx = new CompileContext(environment);
            ctx.Name = name;
            ctx.Data = scope ?? ctx.CreateVariableScope();
            return ctx;

        }

        /// <summary>
        /// Compile the text into a dynamic class.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name">Unique key of the template</param>
        /// <param name="action">The parameter setting method.</param>
        /// <param name="environment">The options of the engine.</param>
        /// <returns></returns>
        public static ICompilerResult CompileFile(this IHostEnvironment environment, string name, string path, Action<CompileContext> action = null)
        {
            var ctx = environment.GenerateContext(name);
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (action != null)
            {
                action(ctx);
            }
            var res = environment.Loader.Load(ctx, path);
            if (res == null)
            {
                throw new TemplateException($"Path:\"{path}\", the file could not be found.");
            }
            if (string.IsNullOrEmpty(ctx.Name))
            {
                ctx.Name = res.FullPath;
            }
            return ctx.Compile(res.Content);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="IVariableScope"/> class
        /// </summary>
        /// <param name="environment">The <see cref="IHostEnvironment"/></param>
        /// <returns></returns>
        public static IVariableScope CreateVariableScope(this IHostEnvironment environment)
        {
            var vs = environment?.ScopeProvider?.CreateScope();
            if (vs != null)
            {
                if (environment.Options.Data != null && environment.Options.Data.Count > 0)
                {
                    vs.Parent = environment.Options.Data;
                }
                vs.DetectPattern = environment.Options.TypeDetectPattern;
            }
            return vs;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IVariableScope"/> class
        /// </summary>
        /// <param name="environment">The <see cref="IHostEnvironment"/></param>
        /// <param name="parent">The <see cref="IVariableScope"/></param>
        /// <returns></returns>
        public static IVariableScope CreateVariableScope(this IHostEnvironment environment, IVariableScope parent)
        {
            var vs = environment?.ScopeProvider?.CreateScope();
            if (vs != null)
            {
                vs.Parent = parent;
                vs.DetectPattern = parent?.DetectPattern ?? environment.Options.TypeDetectPattern;
            }
            return vs;
        }

        /// <summary>
        /// Compile the text into a dynamic class.
        /// </summary>
        /// <param name="content">the context of the text</param>
        /// <param name="name">Unique key of the template</param>
        /// <param name="action">The parameter setting method.</param>
        /// <param name="environment">The options of the engine.</param>
        /// <returns></returns>
        public static ICompilerResult Compile(this IHostEnvironment environment, string name, string content, Action<CompileContext> action = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = content.GetHashCode().ToString();
            }
            var ctx = environment.GenerateContext(name);
            if (action != null)
            {
                action(ctx);
            }
            return ctx.Compile(content);
        }

        /// <summary>
        /// Creates template context.
        /// </summary>
        /// <returns>An instance of a <see cref="TemplateContext"/>.</returns>
        public static TemplateContext CreateContext(this IHostEnvironment environment)
        {
            var data = environment.CreateVariableScope();
            var ctx = new TemplateContext(data, environment);
            return ctx;
        }
    }
}
