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
        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc != null
                && parser != null
                && tc.First.TokenKind == TokenKind.TextData
                && tc.Count > 2
                && tc.Last.TokenKind == TokenKind.RightParentheses)
            {
                int y = tc.Count - 1;
                int x = -1;
                int pos = 0;

                for (int i = y; i >= 0; i--)
                {
                    if (tc[i].TokenKind == TokenKind.Dot && pos == 0)
                    {
                        return null;
                    }
                    if (tc[i].TokenKind == TokenKind.RightParentheses)
                    {
                        pos++;
                        continue;
                    }
                    if (tc[i].TokenKind == TokenKind.LeftParentheses)
                    {
                        pos--;
                        if (pos == 0 && x == -1)
                        {
                            x = i;
                        }
                    }
                }

                if (x == -1)
                {
                    return null;
                }
                FunctaionTag tag = new FunctaionTag();

                //tag.Name = tc.First.Text; 
                tag.Func = (SimpleTag)parser.Read(tc[0, x]);

                pos = 0;

                TokenCollection[] tcs = tc.Split(x + 1, tc.Count-1, TokenKind.Comma);
                for (int i = 0; i < tcs.Length; i++)
                {
                    if(tcs[i].Count==1 && tcs[i][0].TokenKind == TokenKind.Comma)
                    {
                        continue;
                    }
                    tag.AddChild(parser.Read(tcs[i]));
                }

                //int start = x + 1;
                //int end = -1;

                //for (int i = start; i < tc.Count; i++)
                //{
                //    end = i;
                //    switch (tc[i].TokenKind)
                //    {
                //        case TokenKind.Comma:
                //            if (pos == 0)
                //            {
                //                TokenCollection coll = tc[start, end]; 
                //                if (coll.Count > 0)
                //                {
                //                    tag.AddChild(parser.Read(coll));
                //                }
                //                start = i + 1;
                //            }
                //            break;
                //        default:
                //            if (tc[i].TokenKind == TokenKind.LeftParentheses)
                //            {
                //                pos++;
                //            }
                //            else if (tc[i].TokenKind == TokenKind.RightParentheses)
                //            {
                //                pos--;
                //            }
                //            if (i == tc.Count - 1)
                //            {
                //                TokenCollection coll = tc[start, end]; 
                //                if (coll.Count > 0)
                //                {
                //                    tag.AddChild(parser.Read(coll));
                //                }
                //            }
                //            break;
                //    }

                //}

                return tag;

            }

            return null;
        }

        #endregion
    }
}