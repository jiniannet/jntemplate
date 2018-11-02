/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;
using JinianNet.JNTemplate.Configuration;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Parsers;
using JinianNet.JNTemplate.Resources;

namespace JinianNet.JNTemplate {
    /// <summary>
    /// 引擎入口
    /// </summary>
    public class Engine {
        #region 私有变量
        private static object lockObject = new object ();
        /// <summary>
        /// 运行时
        /// </summary>
        public static RuntimeInfo Runtime {
            get {
                if (RuntimeInfo.GetInstance ().State == RuntimeInfo.InitializationState.None) {
                    Configure (Configuration.EngineConfig.CreateDefault ());
                }
                return RuntimeInfo.GetInstance ();
            }
        }

        #endregion

        #region 属性 
        /// <summary>
        /// 配置加载器
        /// </summary>
        /// <param name="provider">loder实列</param>
        public static void RegisterLodeProvider (ILoadProvider provider) {
            Runtime.LoadProvider = provider;
        }
        /// <summary>
        /// 配置缓存提供器
        /// </summary>
        /// <param name="provider">缓存提供器实例 </param>
        public static void RegisterCachingProvider (Caching.ICacheProvider provider) {
            Runtime.CacheProvider = provider;
        }
        /// <summary>
        /// 配置动态执行提供器
        /// </summary>
        /// <param name="provider">ICallProxy实例 </param>
        public static void RegisterDynamicProvider (IDynamicProvider provider) {
            Runtime.DynamicProvider = provider;
        }
        /// <summary>
        /// 解析器
        /// </summary>
        public List<ITagParser> Parsers {
            get {
                return Runtime.Parsers;
            }
        }
        #endregion

        #region 引擎配置
        private static object ChangeType (Type oldType, Type newType, object value) {
            if (oldType == newType) {
                return value;
            }
            if (newType.IsClass && oldType == typeof (string)) {
                Type t = Type.GetType (value.ToString ());
                return CreateInstance (t);
            }
            if (newType.IsArray && oldType.IsArray) {
                var oldArr = value as Array;
                var arr = Array.CreateInstance (newType.GetElementType (), oldArr.Length);
                for (int i = 0; i < oldArr.Length; i++) {
                    arr.SetValue (ChangeType (oldType.GetElementType (), newType.GetElementType (), oldArr.GetValue (i)), i);
                }
                return arr;
            }
            return Convert.ChangeType (value, newType);
        }

        /// <summary>
        /// 引擎配置
        /// </summary>
        /// <param name="conf">配置内容</param>
        /// <param name="scope">初始化全局数据</param>
        public static void Configure (ConfigBase conf, VariableScope scope) {
            if (conf == null) {
                throw new ArgumentNullException ("\"conf\" cannot be null.");
            }
            RuntimeInfo.GetInstance ().State = RuntimeInfo.InitializationState.Initialization;
            if (conf.TagParsers == null || conf.TagParsers.Count == 0) {
                conf.TagParsers = new List<ITagParser> ();
                conf.TagParsers.AddRange (LoadParsers (Field.RSEOLVER_TYPES));
            }
            if (conf.LoadProvider == null) {
                conf.LoadProvider = new FileLoadProvider ();
            }
            if (conf.DynamicProvider == null) {
                conf.DynamicProvider = new ReflectionDynamicProvider ();
            }
            if (conf.ResourceDirectories == null) {
                conf.ResourceDirectories = new List<string> ();
            }
            if (conf.ResourceDirectories.Count == 0) {
                conf.ResourceDirectories.Add (System.IO.Directory.GetCurrentDirectory ());
            }
            lock (lockObject) {
                RuntimeInfo.GetInstance ().Data = scope;
                Type runtimeType = RuntimeInfo.GetInstance ().GetType ();
                Type type = conf.GetType ();
                Type attrType = typeof (PropertyAttribute);
                PropertyInfo[] properties = type.GetProperties ();
                PropertyAttribute attr;
                foreach (PropertyInfo p in properties) {
                    object value;
                    if (!p.CanRead ||
#if NET20 || NET40
                        (value = p.GetValue (conf, null))
#else
                        (value = p.GetValue (conf))
#endif
                        ==
                        null) {
                        continue;
                    }
                    PropertyInfo runtimeProperty;
                    if (Attribute.IsDefined (p, attrType) && !string.IsNullOrEmpty ((attr =
#if NET20 || NET40
                            (PropertyAttribute) p.GetCustomAttributes (attrType, true) [0]
#else
                            (PropertyAttribute) p.GetCustomAttribute (attrType)
#endif
                        ).Name)) {
                        runtimeProperty = ReflectionDynamicProvider.GetPropertyInfo (runtimeType, attr.Name);
                    } else {
                        runtimeProperty = ReflectionDynamicProvider.GetPropertyInfo (runtimeType, p.Name);
                    }
                    if (runtimeProperty != null && runtimeProperty.CanWrite) {
                        object newValue = ChangeType (p.PropertyType, runtimeProperty.PropertyType, value);
                        if (newValue != null) {
#if NET20 || NET40
                            runtimeProperty.SetValue (RuntimeInfo.GetInstance (), newValue, null);
#else
                            runtimeProperty.SetValue (RuntimeInfo.GetInstance (), newValue);
#endif
                        }
                        continue;
                    }
                    SetEnvironmentVariable (p.Name, value.ToString ());
                }
            }

            if (conf.IgnoreCase) {
                RuntimeInfo.GetInstance ().BindIgnoreCase = BindingFlags.IgnoreCase;
                RuntimeInfo.GetInstance ().ComparerIgnoreCase = StringComparer.OrdinalIgnoreCase;
                RuntimeInfo.GetInstance ().ComparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
            } else {
                RuntimeInfo.GetInstance ().ComparisonIgnoreCase = StringComparison.Ordinal;
                RuntimeInfo.GetInstance ().BindIgnoreCase = BindingFlags.DeclaredOnly;
                RuntimeInfo.GetInstance ().ComparerIgnoreCase = StringComparer.Ordinal;
            }

            RuntimeInfo.GetInstance ().State = RuntimeInfo.InitializationState.Complete;
        }

        /// <summary>
        /// 引擎配置
        /// </summary>
        /// <param name="conf">配置内容</param>
        public static void Configure (ConfigBase conf) {
            Configure (conf, null);
        }

        #endregion

        #region 对外方法
        /// <summary>
        /// 创建模板上下文
        /// </summary>
        /// <returns></returns>
        public static TemplateContext CreateContext () {
            if (Runtime.Data == null) {
                return new TemplateContext ();
            }
            return new TemplateContext (Runtime.Data);
        }

        /// <summary>
        /// 从指定模板内容创建Template实例
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public static ITemplate CreateTemplate (string text) {
            return new Template (CreateContext (), text);
        }

        /// <summary>
        /// 从指定路径加载模板
        /// </summary>
        /// <param name="path">模板文件</param>
        /// <returns>ITemplate</returns>
        public static ITemplate LoadTemplate (string path) {
            return LoadTemplate (path, CreateContext ());
        }

        /// <summary>
        /// 从指定路径加载模板
        /// </summary>
        /// <param name="path">模板文件</param>
        /// <param name="ctx">模板上下文</param>
        /// <returns>ITemplate</returns>
        public static ITemplate LoadTemplate (string path, TemplateContext ctx) {
            ResourceInfo info = Runtime.Load (path, ctx.Charset);
            Template template;

            if (info != null) {
                template = new Template (ctx, info.Content);
                template.TemplateKey = info.FullPath;
                ctx.CurrentPath = Runtime.GetDirectoryName (info.FullPath);
            } else {
                template = new Template (ctx, string.Empty);
            }
            return template;
        }

        /// <summary>
        /// 分析标签
        /// </summary>
        /// <param name="parser">模板解析器</param>
        /// <param name="tc">TOKEN集合</param>
        /// <returns></returns>
        public static Nodes.Tag Resolve (TemplateParser parser, TokenCollection tc) {

            Tag t;
            for (int i = 0; i < Runtime.Parsers.Count; i++) {
                if (Runtime.Parsers[i] == null) {
                    continue;
                }
                t = Runtime.Parsers[i].Parse (parser, tc);
                if (t != null) {
                    t.FirstToken = tc.First;

                    if (t.Children.Count == 0 ||
                        (t.LastToken = t.Children[t.Children.Count - 1].LastToken ?? t.Children[t.Children.Count - 1].FirstToken) == null ||
                        tc.Last.CompareTo (t.LastToken) > 0) {
                        t.LastToken = tc.Last;
                    }
                    return t;
                }
            }
            return null;
        }

        #endregion
        #region 环境变量

        /// <summary>
        /// 获取环境变量
        /// </summary>
        /// <param name="variable">变量名称</param>
        /// <returns></returns>
        public static string GetEnvironmentVariable (string variable) {
            string value;

            if (Runtime.EnvironmentVariable.TryGetValue (variable, out value)) {
                return value;
            }
            return null;
        }

        /// <summary>
        /// 设置环境变量
        /// </summary>
        /// <param name="variable">变量名</param>
        /// <param name="value">值</param>
        public static void SetEnvironmentVariable (string variable, string value) {
            if (variable == null) {
                throw new ArgumentNullException ("\"variable\" cannot be null.");
            }
            if (value == null) {
                Runtime.EnvironmentVariable.Remove (variable);
            } else {
                Runtime.EnvironmentVariable[variable] = value;
            }
        }

        #endregion

        #region 私有方法
        /// <summary>
        /// 初始化环境变量配置
        /// </summary>
        /// <param name="conf">配置内容</param>
        private static void InitializationEnvironment (IDictionary<string, string> conf) {
            foreach (KeyValuePair<string, string> node in conf) {
                SetEnvironmentVariable (node.Key, node.Value);
            }
        }

        /// <summary>
        /// 加载标签分析器
        /// </summary>
        /// <param name="arr">解析器类型</param>
        private static ITagParser[] LoadParsers (string[] arr) {
            ITagParser[] list = new ITagParser[arr.Length];
            for (int i = 0; i < arr.Length; i++) {
                list[i] = CreateInstance<ITagParser> (Type.GetType (arr[i]));
            }
            return list;
        }

        private static T CreateInstance<T> (Type type) {
            return (T) Activator.CreateInstance (type ?? typeof (T));
        }
        private static object CreateInstance (Type type) {
            return Activator.CreateInstance (type);
        }
        #endregion
    }
}