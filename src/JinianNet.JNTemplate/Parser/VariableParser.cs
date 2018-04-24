/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Node;

namespace JinianNet.JNTemplate.Parser
{

    /// <summary>
    /// Variable标签分析器
    /// </summary>
    public class VariableParser : ITagParser
    {
        #region ITagParser 成员
        /// <summary>
        /// 分析标签
        /// </summary>
        /// <param name="parser">TemplateParser</param>
        /// <param name="tc">Token集合</param>
        /// <returns>标签</returns>
        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc != null
                && tc.Count == 1
                && tc.First.TokenKind == TokenKind.TextData)
            {
                VariableTag tag = new VariableTag();
                tag.Name = tc.First.Text;
                return tag;
            }
            return null;
        }

        #endregion
    }
}