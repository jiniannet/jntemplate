/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// Layout标签解析器
    /// </summary>
    public class LayoutParser : ITagParser
    {

        #region ITagParser 成员
        /// <summary>
        /// 分析标签
        /// </summary>
        /// <param name="parser">TemplateParser</param>
        /// <param name="tc">Token集合</param>
        /// <returns></returns>
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc != null
      && parser != null
      && tc.Count > 2
      && Utility.IsEqual(tc.First.Text, Field.KEY_LAYOUT)
      && (tc[1].TokenKind == TokenKind.LeftParentheses)
      && tc.Last.TokenKind == TokenKind.RightParentheses)
            {
                LayoutTag tag = new LayoutTag();
                tag.Path = parser.Read(new TokenCollection(tc, 2, tc.Count - 2));
                //if(!(path is StringTag))
                //{
                //    throw new Exception("");
                //}
                while (parser.MoveNext())
                {
                    tag.Children.Add(parser.Current);
                }
                return tag;
            }
            return null;
        }

        #endregion
    }
}