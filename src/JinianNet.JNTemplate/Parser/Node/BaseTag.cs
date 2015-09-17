/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    /// <summary>
    /// 基本标签
    /// </summary>
    public abstract class BaseTag : Tag
    {
        /// <summary>
        /// 获取标签内容的字符串引用
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            if (this.LastToken != null && this.FirstToken != this.LastToken)
            {
                StringBuilder sb = new StringBuilder();
                Token t = this.FirstToken;
                sb.Append(t.ToString());
                while ((t = t.Next) != null && t != this.LastToken)
                {
                    sb.Append(t.ToString());
                }
                sb.Append(this.LastToken.ToString());
                return sb.ToString();
            }
            else
            {
                return this.FirstToken.ToString();
            }
        }

        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="write">write</param>
        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {
            write.Write(Parse(context));
        }
    }
}