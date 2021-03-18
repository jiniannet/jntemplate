/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System.Text;
#if !NET20
using System.Threading.Tasks;
#endif


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
        /// <param name="encoding">The <see cref="Encoding"/>.</param>
        /// <param name="directory">The resource search directory.</param>
        /// <returns>An instance of a resource.</returns>
        ResourceInfo Load(string filename, Encoding encoding, params string[] directory);

        /// <summary>
        /// Search for file full path.
        /// </summary>
        /// <param name="filename">The fully qualified path or file name of the file to load.</param>
        /// <param name="directory">The resource search directory.</param>
        /// <returns>The file full path of the resource.</returns>
        string FindFullPath(string filename, params string[] directory);

        /// <summary>
        /// Returns the directory information for the specified path string.
        /// </summary>
        /// <param name="fullPath">The path of a file.</param>
        /// <returns>Directory information for path, or null if path denotes a root directory or is null.</returns>
        string GetDirectoryName(string fullPath);

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// Loads the resource on the specified path.
        /// </summary>
        /// <param name="filename">The fully qualified path or file name of the file to load.</param>
        /// <param name="encoding">The <see cref="Encoding"/>.</param>
        /// <param name="directory">The resource search directory.</param>
        /// <returns>An instance of a resource.</returns>
        Task<ResourceInfo> LoadAsync(string filename, Encoding encoding, params string[] directory);
#endif
    }
}
