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
    public class TemplateReader : IResourceReader
    {
        private string resourcePath;
        private string content;
        private bool isComplete;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param> 
        public TemplateReader(string path)
        {
            this.resourcePath = path;
            this.isComplete = false;
        }

        /// <inheritdoc />
        public string ReadToEnd(Context context)
        {
            if (!isComplete)
            {
                var res = context.Load(this.resourcePath);
                if (res != null)
                {
                    content = res.Content;
                }
                isComplete = true;
            }
            return content;
        }

        #region Task based Async APIs
#if !NF40 && !NF35 && !NF20
        /// <inheritdoc />
#if NF45
        public Task<string> ReadToEndAsync(Context context)
        {
            return Task.FromResult(ReadToEnd(context));
        }
#else
        public async Task<string> ReadToEndAsync(Context context)
        {
            if (!isComplete)
            {
                var res = await context.LoadAsync(this.resourcePath);
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


        /// <inheritdoc />
        public override int GetHashCode()
        {
            return resourcePath?.GetHashCode() ?? 0;
        }
    }
}

