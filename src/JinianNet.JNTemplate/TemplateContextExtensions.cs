/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Parsers;
using JinianNet.JNTemplate.Exceptions;
using System;
using JinianNet.JNTemplate.Runtime;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Extensions methods for <see cref="TemplateContext"/>.
    /// </summary>
    public static class TemplateContextExtensions
    {
        /// <summary>
        /// Returns the names of directories (including their paths) in the <see cref="Context"/>.
        /// </summary>
        /// <param name="ctx">The  <see cref="Context"/>.</param>
        /// <returns>An array of the full names (including paths) of directories in the <see cref="Context"/>.</returns>
        public static string[] GetResourceDirectories(this Context ctx)
        {
            //if (string.IsNullOrEmpty(ctx.CurrentPath) || ctx.Options.ResourceDirectories.Contains(ctx.CurrentPath))
            //{
            //    return ctx.Options.ResourceDirectories.ToArray();
            //}
            //string[] paths = new string[ctx.Options.ResourceDirectories.Count + 1];
            //paths[0] = ctx.CurrentPath;
            //ctx.Options.ResourceDirectories.CopyTo(paths, 1);
            //return paths;
            return ctx.Options.ResourceDirectories.ToArray();
        }

        /// <summary>
        /// Loads the contents of an resource file on the specified path.
        /// </summary>
        /// <param name="fileName">The path of the file to load.</param> 
        /// <param name="ctx">The <see cref="Context"/>.</param>
        /// <returns>The loaded resource.</returns>
        public static Resources.ResourceInfo Load(this Context ctx, string fileName)
        {
            return ctx.Options.Loader.Load(ctx, fileName);
        }

        /// <summary>
        /// Returns the full path in the resource directorys.
        /// </summary>
        /// <param name="fileName">The relative or absolute path to the file to search.</param> 
        /// <param name="ctx">The <see cref="Context"/>.</param>
        /// <returns>The full path.</returns>
        public static string FindFullPath(this Context ctx, string fileName)
        {
            return ctx.Options.Loader.Find(ctx, fileName);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="IVariableScope"/> class
        /// </summary>
        /// <param name="options">The <see cref="RuntimeOptions"/></param>
        /// <returns></returns>
        public static IVariableScope CreateVariableScope(this RuntimeOptions options)
        {
            var vs = options?.ScopeProvider?.CreateScope();
            if (vs != null)
            {
                if (options.Data.Count > 0)
                {
                    vs.Parent = options.Data;
                }
                vs.DetectPattern = options.TypeDetectPattern;
            }
            return vs;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IVariableScope"/> class
        /// </summary>
        /// <param name="options">The <see cref="RuntimeOptions"/></param>
        /// <param name="parent">The <see cref="IVariableScope"/></param>
        /// <returns></returns>
        public static IVariableScope CreateVariableScope(this RuntimeOptions options, IVariableScope parent)
        {
            var vs = options?.ScopeProvider?.CreateScope();
            if (vs != null)
            {
                vs.Parent = parent;
                vs.DetectPattern = parent?.DetectPattern ?? options.TypeDetectPattern;
            }
            return vs;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IVariableScope"/> class
        /// </summary>
        /// <param name="ctx">The <see cref="Context"/></param>
        /// <returns></returns>
        public static IVariableScope CreateVariableScope(this Context ctx)
        {
            return ctx.Options.CreateVariableScope();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IVariableScope"/> class
        /// </summary>
        /// <param name="ctx">The <see cref="Context"/></param>
        /// <param name="parent">The <see cref="IVariableScope"/></param>
        /// <returns></returns>
        public static IVariableScope CreateVariableScope(this Context ctx, IVariableScope parent)
        {
            return ctx.Options.CreateVariableScope(parent);
        }

        /// <summary>
        /// Copies a range of elements from an <see cref="TemplateContext"/> starting at the first element and pastes them into another <see cref="CompileContext"/> starting at the first element.
        /// </summary>
        /// <param name="ctx1">The <see cref="TemplateContext"/> that contains the data to copy.</param>
        /// <param name="ctx2">The <see cref="CompileContext"/> that receives the data.</param>
        public static void CopyTo(this TemplateContext ctx1, CompileContext ctx2)
        {
            if (ctx1 != null && ctx2 != null)
            {
                ctx2.Data = ctx1.TempData;
                ctx2.CurrentPath = ctx1.CurrentPath;
                ctx2.Charset = ctx1.Charset;
                ctx2.OutMode = ctx1.OutMode;
                ctx2.ThrowExceptions = ctx1.ThrowExceptions;
                ctx2.Debug = ctx1.Debug;
            }
        }

        /// <summary>
        /// Create an <see cref="TemplateLexer"/> object.
        /// </summary>
        /// <param name="ctx">The <see cref="Context"/>.</param>
        /// <param name="text">The template contents.</param>
        /// <returns>An TemplateParser.</returns>
        public static TemplateLexer CreateTemplateLexer(this Context ctx, string text)
        {
            return new TemplateLexer(text,
                ctx.Options.TagPrefix,
                ctx.Options.TagSuffix,
                ctx.Options.TagFlag,
                ctx.Options.DisableeLogogram);
        }

        /// <summary>
        /// Create an <see cref="TemplateParser"/> object.
        /// </summary>
        /// <param name="ctx">The <see cref="Context"/>.</param>
        /// <param name="ts">The token array.</param>
        /// <returns>An TemplateParser.</returns>
        public static TemplateParser CreateTemplateParser(this Context ctx, Token[] ts)
        {
            return new TemplateParser(ctx.Options.Parser, ts);
        }

        /// <summary>
        /// Compiles and renders a template.
        /// </summary>
        /// <param name="path">The fully qualified path of the file to load.</param>
        /// <param name="context">The <see cref="TemplateContext"/>.</param>
        /// <returns></returns>
        public static string CompileFileAndExec(this TemplateContext context, string path)
        {
            var full = context.FindFullPath(path);
            if (full == null)
            {
                throw new TemplateException($"\"{ path }\" cannot be found.");
            }
            var template = context.Options.CompilerResults.GetOrAdd(full, (fullName) =>
            {
                var res = context.Load(fullName);
                if (res == null)
                {
                    throw new TemplateException($"Path:\"{path}\", the file could not be found.");
                }
                return TemplateCompiler.Compile(fullName, res.Content, context.Options, (c) => context.CopyTo(c));
            });
            using (var sw = new System.IO.StringWriter())
            {
                template.Render(sw, context);
                return sw.ToString();
            }
        }
    }
}
