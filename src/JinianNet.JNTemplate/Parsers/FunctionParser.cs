/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// Function标签分析器
    /// </summary>
    public class FunctionParser : ITagParser
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
                && tc.First.TokenKind == TokenKind.TextData
                && tc.Count > 2
                && tc[1].TokenKind == TokenKind.LeftParentheses
                && tc.Last.TokenKind == TokenKind.RightParentheses
                && tc.Split(0, tc.Count, TokenKind.Operator).Length == 1)
            {
                FunctaionTag tag = new FunctaionTag();

                tag.Name = tc.First.Text; 
                //tag.Func = (BasisTag)parser.Read(tc[0, 1]);

                TokenCollection[] tcs = tc.Split(2, tc.Count - 1, TokenKind.Comma);
                for (int i = 0; i < tcs.Length; i++)
                {
                    if (tcs[i].Count == 1 && tcs[i][0].TokenKind == TokenKind.Comma)
                    {
                        continue;
                    }
                    tag.AddChild(parser.Read(tcs[i]));
                }

                return tag;

            }

            return null;
        }

        #endregion
    }
}