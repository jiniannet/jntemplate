/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Configuration;
using JinianNet.JNTemplate.Resources;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 引擎入口
    /// </summary>
    public class Engine
    {
        #region 私有变量
        private static object lockObject = new object();
        private static Runtime engineRuntime;

        /// <summary>
        /// 运行时
        /// </summary>
        public static Runtime Runtime
        {
            get
            {
                if (engineRuntime == null)
                {
                    lock (lockObject)
                    {
                        if (engineRuntime == null)
                        {
                            Configure(Configuration.EngineConfig.CreateDefault());
                        }
                    }
                }
                return engineRuntime;
            }
        }

        #endregion

        #region 引擎配置


        /// <summary>
        /// 引擎配置
        /// </summary>
        /// <param name="conf">配置内容</param>
        public static void Configure(IConfig conf)
        {
            Configure(conf, null);
        }

        /// <summary>
        /// 引擎配置
        /// </summary>
        /// <param name="conf">配置内容</param>
        /// <param name="scope">初始数据</param>
        public static void Configure(IConfig conf, VariableScope scope)
        {
            engineRuntime = new Runtime(conf, scope);
        }
        #endregion

        #region 对外方法
        /// <summary>
        /// 创建模板上下文
        /// </summary>
        /// <returns></returns>
        public static TemplateContext CreateContext()
        {
            TemplateContext ctx = new TemplateContext(
                new VariableScope(Runtime.Data)
                , Runtime.Actuator
                , Runtime.Loader
                , Runtime.Parsers
                , Runtime.Cache);
            if (Runtime.ResourceDirectories != null && Runtime.ResourceDirectories.Count > 0)
            {
                ctx.ResourceDirectories.AddRange(Runtime.ResourceDirectories);
            }
            return ctx;
        }

        /// <summary>
        /// 从指定模板内容创建Template实例
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public static ITemplate CreateTemplate(string text)
        {
            var template = new Template(CreateContext(), text);
            template.Context.CurrentPath = System.IO.Directory.GetCurrentDirectory();
            return template;
        }

        /// <summary>
        /// 从指定路径加载模板
        /// </summary>
        /// <param name="path">模板文件</param>
        /// <returns>ITemplate</returns>
        public static ITemplate LoadTemplate(string path)
        {
            return LoadTemplate(path, CreateContext());
        }

        /// <summary>
        /// 从指定路径加载模板
        /// </summary>
        /// <param name="path">模板文件</param>
        /// <param name="ctx">模板上下文</param>
        /// <returns>ITemplate</returns>
        public static ITemplate LoadTemplate(string path, TemplateContext ctx)
        {
            var paths =
#if NETCOREAPP || NETSTANDARD
                    ctx.GetResourceDirectories();
#else
                    TemplateContextExtensions.GetResourceDirectories(ctx);
#endif

            ResourceInfo info = ctx.Loader.Load(path, ctx.Charset, paths);
            Template template;

            if (info != null)
            {
                template = new Template(ctx, info.Content);
                template.TemplateKey = info.FullPath;
                if (string.IsNullOrEmpty(ctx.CurrentPath))
                {
                    ctx.CurrentPath = ctx.Loader.GetDirectoryName(info.FullPath);
                }
            }
            else
            {
                template = new Template(ctx, string.Empty);
            }
            return template;
        }



#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 从指定路径加载模板
        /// </summary>
        /// <param name="path">模板文件</param>
        /// <param name="ctx">模板上下文</param>
        /// <returns>ITemplate</returns>
        public static async Task<ITemplate> LoadTemplateAsync(string path, TemplateContext ctx = null)
        {
            if (ctx == null)
            {
                ctx = CreateContext();
            }
            var paths =
#if NETCOREAPP || NETSTANDARD
                    ctx.GetResourceDirectories();
#else
                    TemplateContextExtensions.GetResourceDirectories(ctx);
#endif
            ResourceInfo info = await ctx.Loader.LoadAsync(path, ctx.Charset, paths);
            Template template;

            if (info != null)
            {
                template = new Template(ctx, info.Content);
                template.TemplateKey = info.FullPath;
                if (string.IsNullOrEmpty(ctx.CurrentPath))
                {
                    ctx.CurrentPath = ctx.Loader.GetDirectoryName(info.FullPath);
                }
            }
            else
            {
                template = new Template(ctx, string.Empty);
            }
            return template;
        }
#endif


        #endregion

        #region 环境变量

        /// <summary>
        /// 获取环境变量
        /// </summary>
        /// <param name="variable">变量名称</param>
        /// <returns></returns>
        public static string GetEnvironmentVariable(string variable)
        {
            string value;

            if (Runtime.EnvironmentVariable.TryGetValue(variable, out value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// 设置环境变量
        /// </summary>
        /// <param name="variable">变量名</param>
        /// <param name="value">值</param>
        public static void SetEnvironmentVariable(string variable, string value)
        {
            if (variable == null)
            {
                throw new ArgumentNullException("\"variable\" cannot be null.");
            }
            if (value == null)
            {
                Runtime.EnvironmentVariable.Remove(variable);
            }
            else
            {
                Runtime.EnvironmentVariable[variable] = value;
            }
        }

        #endregion


        #region 枚举状态

        /// <summary>
        /// 状态枚举
        /// </summary>
        public enum InitializationState
        {
            /// <summary>
            /// 未初始化
            /// </summary>
            None,
            /// <summary>
            /// 初始化中
            /// </summary>
            Initialization,
            /// <summary>
            /// 初始完成
            /// </summary>
            Complete
        }
        #endregion
    }
}