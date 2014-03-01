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
        public class ResourceManager
        {


        }


        public static int FindPath(String[] files, String path, out String fullPath)
        {
            fullPath = null;
            if (!String.IsNullOrEmpty(path))
            {
                String checkUrl;
                //String appPath;
                //if (System.Web.HttpContext.Current != null)
                //{
                //    appPath = System.Web.HttpRuntime.AppDomainAppPath;
                //}
                path = NormalizePath(path);
                String sc = String.Empty.PadLeft(2, System.IO.Path.DirectorySeparatorChar);
                for (Int32 i = 0; i < files.Length && !String.IsNullOrEmpty(files[i]); i++)
                {
                    checkUrl = files[i];
                    //if (checkUrl.IndexOf(System.IO.Path.VolumeSeparatorChar) == -1)
                    //{
                    //    //checkUrl = 
                    //}
                    if (checkUrl[checkUrl.Length - 1] != System.IO.Path.DirectorySeparatorChar && path[0] != System.IO.Path.DirectorySeparatorChar)
                        fullPath = String.Concat(checkUrl, System.IO.Path.DirectorySeparatorChar.ToString(), path);
                    else
                        fullPath = String.Concat(checkUrl, path);
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

        public static String LoadResource(String[] files, String path)
        {
            return LoadResource(files, path, Encoding.Default);
        }

        public static String LoadResource(String[] files, String path, Encoding encoding)
        {
            String full;
            if (FindPath(files, path, out full) != -1)
            {
                return Load(full, encoding);
            }
            return null;
        }

        public static String Load(String path, Encoding encoding)
        {
            return System.IO.File.ReadAllText(path, encoding);
        }


        public static String NormalizePath(String path)
        {
            // Normalize the slashes and add leading slash if necessary
            System.String normalized = path;
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

        public class ResourceCollection : ICollection
        {
            #region ICollection 成员

            public void CopyTo(Array array, Int32 index)
            {
                throw new NotImplementedException();
            }

            public Int32 Count
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsSynchronized
            {
                get { throw new NotImplementedException(); }
            }

            public Object SyncRoot
            {
                get { throw new NotImplementedException(); }
            }

            #endregion

            #region IEnumerable 成员

            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        //public interface ICodeParser
        //{
        //    //CodeCompileUnit Parse(TextReader codeStream);
        //}



        //public sealed class Executor
        //{
        //    //// Fields
        //    //private const Int32 ProcessTimeOut = 0x927c0;

        //    //// Methods
        //    //private Executor();
        //    //private static FileStream CreateInheritedFile(String file);
        //    //public static void ExecWait(String cmd, TempFileCollection tempFiles);
        //    //public static Int32 ExecWaitWithCapture(String cmd, TempFileCollection tempFiles, ref String outputName, ref String errorName);
        //    //public static Int32 ExecWaitWithCapture(IntPtr userToken, String cmd, TempFileCollection tempFiles, ref String outputName, ref String errorName);
        //    //public static Int32 ExecWaitWithCapture(String cmd, String currentDir, TempFileCollection tempFiles, ref String outputName, ref String errorName);
        //    //public static Int32 ExecWaitWithCapture(IntPtr userToken, String cmd, String currentDir, TempFileCollection tempFiles, ref String outputName, ref String errorName);
        //    //internal static Int32 ExecWaitWithCapture(SafeUserTokenHandle userToken, String cmd, String currentDir, TempFileCollection tempFiles, ref String outputName, ref String errorName, String trueCmdLine);
        //    //private static Int32 ExecWaitWithCaptureUnimpersonated(SafeUserTokenHandle userToken, String cmd, String currentDir, TempFileCollection tempFiles, ref String outputName, ref String errorName, String trueCmdLine);
        //    //internal static String GetRuntimeInstallDirectory();
        //    //internal static void ReImpersonate(WindowsImpersonationContext impersonation);
        //    //internal static WindowsImpersonationContext RevertImpersonation();
        //}
    }
}