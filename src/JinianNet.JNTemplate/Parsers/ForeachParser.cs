/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.Parsers
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
                && tc.Count > 5
                && (Common.Utility.IsEqual(Field.KEY_FOREACH, tc.First.Text) || Common.Utility.IsEqual(Field.KEY_FOR, tc.First.Text))
                && tc[1].TokenKind == TokenKind.LeftParentheses
                && tc[2].TokenKind == TokenKind.TextData
                && Common.Utility.IsEqual(tc[3].Text, Field.KEY_IN)
                && tc.Last.TokenKind == TokenKind.RightParentheses)
            {

                ForeachTag tag = new ForeachTag();
                tag.Name = tc[2].Text; 
                tag.Source = parser.Read(tc[4,-1]);

                while (parser.MoveNext())
                {
                    tag.Children.Add(parser.Current);
                    if (parser.Current is EndTag)
                    {
                        return tag;
                    }
                }

                throw new Exception.ParseException(string.Concat("foreach is not properly closed by a end tag:", tc), tc.First.BeginLine, tc.First.BeginColumn);

                //else
                //{
                //    throw new Exception.ParseException(string.Concat("syntax error near foreach:", tc), tc.First.BeginLine, tc.First.BeginColumn);
                //}
            }
            return null;
        }


        #endregion
    }

}