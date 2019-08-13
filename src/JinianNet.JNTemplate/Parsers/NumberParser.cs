/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// 数字标签分析器
    /// </summary>
    public class NumberParser : ITagParser
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
                && tc.Count == 1 
                && tc.First.TokenKind == TokenKind.Number)
            {
                NumberTag tag = new NumberTag();
                if (tc.First.Text.IndexOf('.') == -1)
                {
                    tag.Value = int.Parse(tc.First.Text);
                }
                else
                {
                    tag.Value = Double.Parse(tc.First.Text);
                }

                return tag;
            }

            return null;
        }

        #endregion
    }
}