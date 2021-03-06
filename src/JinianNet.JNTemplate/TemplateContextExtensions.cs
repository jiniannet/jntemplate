﻿/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// TemplateContex扩展类
    /// </summary>
    public static class TemplateContextExtensions
    {
        /// <summary>
        /// 获取当前
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static string[] GetResourceDirectories(this Context ctx)
        {
            if (string.IsNullOrEmpty(ctx.CurrentPath) || ctx.ResourceDirectories.Contains(ctx.CurrentPath))
            {
                return ctx.ResourceDirectories.ToArray();
            }
            string[] paths = new string[ctx.ResourceDirectories.Count + 1];
            paths[0] = ctx.CurrentPath;
            ctx.ResourceDirectories.CopyTo(paths, 1);
            return paths;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="fileName">文件名,可以是纯文件名,也可以是完整的路径</param> 
        /// <param name="ctx">上下文</param>
        /// <returns>ResourceInfo</returns>
        public static Resources.ResourceInfo Load(this Context ctx, string fileName)
        {
            var paths = ctx.GetResourceDirectories();
            return Runtime.Loader.Load(fileName, ctx.Charset, paths);
        }

        /// <summary>
        /// 获取完整路径
        /// </summary>
        /// <param name="fileName">文件名,可以是纯文件名,也可以是完整的路径</param> 
        /// <param name="ctx">上下文</param>
        /// <returns></returns>
        public static string FindFullPath(this Context ctx, string fileName)
        {
            var paths = ctx.GetResourceDirectories();
            return Runtime.Loader.FindFullPath(fileName, paths);
        }

        /// <summary>
        /// TemplateContext CopyTo CompileContext
        /// </summary>
        /// <param name="ctx1">TemplateContext</param>
        /// <param name="ctx2">CompileContext</param>
        public static void CopyTo(this TemplateContext ctx1, Compile.CompileContext ctx2)
        {
            if (ctx1 != null && ctx2 != null)
            {
                ctx2.Data = ctx1.TempData;
                ctx2.CurrentPath = ctx1.CurrentPath;
                ctx2.Charset = ctx1.Charset;
                if (ctx1.ResourceDirectories != null && ctx1.ResourceDirectories.Count > 0)
                {
                    ctx2.ResourceDirectories.AddRange(ctx1.ResourceDirectories);
                }
                ctx2.StripWhiteSpace = ctx1.StripWhiteSpace;
                ctx2.ThrowExceptions = ctx1.ThrowExceptions;
            }
        }
    }
}
