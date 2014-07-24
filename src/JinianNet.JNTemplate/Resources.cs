/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace JinianNet.JNTemplate
{
    public class Resources
    {
        private readonly static List<String> collection = new List<string>();
        /// <summary>
        /// 资源路径
        /// </summary>
        public static List<String> Paths
        {
            get
            {
                return collection;
            }
        }

        /// <summary>
        /// 查找指定文件
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="fullPath">查找结果</param>
        /// <returns></returns>
        public static Int32 FindPath(String filename, out String fullPath)
        {
            return FindPath(Paths, filename, out fullPath);
        }
        /// <summary>
        /// 查找指定文件
        /// </summary>
        /// <param name="paths">检索路径</param>
        /// <param name="filename">文件名</param>
        /// <param name="fullPath">查找结果：完整路径</param>
        /// <returns></returns>
        private static Int32 FindPath(IEnumerable<String> paths, String filename, out String fullPath)
        {
            fullPath = null;
            if (!String.IsNullOrEmpty(filename))
            {
                filename = NormalizePath(filename);
                String sc = String.Empty.PadLeft(2, System.IO.Path.DirectorySeparatorChar);
                Int32 i = 0;
                foreach (String checkUrl in paths)
                {
                    if (checkUrl[checkUrl.Length - 1] != System.IO.Path.DirectorySeparatorChar && filename[0] != System.IO.Path.DirectorySeparatorChar)
                        fullPath = String.Concat(checkUrl, System.IO.Path.DirectorySeparatorChar.ToString(), filename);
                    else
                        fullPath = String.Concat(checkUrl, filename);
                    fullPath = fullPath.Replace('/', System.IO.Path.DirectorySeparatorChar);
                    while (fullPath.Contains(sc))
                    {
                        fullPath = fullPath.Replace(sc, System.IO.Path.DirectorySeparatorChar.ToString());
                    }

                    if (System.IO.File.Exists(fullPath))
                        return i;
                }

            }
            return -1;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static String LoadResource(String filename, Encoding encoding)
        {
            return LoadResource(Paths, filename, encoding);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="paths">检索路径</param>
        /// <param name="filename">文件名</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static String LoadResource(IEnumerable<String> paths, String filename, Encoding encoding)
        {
            String full;
            if (FindPath(paths, filename, out full) != -1)
            {
                return Load(full, encoding);
            }
            return null;
        }

        /// <summary>
        /// 载入文件
        /// </summary>
        /// <param name="filename">完整文件路径</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static String Load(String filename, Encoding encoding)
        {
            return System.IO.File.ReadAllText(filename, encoding);
        }
        /// <summary>
        /// 路径处理
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static String NormalizePath(String filename)
        {
            // Normalize the slashes and add leading slash if necessary
            System.String normalized = filename;
            if (normalized.IndexOf(System.IO.Path.DirectorySeparatorChar) >= 0)
            {
                normalized = normalized.Replace(System.IO.Path.DirectorySeparatorChar, '/');
            }

            if (!normalized.StartsWith("/"))
            {
                normalized = "/" + normalized;
            }

            // Resolve occurrences of "//" in the normalized path
            while (true)
            {
                Int32 index = normalized.IndexOf("//");
                if (index < 0)
                    break;
                normalized = normalized.Substring(0, (index) - (0)) + normalized.Substring(index + 1);
            }

            // Resolve occurrences of "%20" in the normalized path
            while (true)
            {
                Int32 index = normalized.IndexOf("%20");
                if (index < 0)
                    break;
                normalized = normalized.Substring(0, (index) - (0)) + " " + normalized.Substring(index + 3);
            }

            // Resolve occurrences of "/./" in the normalized path
            while (true)
            {
                Int32 index = normalized.IndexOf("/./");
                if (index < 0)
                    break;
                normalized = normalized.Substring(0, (index) - (0)) + normalized.Substring(index + 2);
            }

            // Resolve occurrences of "/../" in the normalized path
            while (true)
            {
                Int32 index = normalized.IndexOf("/../");
                if (index < 0)
                    break;
                if (index == 0)
                    return (null);
                // Trying to go outside our context
                Int32 index2 = normalized.LastIndexOf((System.Char)'/', index - 1);
                normalized = normalized.Substring(0, (index2) - (0)) + normalized.Substring(index + 3);
            }

            // Return the normalized path that we have completed
            return (normalized);
        }
    }
}