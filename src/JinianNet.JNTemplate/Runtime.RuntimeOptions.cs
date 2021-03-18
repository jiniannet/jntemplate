/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Caching;
using JinianNet.JNTemplate.Configuration;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Parsers;
using JinianNet.JNTemplate.Resources;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace JinianNet.JNTemplate
{
    public sealed partial class Runtime
    {
        /// <summary>
        /// Represents an global options.
        /// </summary>
        public class RuntimeOptions
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RuntimeOptions"/> class
            /// </summary>
            private RuntimeOptions()
            {

            }

            /// <summary>
            /// Greate a new instance of the <see cref="RuntimeOptions"/>.
            /// </summary>
            /// <returns></returns>
            internal static RuntimeOptions Create()
            {
                var store = new RuntimeOptions();
                store.Data = null;
                store.Variable = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                    { nameof(IConfig.Charset),"utf-8" },
                    { nameof(IConfig.TagPrefix),"${" },
                    { nameof(IConfig.TagSuffix),"}" },
                    { nameof(IConfig.TagFlag),"$" },
                    { nameof(IConfig.ThrowExceptions),"True" },
                    { nameof(IConfig.StripWhiteSpace),"False" },
                    { nameof(IConfig.IgnoreCase),"True" }
                };
                store.Cache = new MemoryCache();
                store.Parsers = new List<ITagParser>();
                store.ResourceDirectories = new List<string>();
                store.Loader = new FileLoader();
                store.Encoding = Encoding.UTF8;
                store.Templates = new TemplateCollection<Compile.ICompileTemplate>();
                store.BindIgnoreCase = BindingFlags.IgnoreCase;
                store.ComparerIgnoreCase = StringComparer.OrdinalIgnoreCase;
                store.ComparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
                foreach (var type in Field.RSEOLVER_TYPES)
                {
                    var parser = DynamicHelpers.CreateInstance<ITagParser>(type);
                    store.Parsers.Add(parser);
                }
                return store;
            }

            /// <summary>
            /// Initializes instance.
            /// </summary>
            /// <param name="dict">The <see cref="IDictionary{TKey, TValue}"/>.</param>
            public void Initialization(IDictionary<string, string> dict)
            {
                foreach (KeyValuePair<string, string> node in dict)
                {
                    if (string.IsNullOrEmpty(node.Value))
                    {
                        continue;
                    }
                    switch (node.Key)
                    {
                        case nameof(IConfig.Charset):
                            this.Encoding = Encoding.GetEncoding(node.Value);
                            break;
                        case nameof(IConfig.IgnoreCase):
                            if (Utility.StringToBoolean(node.Value))
                            {
                                this.BindIgnoreCase = BindingFlags.IgnoreCase;
                                this.ComparerIgnoreCase = StringComparer.OrdinalIgnoreCase;
                                this.ComparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
                            }
                            else
                            {
                                this.ComparisonIgnoreCase = StringComparison.Ordinal;
                                this.BindIgnoreCase = BindingFlags.DeclaredOnly;
                                this.ComparerIgnoreCase = StringComparer.Ordinal;
                            }
                            break;
                        default:
                            this.Variable[node.Key] = node.Value;
                            break;
                    }
                }
            }

            /// <summary>
            /// Gets or sets the global data of the engine.
            /// </summary>
            public VariableScope Data { set; get; }

            /// <summary>
            /// Gets or sets the <see cref="IResourceLoader"/> of the engine.
            /// </summary>
            public IResourceLoader Loader { set; get; }

            /// <summary>
            /// Gets or sets the <see cref="BindingFlags"/> of the engine.
            /// </summary>
            public BindingFlags BindIgnoreCase { set; get; }

            /// <summary>
            /// Gets or sets the <see cref="StringComparer"/> of the engine.
            /// </summary>
            public StringComparer ComparerIgnoreCase { set; get; }

            /// <summary>
            /// Gets or sets the <see cref="ComparisonIgnoreCase"/> of the engine.
            /// </summary>
            public StringComparison ComparisonIgnoreCase { set; get; }

            /// <summary>
            /// Gets or sets the <see cref="Encoding"/> of the engine.
            /// </summary>
            public Encoding Encoding { set; get; }

            /// <summary>
            /// Gets or sets the cache of the engine.
            /// </summary>

            public ICache Cache { set; get; }

            /// <summary>
            /// Gets or sets the global resource directories of the engine.
            /// </summary>
            /// <value></value>
            public List<string> ResourceDirectories { set; get; }
            /// <summary>
            /// Gets or sets the environment variable of the engine.
            /// </summary>
            public Dictionary<string, string> Variable { set; get; }
            /// <summary>
            /// Gets or sets the tag parsers of the engine.
            /// </summary>
            public List<ITagParser> Parsers { set; get; }

            /// <summary>
            /// Enable or disenable the compile mode.
            /// </summary>
            public bool EnableCompile { get; set; } = true;

            /// <summary>
            /// Enable or disenable the cache.
            /// </summary>
            public bool EnableTemplateCache { get; set; } = true;

            /// <summary>
            /// Gets or sets the template collection.
            /// </summary>
            public TemplateCollection<Compile.ICompileTemplate> Templates { get; set; }

        }
    }
}
