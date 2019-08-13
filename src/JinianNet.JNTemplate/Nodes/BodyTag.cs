/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// Body标签（配套Layout使用）
    /// </summary>
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
    }
}
