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
    public class ResourceReader : IReader
    {
        private string resourcePath;
        private TemplateContext ctx;
        private string content;
        private bool isComplete;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="context"></param>
        public ResourceReader(string path, TemplateContext context)
        {
            this.resourcePath = path;
            this.ctx = context;
            this.isComplete = false;
        }
        /// <inheritdoc />
        public string ReadToEnd()
        {
            if (!isComplete)
            {
                var res = ctx.Load(this.resourcePath);
                if (res != null)
                {
                    content = res.Content;
                }
                isComplete = true;
            }
            return content;
        }

        #region Task based Async APIs
#if !NF40
        /// <inheritdoc />
#if NF45
        public Task<string> ReadToEndAsync()
        {
            return Task.FromResult(ReadToEnd());
        }
#else
        public async Task<string> ReadToEndAsync()
        {
            if (!isComplete)
            {
                var res = await ctx.LoadAsync(this.resourcePath);
                if (res != null)
                {
                    content = res.Content;
                }
                isComplete = true;
            }
            return content;
        }
#endif
#endif
        #endregion
    }
}

