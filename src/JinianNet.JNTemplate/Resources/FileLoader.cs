/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JinianNet.JNTemplate.Resources
{
    /// <summary>
    /// The resource loader
    /// </summary>
    public class FileLoader : IResourceLoader
    {
        /// <inheritdoc />
        public ResourceInfo Load(Context ctx, string filename)
        {

            filename = Find(ctx, filename);

            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
            {
                return null;
            }
            return new ResourceInfo
            {
                Content = LoadResource(filename, ctx.Charset),
                FullPath = filename
            };
        }

        /// <inheritdoc />
        public string Find(Context ctx, string filename)
        {
            if (filename.IsAbsolutePath())
            {
                return filename;
            }
            string full;
            if (!string.IsNullOrEmpty(ctx.CurrentPath)
                && !string.IsNullOrEmpty(full = FindPath(filename, ctx.CurrentPath)))
            {
                return full;
            }
            return FindPath(filename, ctx.GetResourceDirectories());
        }


        /// <inheritdoc />
        public string GetDirectoryName(string fullPath)
        {
            return System.IO.Path.GetDirectoryName(fullPath);
        }

        /// <inheritdoc />
        [Obsolete]
        public ResourceInfo Load(string filename, Encoding encoding, params string[] directory)
        {
            var info = new ResourceInfo();
            if (string.IsNullOrEmpty(info.FullPath = FindPath(filename, directory)))
            {
                return null;
            }
            info.Content = LoadResource(info.FullPath, encoding);
            return info;
        }


        /// <inheritdoc />
        [Obsolete]
        public string FindFullPath(string filename, params string[] directory)
        {
            if (filename.IsAbsolutePath())
            {
                return filename;
            }
            return FindPath(filename, directory);
        }

        /// <summary>
        /// Find the specified file
        /// </summary>
        /// <param name="paths">The resource search directory.</param>
        /// <param name="filename">The file name.</param>
        /// <returns>The full path.</returns>
        private string FindPath(string filename, params string[] paths)
        {
            //filename  
            string fullPath = null;

            if (!string.IsNullOrEmpty(filename))
            {
                if ((filename = NormalizePath(filename)) == null  // illegal path（../header.txt）
                    || paths == null)
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
            return File.ReadAllText(fullPath, encoding);
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

#if !NF40 && !NF45

        /// <inheritdoc />
        [Obsolete]
        public async Task<ResourceInfo> LoadAsync(string filename, Encoding encoding, params string[] directory)
        {
            ResourceInfo info = new ResourceInfo();
            if (string.IsNullOrWhiteSpace(info.FullPath = FindPath(filename, directory)))
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
        private Task<string> LoadResourceAsync(string fullPath, Encoding encoding)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
#if NFW || NETSTANDARD2_0
            var text =  System.IO.File.ReadAllText(fullPath, encoding);
            return Task.FromResult<string>(text);
#else
            return System.IO.File.ReadAllTextAsync(fullPath, encoding);
#endif
        }
        /// <inheritdoc />
        public async Task<ResourceInfo> LoadAsync(Context ctx, string filename)
        {
            if (!filename.IsAbsolutePath())
            {
                filename = Find(ctx, filename);
            }

            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
            {
                return null;
            }
            return new ResourceInfo
            {
                Content = await LoadResourceAsync(filename, ctx.Charset),
                FullPath = filename
            };
        }
#endif

    }
}