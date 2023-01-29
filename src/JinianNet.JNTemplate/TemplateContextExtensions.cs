/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Threading;
using JinianNet.JNTemplate.Hosting;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Resources;
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Exceptions;
#if !NF35 && !NF20
using System.Threading.Tasks;
#endif

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
            return ctx.Environment.Options.ResourceDirectories.ToArray();
        }

        /// <summary>
        /// Loads the contents of an resource file on the specified path.
        /// </summary>
        /// <param name="fileName">The path of the file to load.</param> 
        /// <param name="ctx">The <see cref="Context"/>.</param>
        /// <returns>The loaded resource.</returns>
        public static Resources.ResourceInfo Load(this Context ctx, string fileName)
        {
            return ctx.Environment.Loader.Load(ctx, fileName);
        }

#if !NF40 && !NF45 && !NF35 && !NF20
        /// <summary>
        /// Loads the contents of an resource file on the specified path.
        /// </summary>
        /// <param name="fileName">The path of the file to load.</param> 
        /// <param name="ctx">The <see cref="Context"/>.</param>
        /// <returns>The loaded resource.</returns>
        public static Task<Resources.ResourceInfo> LoadAsync(this Context ctx, string fileName)
        {
            return ctx.Environment.Loader.LoadAsync(ctx, fileName);
        }
#endif

        /// <summary>
        /// Returns the full path in the resource directorys.
        /// </summary>
        /// <param name="fileName">The relative or absolute path to the file to search.</param> 
        /// <param name="ctx">The <see cref="Context"/>.</param>
        /// <returns>The full path.</returns>
        public static string FindFullPath(this Context ctx, string fileName)
        {
            return ctx.Environment.Loader.Find(ctx, fileName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IVariableScope"/> class
        /// </summary>
        /// <param name="ctx">The <see cref="Context"/></param>
        /// <returns></returns>
        public static IVariableScope CreateVariableScope(this Context ctx)
        {
            return ctx.Environment.CreateVariableScope();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IVariableScope"/> class
        /// </summary>
        /// <param name="ctx">The <see cref="Context"/></param>
        /// <param name="parent">The <see cref="IVariableScope"/></param>
        /// <returns></returns>
        public static IVariableScope CreateVariableScope(this Context ctx, IVariableScope parent)
        {
            return ctx.Environment.CreateVariableScope(parent);
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
            var options = ctx.Environment.Options;
            return new TemplateLexer(text,
                options.TagPrefix,
                options.TagSuffix,
                options.TagFlag,
                options.DisableeLogogram);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IResult InterpretTemplate(this Context ctx, string text)
        {
            return InterpretTemplate(ctx, text.GetHashCode().ToString(), new StringReader(text));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static IResult InterpretTemplate(this Context context, string name, IReader reader)
        {

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var env = context.Environment;

            if (!context.EnableCache)
            {
                return new InterpretResult(Lexer(context, reader.ReadToEnd(context)));
            }

            return env.Results.GetOrAdd(name, (fullName) =>
            {
                return new InterpretResult(Lexer(context, reader.ReadToEnd(context)));
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="text"></param> 
        /// <returns></returns>
        public static ITag[] Lexer(this Context context, string text)
        {

            if (string.IsNullOrEmpty(text))
            {
                return new ITag[0];
            }

            var lexer = CreateTemplateLexer(context, text);
            var ts = lexer.Execute();
            var parser = CreateTemplateParser(context, ts);

            return parser.Execute();

        }



        /// <summary>
        /// Create an <see cref="TemplateParser"/> object.
        /// </summary>
        /// <param name="ctx">The <see cref="Context"/>.</param>
        /// <param name="ts">The token array.</param>
        /// <returns>An TemplateParser.</returns>
        public static TemplateParser CreateTemplateParser(this Context ctx, Token[] ts)
        {
            return new TemplateParser(ctx.Environment.Parser, ts);
        }

        /// <summary>
        /// Compiles and renders a template.
        /// </summary>
        /// <param name="path">The fully qualified path of the file to load.</param>
        /// <param name="context">The <see cref="TemplateContext"/>.</param>
        /// <returns></returns>
        public static IResult CompileFile(this TemplateContext context, string path)
        {
            var full = context.FindFullPath(path);
            if (full == null)
            {
                throw new TemplateException($"\"{ path }\" cannot be found.");
            }
            var template = context.Environment.Results.GetOrAdd(full, (fullName) =>
            {
                var res = context.Load(fullName);
                if (res == null)
                {
                    throw new TemplateException($"Path:\"{path}\", the file could not be found.");
                }
                return context.Environment.Compile(fullName, res.Content, (c) => context.CopyTo(c));
            });
            return template;
        }

        /// <summary>
        /// Compiles and renders a template.
        /// </summary>
        /// <param name="path">The fully qualified path of the file to load.</param>
        /// <param name="context">The <see cref="TemplateContext"/>.</param>
        /// <returns></returns>
        public static string CompileAndRenderFile(this TemplateContext context, string path)
        {
            var template = CompileFile(context, path);
            using (var sw = new System.IO.StringWriter())
            {
                template.Render(sw, context);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Compiles and renders a template.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="context">The <see cref="TemplateContext"/>.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IResult CompileTemplate(this TemplateContext context, string name, IReader reader)
        {
            if (reader == null)
            {
                return new EmptyCompileTemplate();
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var env = context.Environment;

            if (!context.EnableCache && context.Debug)
            {
                System.Diagnostics.Trace.TraceWarning("WARN:The template cache is disabled.");
                return env.Compile(name, reader.ReadToEnd(context), (c) => context.CopyTo(c));

            }

            return env.Results.GetOrAdd(name, (fullName) =>
            {
                return env.Compile(fullName, reader.ReadToEnd(context), (c) => context.CopyTo(c));

            });
        }

        /// <summary>
        /// Performs the render for a tags.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="writer">See the <see cref="System.IO.TextWriter"/>.</param>
        /// <param name="collection">The tags collection.</param>
        public static void Render(this TemplateContext ctx, System.IO.TextWriter writer, ITag[] collection)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("\"writer\" cannot be null.");
            }

            if (collection != null && collection.Length > 0)
            {
                for (int i = 0; i < collection.Length; i++)
                {
                    try
                    {
                        var tagResult = Execute(ctx, collection[i]);
                        if (tagResult != null)
                        {
#if !NF35 && !NF20
                            if (tagResult is Task task)
                            {
                                var type = tagResult.GetType();
                                if (type.IsGenericType)
                                {
                                    var taskResult = typeof(Utility).CallGenericMethod(null, "ExcuteTaskAsync", type.GetGenericArguments(), tagResult);
#if NF40
                                    writer.Write((string)taskResult);
#else
                                    writer.Write(((Task<string>)taskResult).GetAwaiter().GetResult());
#endif
                                }
                                else
                                {
#if NF40
                                    task.Wait();
#else
                                    task.ConfigureAwait(false).GetAwaiter();
#endif
                                }
                                continue;
                            }
#endif
                            writer.Write(tagResult.ToString());
                        }
                    }
                    catch (TemplateException e)
                    {
                        ThrowException(ctx, e, collection[i], writer);
                    }
                    catch (System.Exception e)
                    {
                        var baseException = e.GetBaseException();
                        ThrowException(ctx, new ParseException(collection[i], baseException), collection[i], writer);
                    }
                }
            }
        }

        /// <summary>
        /// Throw exception.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="e">Represents errors that occur during application execution.</param>
        /// <param name="tag">Represents errors tag.</param>
        /// <param name="writer">See the <see cref="System.IO.TextWriter"/>.</param>
        public static void ThrowException(this TemplateContext ctx, TemplateException e, ITag tag, System.IO.TextWriter writer)
        {
            ctx.AddError(e);
            if (!ctx.ThrowExceptions)
            {
                writer.Write(tag.ToSource());
            }
        }

#if !NF40 && !NF45 && !NF35 && !NF20

        /// <summary>
        /// Performs the render for a tags.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="writer">See the <see cref="System.IO.TextWriter"/>.</param>
        /// <param name="collection">The tags collection.</param>
        /// <param name="cancellationToken">See the <see cref="CancellationToken"/>.</param>
        public static async Task RenderAsync(this TemplateContext ctx, System.IO.TextWriter writer, ITag[] collection, CancellationToken cancellationToken = default)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("\"writer\" cannot be null.");
            }

            if (collection != null && collection.Length > 0)
            {
                for (int i = 0; i < collection.Length; i++)
                {
                    try
                    {
                        var tagResult = Execute(ctx, collection[i]);
                        if (tagResult != null)
                        {
                            if (tagResult is Task task)
                            {
                                var type = tagResult.GetType();
                                if (type.IsGenericType)
                                {
                                    var taskResult = (Task<string>)typeof(Utility).CallGenericMethod(null, "ExcuteTaskAsync", type.GetGenericArguments(), tagResult);
                                    var taskValue = await taskResult;
                                    await writer.WriteAsync(taskValue);
                                }
                                else
                                {
                                    await task;
                                }
                                continue;
                            }
                            await writer.WriteAsync(tagResult.ToString());
                        }
                    }
                    catch (TemplateException e)
                    {
                        await ThrowExceptionAsync(ctx, e, collection[i], writer);
                    }
                    catch (System.Exception e)
                    {
                        var baseException = e.GetBaseException();
                        await ThrowExceptionAsync(ctx, new ParseException(collection[i], baseException), collection[i], writer);
                    }
                }
            }
        }

        /// <summary>
        /// Throw exception.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="e">Represents errors that occur during application execution.</param>
        /// <param name="tag">Represents errors tag.</param>
        /// <param name="writer">See the <see cref="System.IO.TextWriter"/>.</param>
        public static async Task ThrowExceptionAsync(this TemplateContext ctx, TemplateException e, ITag tag, System.IO.TextWriter writer)
        {
            ctx.AddError(e);
            if (!ctx.ThrowExceptions)
            {
                await writer.WriteAsync(tag.ToSource());
            }
        }
#endif

        /// <summary>
        /// Execute the tags.
        /// </summary>
        /// <param name="tag">The <see cref="ITag"/>.</param>
        /// <param name="ctx">The <see cref="TemplateContext"/>.</param>
        /// <returns></returns>
        public static object Execute(this TemplateContext ctx, ITag tag)
        {
            var func = ctx.ExecutorBuilder.Build(tag);
            return func(tag, ctx);
        }



        /// <summary>
        /// Set a anonymous object for variables.
        /// </summary>
        /// <param name="c">The <see cref="TemplateContext"/>.</param>
        /// <param name="key">The key of the element to get</param> 
        /// <param name="value">The value with the specified key.</param> 
        public static void SetAnonymousObject(this TemplateContext c, string key, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            SetAnonymousObject(c, key, value, value.GetType());
        }

        /// <summary>
        /// Set a anonymous object for variables.
        /// </summary>
        /// <param name="c">The <see cref="TemplateContext"/>.</param>
        /// <param name="key">The key of the element to get</param> 
        /// <param name="value">The value with the specified key.</param>
        /// <param name="anonymousType">The <see cref="Type"/>.</param>
        public static void SetAnonymousObject(this TemplateContext c, string key, object value, Type anonymousType)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            var type = ObjectBuilder.GetOrGenerateType(anonymousType);
            if (value != null)
            {
                value = ObjectBuilder.FromAnonymousObject(value, type);
            }
            c.TempData.Set(key, value, type);
        }


        /// <summary>
        /// Set a new value for variables.
        /// </summary>
        /// <param name="ctx">The <see cref="TemplateContext"/>.</param> 
        /// <param name="key">The key of the element to get</param> 
        /// <param name="value">The element with the specified key.</param>
        /// <param name="type">The type with the specified key.</param>
        public static void Set(this TemplateContext ctx, string key, object value, Type type)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (ctx.Mode == EngineMode.Compiled)
            {
                type = type ?? value?.GetType();
                if (type != null && !type.IsVisible)
                {
                    if (!type.Name.Contains("AnonymousType"))
                    {
                        throw new ArgumentException($"The type \"{type.FullName}\" is not accessible");
                    }
                    TemplateContextExtensions.SetAnonymousObject(ctx, key, value, type);
                }
                else
                {
                    ctx.TempData.Set(key, value, type);
                }
            }
            else
            {
                if (value == null)
                {
                    ctx.TempData.Set(key, value, type);
                }
                else
                {
                    ctx.TempData.Set(key, value, null);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tag"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private static object Execute(this TemplateContext ctx, string name, ITag tag)
        {
            var func = ctx.ExecutorBuilder.Build(name);
            return func(tag, ctx);
        }
    }
}
