/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Threading.Tasks;

namespace JinianNet.JNTemplate.Resources
{
    /// <summary>
    /// 
    /// </summary>
    public class StringReader : /*System.IO.StringReader,*/ IReader
    {
        private string content;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public StringReader(string text)
        //:base(text)
        {
            content = text;
        }


        /// <inheritdoc />
        public string ReadToEnd()
        {
            return content;
        }
#if !NF40
        /// <inheritdoc />
        public Task<string> ReadToEndAsync()
        {
            return Task.FromResult(content);
        }
#endif
    }
}
