/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

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
        /// Loads the contents of an resource file on the specified path.
        /// </summary>
        /// <param name="fileName">The path of the file to load.</param> 
        /// <param name="ctx">The <see cref="Context"/>.</param>
        /// <returns>The loaded resource.</returns>
        public static Resources.ResourceInfo Load(this Context ctx, string fileName)
        {
            var paths = ctx.GetResourceDirectories();
            return Runtime.Loader.Load(fileName, ctx.Charset, paths);
        }

        /// <summary>
        /// Returns the full path in the resource directorys.
        /// </summary>
        /// <param name="fileName">The relative or absolute path to the file to search.</param> 
        /// <param name="ctx">The <see cref="Context"/>.</param>
        /// <returns>The full path.</returns>
        public static string FindFullPath(this Context ctx, string fileName)
        {
            var paths = ctx.GetResourceDirectories();
            return Runtime.Loader.FindFullPath(fileName, paths);
        }

        /// <summary>
        /// Copies a range of elements from an <see cref="TemplateContext"/> starting at the first element and pastes them into another <see cref="Compile.CompileContext"/> starting at the first element.
        /// </summary>
        /// <param name="ctx1">The <see cref="TemplateContext"/> that contains the data to copy.</param>
        /// <param name="ctx2">The <see cref="Compile.CompileContext"/> that receives the data.</param>
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
