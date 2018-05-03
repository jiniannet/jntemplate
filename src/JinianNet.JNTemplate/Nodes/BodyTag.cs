/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// Body标签（配套Layout使用）
    /// </summary>
    public class BodyTag : TagBase
    {
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override object Parse(TemplateContext context)
        {
            if (this.Children != null && this.Children.Count > 0)
            {
                using (System.IO.StringWriter write = new System.IO.StringWriter())
                {
                    for (Int32 i = 0; i < this.Children.Count; i++)
                    {
                        this.Children[i].Parse(context, write);
                    }
                    return write.ToString();
                }
            }
            return null;
        }
    }
}
