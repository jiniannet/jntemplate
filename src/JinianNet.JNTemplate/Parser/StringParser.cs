/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate.Parser
{
    /// <summary>
    /// String标签分析器
    /// </summary>
    public class StringParser : ITagParser
    {
        #region ITagParser 成员
        /// <summary>
        /// 分析标签
        /// </summary>
        /// <param name="parser">TemplateParser</param>
        /// <param name="tc">Token集合</param>
        /// <returns></returns>
        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc!=null
                && tc.Count == 3
                && tc.First.TokenKind == TokenKind.StringStart
                && tc[1].TokenKind == TokenKind.String
                && tc.Last.TokenKind == TokenKind.StringEnd
                )
            {
                StringTag tag = new StringTag();
                tag.Value = tc[1].Text;
                return tag;
            }
            return null;
        }

        #endregion
    }
}