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
    /// SET标签分析器
    /// </summary>
    public class SetParser : ITagParser
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
            //支持写法：简写格式：
            //常规格式：
            if (tc.Count > 5
                && Common.ParserHelpers.IsEqual(tc.First.Text, Field.KEY_SET)
                && tc[1].TokenKind == TokenKind.LeftParentheses
                && tc[3].Text == "="
                && tc.Last.TokenKind == TokenKind.RightParentheses)
            {
                SetTag tag = new SetTag();
                tag.Name = tc[2].Text;

                TokenCollection coll = new TokenCollection();
                coll.Add(tc, 4, tc.Count - 2);

                tag.Value = parser.Read(coll);
                return tag;

            }
            else if (tc.Count == 2
                && tc.First.TokenKind == TokenKind.TextData
                && tc.Last.TokenKind == TokenKind.Operator
                && (tc.Last.Text == "++" || tc.Last.Text == "--"))
            {
                SetTag tag = new SetTag();
                tag.Name = tc.First.Text;

                ExpressionTag c = new ExpressionTag();
                c.AddChild(new VariableTag()
                {
                    FirstToken = tc.First,
                    Name = tc.First.Text
                });
                c.AddChild(new TextTag()
                {
                    FirstToken = new Token(TokenKind.Operator, tc.Last.Text[0].ToString())
                });
                c.AddChild(new NumberTag()
                {
                    Value = 1,
                    FirstToken = new Token(TokenKind.Number, "1")
                });

                tag.Value = c;
                return tag;
            }
            else if (tc.Count > 2
                && tc.First.TokenKind == TokenKind.TextData
                && tc[1].Text == "=")
            {
                SetTag tag = new SetTag();
                tag.Name = tc.First.Text;

                TokenCollection coll = new TokenCollection();
                coll.Add(tc, 2, tc.Count - 1);

                tag.Value = parser.Read(coll);
                return tag;
            }

            return null;
        }

        #endregion
    }
}