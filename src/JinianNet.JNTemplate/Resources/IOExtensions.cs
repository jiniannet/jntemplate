/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;

namespace JinianNet.JNTemplate.Resources
{
    /// <summary>
    /// 
    /// </summary>
    public static class IOExtensions
    {
        /// <summary>
        /// Returns a value that indicates whether the specified file path is fixed to a  specific drive or UNC path.
        /// </summary>
        /// <param name="path">A file path.</param>
        /// <returns>true if the path is fixed to a specific drive or UNC path; false if the path  is relative to the current drive or working directory.</returns>
        public static bool IsAbsolutePath(this string path)
        {
#if NET40 || NET20 || NETSTANDARD2_0 || NET45|| NET46 || NET47
            return IsWindowsAbsolutePath(path) || IsUnixAbsolutePath(path);
#else
            return Path.IsPathFullyQualified(path);
#endif
        }


        /// <summary>
        /// Whether the windows path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsWindowsAbsolutePath(string path)
        {
            if (path == null || path.Length < 2)
            {
                return false;
            }
            if (path[0] == Path.DirectorySeparatorChar && path[1] == path[0])
            {
                return true;
            }
            return (path.Length >= 3)
                && (path[1] == Path.VolumeSeparatorChar)
                && path[2] == Path.DirectorySeparatorChar
                && IsValidDriveChar(path[0]);

        }

        /// <summary>
        /// Returns true if the given character is a valid drive letter
        /// </summary>
        private static bool IsValidDriveChar(char value)
        {
            return ((value >= 'A' && value <= 'Z') || (value >= 'a' && value <= 'z'));
        }

        /// <summary>
        /// Whether the unix path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsUnixAbsolutePath(string path)
        {
            return path != null && path.Length > 0 && path[0] == Path.DirectorySeparatorChar;
        }
    }
}
