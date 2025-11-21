/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Resources;
using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Exceptions;
using System.Collections.Generic;
using System.Threading;


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
        /// <param name="ctx">The  <see cref="ITemplateContext"/>.</param>
        /// <returns>An array of the full names (including paths) of directories in the <see cref="Context"/>.</returns>
        public static IEnumerable<string> GetResourceDirectories(this ITemplateContext ctx)
        {
            if (ctx.ResourceDirectories?.Count == 0)
                return new string[] { System.IO.Directory.GetCurrentDirectory() };
            return ctx.ResourceDirectories;
        }

        /// <summary>
        /// Loads the contents of an resource file on the specified path.
        /// </summary>
        /// <param name="fileName">The path of the file to load.</param> 
        /// <param name="ctx">The <see cref="ITemplateContext"/>.</param>
        /// <returns>The loaded resource.</returns>
        public static Resources.ResourceInfo Load(this ITemplateContext ctx, string fileName)
        {
            return ctx.ResourceManager.Load(ctx, fileName);
        }

#if !NF40 && !NF45 && !NF35 && !NF20
        /// <summary>
        /// Loads the contents of an resource file on the specified path.
        /// </summary>
        /// <param name="fileName">The path of the file to load.</param> 
        /// <param name="ctx">The <see cref="ITemplateContext"/>.</param>
        /// <returns>The loaded resource.</returns>
        public static Task<Resources.ResourceInfo> LoadAsync(this ITemplateContext ctx, string fileName)
        {
            return ctx.ResourceManager.LoadAsync(ctx, fileName);
        }
#endif

        /// <summary>
        /// Returns the full path in the resource directorys.
        /// </summary>
        /// <param name="fileName">The relative or absolute path to the file to search.</param> 
        /// <param name="ctx">The <see cref="ITemplateContext"/>.</param>
        /// <returns>The full path.</returns>
        public static string FindFullPath(this ITemplateContext ctx, string fileName)
        {
            return ctx.ResourceManager.Find(ctx, fileName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IVariableScope"/> class
        /// </summary>
        /// <param name="ctx">The <see cref="ITemplateContext"/></param>
        /// <returns></returns>
        public static IVariableScope CreateVariableScope(this ITemplateContext ctx)
        {
            var vs = ctx?.ScopeProvider?.CreateScope();
            vs.Parent = ctx.TempData;
            return vs;
        }

        /// <summary>
        /// Create an <see cref="TemplateLexer"/> object.
        /// </summary>
        /// <param name="ctx">The <see cref="Context"/>.</param>
        /// <param name="text">The template contents.</param>
        /// <returns>An TemplateParser.</returns>
        public static TemplateLexer CreateTemplateLexer(this ITemplateContext ctx, string text)
        {
            return new TemplateLexer(text, ctx);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static ITemplateResult InterpretTemplate(this ITemplateContext ctx, string text)
        {
            return InterpretTemplate(ctx, Utility.ContentToTemplateName(text), new Resources.StringReader(text));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ITemplateResult InterpretTemplate(this ITemplateContext context, string name, IResourceReader reader)
        {

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }


            ITemplateResult result;
            if (context.EnableCache && (result = context.Cache[name]) != null)
                return result;

            result = Interpre(reader, context);

            if (context.EnableCache)
                context.Cache[name] = result;

            return result;
        }


        private static ITemplateResult Interpre(IResourceReader reader, ITemplateContext ctx)
        {
            var content = reader.ReadToEnd(ctx);
            var tags = Lexer(ctx, content);
            return new InterpretResult(tags);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="text"></param> 
        /// <returns></returns>
        public static TagCollection Lexer(this ITemplateContext context, string text)
        {

            if (string.IsNullOrEmpty(text))
            {
                return new TagCollection();
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
        public static TemplateParser CreateTemplateParser(this ITemplateContext ctx, Token[] ts)
        {
            return new TemplateParser(ctx.Resolver, ts);
        }

        /// <summary>
        /// Compiles and renders a template.
        /// </summary>
        /// <param name="path">The fully qualified path of the file to load.</param>
        /// <param name="context">The <see cref="TemplateContext"/>.</param>
        /// <returns></returns>
        public static ITemplateResult CompileFile(this TemplateContext context, string path)
        {
            var full = context.FindFullPath(path);
            if (full == null)
            {
                throw new TemplateException($"\"{path}\" cannot be found.");
            }
            var template = context.Cache.GetOrAdd(full, (fullName) =>
            {
                var res = context.Load(fullName);
                if (res == null)
                {
                    throw new TemplateException($"Path:\"{path}\", the file could not be found.");
                }
                return context.Compile(fullName, res.Content);
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
        public static ITemplateResult CompileTemplate(this TemplateContext context, string name, IResourceReader reader)
        {
            if (reader == null)
            {
                return new EmptyCompileTemplate();
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            ITemplateResult result;
            if (context.EnableCache && (result = context.Cache[name]) != null)
                return result;

            result = Compile(name, reader, context);

            if (context.EnableCache)
                context.Cache[name] = result;

            return result;
        }

        private static ICompilerResult Compile(string name, IResourceReader reader, TemplateContext ctx)
        {
            var content = reader.ReadToEnd(ctx);
            return ctx.Compile(name, content);
        }




        /// <summary>
        /// Compile the text into a dynamic class.
        /// </summary>
        /// <param name="content">the context of the text</param>
        /// <param name="name">Unique key of the template</param>
        /// <param name="action">The parameter setting method.</param>
        /// <param name="context">The <see cref="TemplateContext"/>.</param>
        /// <returns></returns> 
        public static ICompilerResult Compile(this TemplateContext context, string name, string content, Action<CompileContext> action = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = Utility.ContentToTemplateName(content);
            }

            using (var ctx = context.GenerateContext(name, context.TempData))
            {
                if (action != null)
                {
                    action(ctx);
                }
                return ctx.Compile(content);
            }
        }

        /// <summary>
        /// Create a compilation context.
        /// </summary> 
        /// <param name="scope">The template data.</param>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static CompileContext GenerateContext(this ITemplateContext context, string name, IVariableScope scope)
        {
            var ctx = new CompileContext(context);
            ctx.TempData = scope ?? context.CreateVariableScope();
            ctx.Name = name;
            return ctx;

        }

        /// <summary>
        /// Performs the render for a tags.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="writer">See the <see cref="System.IO.TextWriter"/>.</param>
        /// <param name="collection">The tags collection.</param>
        public static void Render(this TemplateContext ctx, System.IO.TextWriter writer, TagCollection collection)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (collection == null || collection.Count <= 0)
                return;

            for (int i = 0; i < collection.Count; i++)
            {
                try
                {
                    var tagResult = Execute(ctx, collection[i]);
                    if (tagResult == null)
                        continue;
#if !NF35 && !NF20
                    if (tagResult is Task task)
                    {
#if NF40
                        task.Wait();
#else
                        task.ConfigureAwait(false).GetAwaiter();
#endif
                        var pi = task.GetType().GetPropertyInfo("Result");
                        if (pi != null)
                        {
                            var value = pi.GetValue(task, null);
                            writer.Write(value?.ToString());
                        }
                        continue;
                    }
#endif
                    writer.Write(tagResult.ToString());

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
            if (!ctx.DisableCodeOutput && !ctx.ThrowExceptions)
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
        public static async Task RenderAsync(this TemplateContext ctx, System.IO.TextWriter writer, TagCollection collection, CancellationToken cancellationToken = default)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (collection == null || collection.Count <= 0)
                return;
            for (int i = 0; i < collection.Count; i++)
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
                                var taskResult = (Task<string>)typeof(Utility).CallGenericMethod(null, "ExcuteReturnTaskAsync", type.GetGenericArguments(), tagResult);
                                var taskValue = await taskResult;
                                await writer.WriteAsync(taskValue);
                            }
                            else
                            {
                                await task;
                            }
                            continue;
                        }
                        await writer.WriteAsync(OutputFormat(ctx, tagResult));
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
            return ctx.Resolver.Excute(tag, ctx);
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
            if (value == null)
                return;
            if (value is Array arr)
            {
                var eType = anonymousType.GetElementType();
                var type = ObjectBuilder.GetOrGenerateType(eType);
                var newArr = Array.CreateInstance(type, arr.Length);
                for (var i = 0; i < arr.Length; i++)
                {
                    var item = ObjectBuilder.FromAnonymousObject(arr.GetValue(i), type);
                    newArr.SetValue(item, i);
                }
                c.TempData.Set(key, newArr, newArr.GetType());

            }
            else
            {
                var type = ObjectBuilder.GetOrGenerateType(anonymousType);
                value = ObjectBuilder.FromAnonymousObject(value, type);
                c.TempData.Set(key, value, type);
            }
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
            if (ctx.IsCompileMode)
            {
                type = type ?? value?.GetType();
                var isArray = type?.IsArray ?? false;
                Type elementType = isArray ? type.GetElementType() : type;

                if (elementType != null && !elementType.IsVisible)
                {
                    if (!elementType.Name.Contains("AnonymousType"))
                    {
                        throw new ArgumentException($"The type \"{elementType.FullName}\" is not accessible");
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
        /// <param name="ctx"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool MustFormat(this TemplateContext ctx, object value)
        {
            if (value == null)
                return false;
            var type = value.GetType();
            return MustFormat(ctx, type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool MustFormat(this TemplateContext ctx, Type type)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string OutputFormat(this ITemplateContext ctx, object value)
        {
            if (value == null)
                return null;
            return value.ToString();
        }
    }
}
