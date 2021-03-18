/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Caching;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Parsers;
using JinianNet.JNTemplate.Resources;
using System;
using System.Collections.Generic;
using System.Text;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Engine runtime
    /// </summary>
    public sealed partial class Runtime
    {
        private static RuntimeOptions options;
        private static volatile object state;

        /// <summary>
        /// Gets the <see cref="RuntimeOptions"/>.
        /// </summary>
        internal static RuntimeOptions Options
        {
            get
            {
                if (options == null)
                {
                    lock (state)
                    {
                        options = RuntimeOptions.Create();
                    }
                }
                return options;
            }
        }

        /// <summary>
        /// Initializes a static instance of the <see cref="Runtime"/>.
        /// </summary>
        static Runtime()
        {
            state = new object();
        }

        /// <summary>
        /// Configuration engine which <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="conf">The <see cref="IDictionary{TKey, TValue}"/>.</param>
        /// <param name="scope">The global <see cref="VariableScope"/>.</param>
        internal static void Configure(IDictionary<string, string> conf, VariableScope scope)
        {
            Options.Data = (scope == null || scope.Count == 0) ? null : scope;
            Options.Initialization(conf);
        }

        /// <summary>
        /// Gets the resource directories.
        /// </summary>
        /// <value></value>
        public static List<string> ResourceDirectories => Options.ResourceDirectories;


        /// <summary>
        /// Gets the global data.
        /// </summary>
        public static VariableScope Data => Options.Data;

        /// <summary>
        /// Gets the <see cref="IResourceLoader"/>.
        /// </summary>
        public static IResourceLoader Loader => Options.Loader;

        /// <summary>
        /// Gets the <see cref="Encoding"/>.
        /// </summary>
        public static Encoding Encoding => Options.Encoding;

        /// <summary>
        /// Gets the cache object.
        /// </summary>

        public static ICache Cache => Options.Cache;

        /// <summary>
        /// Gets the <see cref="TemplateCollection{T}"/>.
        /// </summary>
        public static TemplateCollection<Compile.ICompileTemplate> Templates => Options.Templates;

        /// <summary>
        /// Sets an <see cref="IResourceLoader"/> values from engine.
        /// </summary>
        /// <param name="loader">The <see cref="IResourceLoader"/> to add set.</param> 
        public static void SetLoader(IResourceLoader loader)
        {
            if (loader == null)
            {
                throw new ArgumentNullException(nameof(loader));
            }
            Runtime.Options.Loader = loader;
        }

        /// <summary>
        /// Appends the specified directory name to the resource path list.
        /// </summary>
        /// <param name="path">The name of the directory to be appended to the resource path.</param>
        public static void AppendResourcePath(string path)
        {
            if (!Options.ResourceDirectories.Contains(path))
            {
                Options.ResourceDirectories.Add(path);
            }
        }

        /// <summary>
        /// Gets an value from environment variables.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        public static string GetEnvironmentVariable(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException($"\"{nameof(key)}\" cannot be null.");
            }
            string value;

            if (Options.Variable.TryGetValue(key, out value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Sets an value from environment variables.
        /// </summary>
        /// <param name="key">The key of the value to set.</param>
        /// <param name="value">The variable to add to.</param>
        public static void SetEnvironmentVariable(string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException($"\"{nameof(key)}\" cannot be null.");
            }
            if (value == null)
            {
                Options.Variable.Remove(key);
            }
            else
            {
                Options.Variable[key] = value;
            }
        }

        /// <summary>
        /// Register a tag parser.
        /// </summary>
        /// <param name="parser">The <see cref="ITagParser"/>.</param>
        /// <param name="index">Inserts an parser into the parser collections at the specified index.</param>
        public static void RegisterTagParser(ITagParser parser, int index = -1)
        {
            if (Options.Parsers.Contains(parser))
            {
                return;
            }
            if (index < 0)
            {
                Options.Parsers.Add(parser);
            }
            else
            {
                Options.Parsers.Insert(index, parser);
            }
        }

        /// <summary>
        /// Parsing the tag from the tokens.
        /// </summary>
        /// <param name="parser">The <see cref="TemplateParser"/>.</param>
        /// <param name="tc">The <see cref="TokenCollection"/>.</param>
        /// <returns></returns>
        public static ITag Parsing(TemplateParser parser, TokenCollection tc)
        {
            if (tc == null || tc.Count == 0 || parser == null)
            {
                return null;
            }

            var parsers = Options.Parsers;
            ITag t;
            for (int i = 0; i < parsers.Count; i++)
            {
                if (parsers[i] == null)
                {
                    continue;
                }
                t = parsers[i].Parse(parser, tc);
                if (t != null)
                {
                    t.FirstToken = tc.First;

                    if (t.Children.Count == 0 ||
                        (t.LastToken = t.Children[t.Children.Count - 1].LastToken ?? t.Children[t.Children.Count - 1].FirstToken) == null ||
                        tc.Last.CompareTo(t.LastToken) > 0)
                    {
                        t.LastToken = tc.Last;
                    }
                    return t;
                }
            }
            return null;
        }
    }
}