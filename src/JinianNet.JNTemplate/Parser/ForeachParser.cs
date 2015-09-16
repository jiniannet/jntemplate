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
    /// Foreach标签分析器
    /// </summary>
    public class ForeachParser : ITagParser
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
            if (tc != null
                && parser != null
                && tc.Count > 0 
                && Common.ParserHelpers.IsEqual(Field.KEY_FOREACH, tc.First.Text))
            {
                if (tc.Count > 5
                    && tc[1].TokenKind == TokenKind.LeftParentheses
                    && tc[2].TokenKind == TokenKind.TextData
                    && Common.ParserHelpers.IsEqual(tc[3].Text, Field.KEY_IN)
                    && tc.Last.TokenKind == TokenKind.RightParentheses)
                {
                    ForeachTag tag = new ForeachTag();
                    tag.Name = tc[2].Text;
                    TokenCollection coll = new TokenCollection();
                    coll.Add(tc, 4, tc.Count - 2);
                    tag.Source = parser.Read(coll);

                    while (parser.MoveNext())
                    {
                        tag.Children.Add(parser.Current);
                        if (parser.Current is EndTag)
                        {
                            return tag;
                        }
                    }

                    throw new Exception.ParseException(String.Concat("foreach is not properly closed by a end tag:", tc), tc.First.BeginLine, tc.First.BeginColumn);
                }
                else
                {
                    throw new Exception.ParseException(String.Concat("syntax error near foreach:", tc), tc.First.BeginLine, tc.First.BeginColumn);
                }

            }

            return null;
        }


        #endregion
    }

}