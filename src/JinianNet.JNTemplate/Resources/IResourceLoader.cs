/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Text;
using System.Threading.Tasks;

namespace JinianNet.JNTemplate.Resources
{
    /// <summary>
    /// The resource loader
    /// </summary>
    public interface IResourceLoader
    {

        /// <summary>
        /// Loads the resource on the specified path.
        /// </summary>
        /// <param name="filename">The fully qualified path or file name of the file to load.</param>
        /// <param name="ctx">The <see cref="Context"/>.</param> 
        /// <returns>An instance of a resource.</returns>
        ResourceInfo Load(Context ctx, string filename);

        /// <summary>
        /// Search for file full path.
        /// </summary>
        /// <param name="filename">The fully qualified path or file name of the file to load.</param>
        /// <param name="ctx">The <see cref="Context"/>.</param> 
        /// <returns>The file full path of the resource.</returns>
        string Find(Context ctx, string filename);

        /// <summary>
        /// Returns the directory information for the specified path string.
        /// </summary>
        /// <param name="fullPath">The path of a file.</param>
        /// <returns>Directory information for path, or null if path denotes a root directory or is null.</returns>
        string GetDirectoryName(string fullPath);

#if !NF40 && !NF45

        /// <summary>
        /// Loads the resource on the specified path.
        /// </summary>
        /// <param name="filename">The fully qualified path or file name of the file to load.</param>
        /// <param name="ctx">The <see cref="Context"/>.</param> 
        /// <returns>An instance of a resource.</returns>
        Task<ResourceInfo> LoadAsync(Context ctx, string filename);
#endif
    }
}
