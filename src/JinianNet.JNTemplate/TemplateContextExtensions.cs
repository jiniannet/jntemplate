/********************************************************************************
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
        public static string[] GetResourceDirectories(
#if NETCOREAPP || NETSTANDARD
            this
#endif
            Context ctx)
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
    }
}
