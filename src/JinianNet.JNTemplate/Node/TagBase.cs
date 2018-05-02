/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Text;

namespace JinianNet.JNTemplate.Node
{
    /// <summary>
    /// 基本标签
    /// </summary>
    public abstract class TagBase : Tag
    {
        /// <summary>
        /// 获取标签内容的字符串引用
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            if (LastToken != null && FirstToken != LastToken)
            {
                StringBuilder sb = new StringBuilder();
                Token t = FirstToken;
                sb.Append(t.ToString());
                while ((t = t.Next) != null && t != LastToken)
                {
                    sb.Append(t.ToString());
                }
                sb.Append(LastToken.ToString());
                return sb.ToString();
            }
            else
            {
                return FirstToken.ToString();
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