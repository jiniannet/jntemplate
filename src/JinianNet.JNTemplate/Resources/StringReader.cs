/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
#if !NF35 && !NF20
using System.Threading.Tasks;
#endif
namespace JinianNet.JNTemplate.Resources
{
    /// <summary>
    /// 
    /// </summary>
    public class StringReader : /*System.IO.StringReader,*/ IResourceReader
    {
        private readonly string content;
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
        public string ReadToEnd(ITemplateContext context)
        {
            return content;
        }
#if !NF40 && !NF35 && !NF20
        /// <inheritdoc />
        public Task<string> ReadToEndAsync(ITemplateContext context)
        {
            return Task.FromResult(content);
        }
#endif
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return content?.GetHashCode() ?? 0;
        }
        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is StringReader r) 
                return this.content == r.content; 
            return false;
        }
    }
}
