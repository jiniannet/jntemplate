/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// string标签分析器
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
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc != null
                && tc.Count > 1
                && tc.First.TokenKind == TokenKind.StringStart
                && tc.Last.TokenKind == TokenKind.StringEnd
                )
            {
                StringTag tag = new StringTag();
                if (tc.Count == 3 && tc[1].TokenKind == TokenKind.String)
                {
                    tag.Value = tc[1].Text;
                }
                else
                {
                    tag.Value = "";
                }
                return tag;
            }
            return null;
        }

        #endregion
    }
}