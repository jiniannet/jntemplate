/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 文件加载器
    /// </summary>
    public class FileLoader : ILoader
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
        public String GetDirectoryName(String fullPath)
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
        public ResourceInfo Load(String filename, Encoding encoding, params string[] directory)
        {
            ResourceInfo info = new ResourceInfo();
            string fullPath;
            if ((directory == null 
                || directory.Length == 0 
                || (info.Content = Load(directory, filename, encoding, out fullPath)) == null)
                && (info.Content = Load(this.ResourceDirectories, filename, encoding, out fullPath)) == null)
            {
                return null;
            }
            info.FullPath = fullPath;
            return info;
        }

        /// <summary>
        /// 查找指定文件
        /// </summary>
        /// <param name="filename">文件名 允许相对路径.路径分隔符只能使用/</param>
        /// <param name="fullPath">查找结果：完整路径</param>
        /// <returns>路径索引</returns>
        public Int32 FindPath(String filename, out String fullPath)
        {
            return FindPath(this.ResourceDirectories, filename, out fullPath);
        }

        /// <summary>
        /// 查找指定文件
        /// </summary>
        /// <param name="paths">检索路径</param>
        /// <param name="filename">文件名 允许相对路径.路径分隔符只能使用/</param>
        /// <param name="fullPath">查找结果：完整路径</param>
        /// <returns>路径索引</returns>
        public Int32 FindPath(IEnumerable<String> paths, String filename, out String fullPath)
        {
            //filename 允许单纯的文件名或相对路径
            fullPath = null;

            if (!String.IsNullOrEmpty(filename))
            {
                if ((filename = NormalizePath(filename)) == null)  //路径非法，比如用户试图跳出当前目录时（../header.txt）
                {
                    return -1;
                }

                Int32 i = 0;
                foreach (String checkUrl in paths)
                {
                    if (checkUrl[checkUrl.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                    {
                        fullPath = String.Concat(checkUrl, filename);
                    }
                    else
                    {
                        fullPath = String.Concat(checkUrl.Remove(checkUrl.Length - 1, 1), filename);
                    }
                    if (System.IO.File.Exists(fullPath))
                    {
                        return i;
                    }
                    i++;
                }

            }
            return -1;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="paths">检索路径</param>
        /// <param name="filename">文件名</param>
        /// <param name="encoding">编码</param>
        /// <returns>文本内容</returns>
        public String Load(IEnumerable<String> paths, String filename, Encoding encoding)
        {
            if (paths == null && String.IsNullOrEmpty(filename))
            {
                return null;
            }
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            String full;
            if (FindPath(paths, filename, out full) != -1)
            {
                return LoadResource(full, encoding);
            }
            return null;
        }


        /// <summary>
        /// 载入文件
        /// </summary>
        /// <param name="fullPath">完整文件路径</param>
        /// <param name="encoding">编码</param>
        /// <returns>文本内容</returns>
        public String LoadResource(String fullPath, Encoding encoding)
        {
            if (!System.IO.File.Exists(fullPath))
            {
                return null;
            }
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return System.IO.File.ReadAllText(fullPath, encoding);
        }

        /// <summary>
        /// 根据文件名(允许有相对路径)查找并读取文件
        /// </summary>
        /// <param name="paths">检索目录</param>
        /// <param name="filename">文件名</param>
        /// <param name="encoding">编码</param>
        /// <param name="fullName">完整路径</param>
        /// <returns></returns>
        public String Load(IEnumerable<String> paths, String filename, Encoding encoding, out String fullName)
        {
            if (IsLocalPath(filename))
            {
                fullName = filename;
            }
            else
            {
                Int32 index = FindPath(paths, filename, out fullName); //如果是相对路径，则进行路径搜索
                if (index == -1)
                {
                    return null;
                }
            }
            return LoadResource(fullName, encoding);
        }

        /// <summary>
        /// 是否WIN风格本地路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private Boolean IsWindowsLocalPath(String path)
        {
            return path.IndexOf(System.IO.Path.VolumeSeparatorChar) != -1;
        }

        /// <summary>
        /// 是否Unix风格本地路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private Boolean IsUnixLocalPath(String path)
        {
            return path[0] == '/'; ;
        }

        /// <summary>
        /// 是否本地路径表达形式
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public Boolean IsLocalPath(String path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                return false;
            }
#if NET20 || NET40
            //win系统
            System.OperatingSystem osInfo = System.Environment.OSVersion;
            if (osInfo.Platform == PlatformID.Win32NT
                || osInfo.Platform == PlatformID.Win32S
                 || osInfo.Platform == PlatformID.Win32Windows
                 || osInfo.Platform == PlatformID.WinCE
                  || osInfo.Platform == PlatformID.Xbox)
            {
                return IsWindowsLocalPath(path);
            }
            //mac or unix
            if (osInfo.Platform == PlatformID.MacOSX
               || osInfo.Platform == PlatformID.Unix)
            {
                return IsUnixLocalPath(path);
            }
#endif
            return IsUnixLocalPath(path) || IsWindowsLocalPath(path);
        }

        /// <summary>
        /// 路径处理
        /// </summary>
        /// <param name="filename">待处理文件</param>
        /// <returns>处理后的路径</returns>
        public String NormalizePath(String filename)
        {
            if (String.IsNullOrEmpty(filename) || filename.IndexOfAny(System.IO.Path.GetInvalidPathChars()) != -1)
            {
                return null;
            }

            List<String> values = new List<String>(filename.Split('/'));

            for (Int32 i = 0; i < values.Count; i++)
            {
                if (values[i] == "." || String.IsNullOrEmpty(values[i]))
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

            values.Insert(0, String.Empty);

            return String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), values.ToArray());
        }
    }
}