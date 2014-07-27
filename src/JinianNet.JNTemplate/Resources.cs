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
    /// <summary>
    ///资源操作
    /// </summary>
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
        /// <param name="filename">文件名 允许相对路径</param>
        /// <param name="fullPath">查找结果：完整路径</param>
        /// <returns></returns>
        private static Int32 FindPath(IEnumerable<String> paths, String filename, out String fullPath)
        {
            //filename 允许单纯的文件名或相对路径
            fullPath = null;

            if (!String.IsNullOrEmpty(filename))
            {
                filename = NormalizePath(filename);
                
                if (filename == null) //路径非法，比如用户试图跳出当前目录时（../header.txt）
                {
                    return -1;   
                }

                filename = filename.Replace('/', System.IO.Path.DirectorySeparatorChar);//替换目录分隔符 liunx应为 /header.txt win下应为 \\header.txt

                //String sc = String.Empty.PadLeft(2, System.IO.Path.DirectorySeparatorChar);
                Int32 i = 0;
                foreach (String checkUrl in paths)
                {
                    if (checkUrl[checkUrl.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                        fullPath = String.Concat(checkUrl,  filename);
                    else
                        fullPath = String.Concat(checkUrl.Remove(checkUrl.Length - 1,1), filename);
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
        /// <param name="filename">如果有目录</param>
        /// <returns></returns>
        public static String NormalizePath(String filename)
        {
            System.String normalized = filename;
            if (!normalized.StartsWith("/"))
            {
                normalized = "/" + normalized;
            }
            while (true)
            {
                Int32 index = normalized.IndexOf("//");
                if (index < 0)
                    break;
                normalized = normalized.Substring(0, (index) - (0)) + normalized.Substring(index + 1);
            }
            while (true)
            {
                Int32 index = normalized.IndexOf("%20");
                if (index < 0)
                    break;
                normalized = normalized.Substring(0, (index) - (0)) + " " + normalized.Substring(index + 3);
            }
            while (true)
            {
                Int32 index = normalized.IndexOf("/./");
                if (index < 0)
                    break;
                normalized = normalized.Substring(0, (index) - (0)) + normalized.Substring(index + 2);
            }
            while (true)
            {
                Int32 index = normalized.IndexOf("/../");
                if (index < 0)
                    break;
                if (index == 0)
                    return (null);
                Int32 index2 = normalized.LastIndexOf((System.Char)'/', index - 1);
                normalized = normalized.Substring(0, (index2) - (0)) + normalized.Substring(index + 3);
            }
            return (normalized);
        }
    }
}