/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Resources
{
    /// <summary>
    /// The resource loader
    /// </summary>
    public class FileLoader : IResourceLoader
    {

        /// <inheritdoc />
        public string GetDirectoryName(string fullPath)
        {
            return System.IO.Path.GetDirectoryName(fullPath);
        }

        /// <inheritdoc />
        public ResourceInfo Load(string filename, Encoding encoding, params string[] directory)
        {
            ResourceInfo info = new ResourceInfo();
            if (string.IsNullOrEmpty(info.FullPath = FindPath(directory, filename)))
            {
                return null;
            }
            info.Content = LoadResource(info.FullPath, encoding);
            return info;
        }


        /// <inheritdoc />
        public string FindFullPath(string filename, params string[] directory)
        {
            return FindPath(directory, filename);
        }

        /// <summary>
        /// Find the specified file
        /// </summary>
        /// <param name="paths">The resource search directory.</param>
        /// <param name="filename">The file name.</param>
        /// <returns>The full path.</returns>
        private string FindPath(IEnumerable<string> paths, string filename)
        {
            if (IsAbsolutePath(filename) && System.IO.File.Exists(filename))
            {
                return filename;
            }
            //filename  
            string fullPath = null;

            if (!string.IsNullOrEmpty(filename))
            {
                if ((filename = NormalizePath(filename)) == null  // illegal path（../header.txt）
                    || paths==null) 
                {
                    return null;
                }

                int i = 0;
                foreach (string checkUrl in paths)
                {
                    if (checkUrl[checkUrl.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                    {
                        fullPath = string.Concat(checkUrl, filename);
                    }
                    else
                    {
                        fullPath = string.Concat(checkUrl.Remove(checkUrl.Length - 1, 1), filename);
                    }
                    if (System.IO.File.Exists(fullPath))
                    {
                        return fullPath;
                    }
                    i++;
                }

            }
            return null;
        }

        /// <summary>
        /// Load the resource
        /// </summary>
        /// <param name="fullPath">The fully qualified path or file name to load.</param>
        /// <param name="encoding">The <see cref="Encoding"/>.</param> 
        /// <returns>An string.</returns>
        private string LoadResource(string fullPath, Encoding encoding)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return System.IO.File.ReadAllText(fullPath, encoding);
        }

        /// <summary>
        /// Whether the windows path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsWindowsAbsolutePath(string path)
        {
            return path.Length > 2 && ((Char.IsLetter(path[0]) && path[1] == System.IO.Path.VolumeSeparatorChar) || (path[0] == System.IO.Path.DirectorySeparatorChar && path[1] == System.IO.Path.DirectorySeparatorChar));
        }

        /// <summary>
        /// Whether the unix path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsUnixAbsolutePath(string path)
        {
            return path.Length > 0 && path[0] == '/';
        }

        /// <summary>
        /// Whether the absolute path
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        private bool IsAbsolutePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
#if NET20 || NET40
            System.OperatingSystem osInfo = System.Environment.OSVersion;
            if (osInfo.Platform == PlatformID.Unix || osInfo.Platform == PlatformID.MacOSX)
            {
                return IsUnixAbsolutePath(path);
            }
            return IsWindowsAbsolutePath(path);
#else
            return IsUnixAbsolutePath(path) || IsWindowsAbsolutePath(path);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string NormalizePath(string filename)
        {
            if (string.IsNullOrEmpty(filename) || filename.IndexOfAny(System.IO.Path.GetInvalidPathChars()) != -1)
            {
                return null;
            }

            List<string> values = new List<string>(filename.Split('/'));

            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] == "." || string.IsNullOrEmpty(values[i]))
                {
                    values.RemoveAt(i);
                    i--;
                }
                else if (values[i] == "..")
                {
                    if (i == 0)
                    {
                        return null;
                    }
                    values.RemoveAt(i);
                    i--;
                    values.RemoveAt(i);
                    i--;
                }
            }

            values.Insert(0, string.Empty);

            return string.Join(System.IO.Path.DirectorySeparatorChar.ToString(), values.ToArray());
        }

#if NETCOREAPP || NETSTANDARD
        /// <inheritdoc />
        public async Task<ResourceInfo> LoadAsync(string filename, Encoding encoding, params string[] directory)
        {
            ResourceInfo info = new ResourceInfo();
            if (string.IsNullOrWhiteSpace(info.FullPath = FindPath(directory, filename)))
            {
                return null;
            }
            info.Content = await LoadResourceAsync(info.FullPath, encoding);
            return info;
        }


        /// <summary>
        /// Load the resource
        /// </summary>
        /// <param name="fullPath">The fully qualified path or file name to load.</param>
        /// <param name="encoding">The <see cref="Encoding"/>.</param> 
        /// <returns>An string.</returns>
        private async Task<string> LoadResourceAsync(string fullPath, Encoding encoding)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
#if NETSTANDARD && NETSTANDARD2_0
            return await Task.Run(() => {
                return System.IO.File.ReadAllText(fullPath, encoding);
            });
#else
            return await System.IO.File.ReadAllTextAsync(fullPath, encoding);
#endif
        }
#endif

    }
}