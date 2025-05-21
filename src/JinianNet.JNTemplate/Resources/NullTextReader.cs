/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;
using System.Threading;
#if !NF35 && !NF20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Resources
{
    /// <summary>
    /// 
    /// </summary>
    public class NullTextReader : TextReader, IResourceReader
    {
        /// <inheritdoc />
        public string ReadToEnd(ITemplateContext context)
        {
            return ReadToEnd();
        }

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
#if !NF40 && !NF35 && !NF20
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<string> ReadToEndAsync(ITemplateContext context)
        {
            return Task.FromResult(string.Empty);
        }
#endif
    }
}
