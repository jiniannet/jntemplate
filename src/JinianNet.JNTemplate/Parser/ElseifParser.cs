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
    /// ELSE IF标签分析器
    /// </summary>
    public class ElseifParser : ITagParser
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
            if (tc.Count > 3
                && Common.ParserHelpers.IsEqual(tc.First.Text, Field.KEY_ELSEIF))
            {

                if (tc[1].TokenKind == TokenKind.LeftParentheses
                   && tc.Last.TokenKind == TokenKind.RightParentheses)
                {
                    ElseifTag tag = new ElseifTag();

                    TokenCollection coll = new TokenCollection();
                    coll.Add(tc, 2, tc.Count - 2);
                    tag.Test = parser.Read(coll);

                    return tag;
                }
                else
                {
                    throw new Exception.ParseException(String.Concat("syntax error near if:", tc), tc.First.BeginLine, tc.First.BeginColumn);
                }
            }

            return null;
        }

        #endregion
    }
}