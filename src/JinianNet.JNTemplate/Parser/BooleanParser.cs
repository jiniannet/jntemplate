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
    /// Boolean标签分析器
    /// </summary>
    public class BooleanParser : ITagParser
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
            if (tc.Count == 1
                && (tc.First.Text == "true" || tc.First.Text == "false"))
            {
                BooleanTag tag = new BooleanTag();
                tag.Value = tc.First.Text == "true";
                return tag;
            }

            return null;
        }

        #endregion
    }
}