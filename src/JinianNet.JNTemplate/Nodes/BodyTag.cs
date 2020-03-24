/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// Body标签（配套Layout使用）
    /// </summary>
    [Serializable]
    public class BodyTag : ComplexTag
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="write"></param>
        public override void Parse(TemplateContext context, TextWriter write)
        {
            if (this.Children != null && this.Children.Count > 0)
            {
                for (int i = 0; i < this.Children.Count; i++)
                {
                    this.Children[i].Parse(context, write);
                }
            }
        }

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="write"></param>
        public override async Task ParseAsync(TemplateContext context, TextWriter write)
        {
            if (this.Children != null && this.Children.Count > 0)
            {
                for (int i = 0; i < this.Children.Count; i++)
                {
                    await this.Children[i].ParseAsync(context, write);
                }
            }
        }
#endif
    }
}
