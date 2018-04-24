/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Node;

namespace JinianNet.JNTemplate.Parser
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
        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc != null
                && tc.Count == 1 
                && tc.First.TokenKind == TokenKind.Number)
            {
                NumberTag tag = new NumberTag();
                if (tc.First.Text.IndexOf('.') == -1)
                {
                    tag.Value = Int32.Parse(tc.First.Text);
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