/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// 简单标标签分析器
    /// </summary>
    public class CommentParser : ITagParser
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
                && tc.Count == 1
                && tc[0].TokenKind == TokenKind.Comment
                )
            {
                CommentTag tag = new CommentTag();
                tag.FirstToken = tc[0];
                return tag;
            }
            return null;
        }

        #endregion
    }
}