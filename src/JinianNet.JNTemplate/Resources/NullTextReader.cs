/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace JinianNet.JNTemplate.Resources
{
    /// <summary>
    /// 
    /// </summary>
    public class NullTextReader : TextReader,IReader
    {
        /// <inheritdoc />
        public override string ReadToEnd()
        {
            return string.Empty;
        }
        /// <inheritdoc />
        public override int Read(char[] buffer, int index, int count)
        {
            return 0;
        }
        /// <inheritdoc />
        public override string ReadLine()
        {
            return null;
        }
    }
}
