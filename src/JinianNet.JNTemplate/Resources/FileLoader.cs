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
    /// 文件加载器
    /// </summary>
    public class FileLoader : IResourceLoader
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public FileLoader()
        {

        }

        /// <summary>
        /// 获取父目录
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public string GetDirectoryName(string fullPath)
        {
            return System.IO.Path.GetDirectoryName(fullPath);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="filename">文件名,可以是纯文件名,也可以是完整的路径</param>
        /// <param name="encoding">编码</param>
        /// <param name="directory">追加查找目录</param>
        /// <returns></returns>
        public ResourceInfo Load(string filename, Encoding encoding, params string[] directory)
        {
            ResourceInfo info = new ResourceInfo();
            if (string.IsNullOrEmpty(info.FullPath = FindResource(filename, directory)))
            {
                return null;
            }
            info.Content = LoadResource(info.FullPath, encoding);
            return info;
        }

        /// <summary>
        /// 查找资源
        /// </summary>
        /// <param name="filename">文件名,可以是纯文件名,也可以是完整的路径</param>
        /// <param name="directory">追加查找目录</param>
        private string FindResource(string filename, string[] directory)
        {
            string full = null;
            if (directory != null && directory.Length > 0)
            {
                full = FindPath(directory, filename);
            }
            return full;
        }


        /// <summary>
        /// 查找指定文件
        /// </summary>
        /// <param name="paths">检索路径</param>
        /// <param name="filename">文件名 允许相对路径.路径分隔符只能使用/</param>
        /// <returns>路径索引</returns>
        private string FindPath(IEnumerable<string> paths, string filename)
        {
            if (IsAbsolutePath(filename) && System.IO.File.Exists(filename))
            {
                return filename;
            }
            //filename 允许单纯的文件名或相对路径
            string fullPath = null;

            if (!string.IsNullOrEmpty(filename))
            {
                if ((filename = NormalizePath(filename)) == null)  //路径非法，比如用户试图跳出当前目录时（../header.txt）
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
        /// 载入文件
        /// </summary>
        /// <param name="fullPath">完整文件路径</param>
        /// <param name="encoding">编码</param>
        /// <returns>文本内容</returns>
        private string LoadResource(string fullPath, Encoding encoding)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return System.IO.File.ReadAllText(fullPath, encoding);
        }

        /// <summary>
        /// 是否WIN风格绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsWindowsAbsolutePath(string path)
        {
            return path.Length > 2 && Char.IsLetter(path[0]) && path[1] == System.IO.Path.VolumeSeparatorChar;
        }

        /// <summary>
        /// 是否Unix风格绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsUnixAbsolutePath(string path)
        {
            return path.Length > 0 && path[0] == '/';
        }

        /// <summary>
        /// 是否绝对路径表达形式
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
        /// 路径处理
        /// </summary>
        /// <param name="filename">待处理文件</param>
        /// <returns>处理后的路径</returns>
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
        /// <summary>
        /// 异步资源
        /// </summary>
        /// <param name="filename">文件名,可以是纯文件名,也可以是完整的路径</param>
        /// <param name="encoding">编码</param>
        /// <param name="directory">追加查找目录</param>
        /// <returns>ResourceInfo</returns>
        public async Task<ResourceInfo> LoadAsync(string filename, Encoding encoding, params string[] directory)
        {
            ResourceInfo info = new ResourceInfo();
            if (string.IsNullOrWhiteSpace(info.FullPath = FindResource(filename, directory)))
            {
                return null;
            }
            info.Content = await LoadResourceAsync(info.FullPath, encoding);
            return info;
        }


        /// <summary>
        /// 载入文件
        /// </summary>
        /// <param name="fullPath">完整文件路径</param>
        /// <param name="encoding">编码</param>
        /// <returns>文本内容</returns>
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