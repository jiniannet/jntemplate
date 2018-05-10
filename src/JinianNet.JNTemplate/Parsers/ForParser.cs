/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// For标签分析器
    /// </summary>
    public class ForParser : ITagParser
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
                && tc.Count > 3 
                && Common.Utility.IsEqual(Field.KEY_FOR, tc.First.Text))
            {

                if (tc[1].TokenKind == TokenKind.LeftParentheses
                   && tc.Last.TokenKind == TokenKind.RightParentheses)
                {
                    int pos = 0,
                        start = 2,
                        end;

                    List<Tag> ts = new List<Tag>(3);

                    ForTag tag = new ForTag();
                    for (int i = 2; i < tc.Count - 1; i++)
                    {
                        end = i;
                        if (tc[i].TokenKind == TokenKind.Punctuation && tc[i].Text == ";")
                        {
                            if (pos == 0)
                            {
                                TokenCollection coll = new TokenCollection();
                                coll.Add(tc, start, end - 1);
                                if (coll.Count > 0)
                                {
                                    ts.Add(parser.Read(coll));
                                }
                                else
                                {
                                    ts.Add(null);
                                }
                                start = i + 1;
                                continue;
                            }
                        }

                        if (tc[i].TokenKind == TokenKind.LeftParentheses)
                        {
                            pos++;
                        }
                        else if (tc[i].TokenKind == TokenKind.RightParentheses)
                        {
                            pos--;
                        }
                        if (i == tc.Count - 2)
                        {
                            TokenCollection coll = new TokenCollection();
                            coll.Add(tc, start, end);
                            if (coll.Count > 0)
                            {
                                ts.Add(parser.Read(coll));
                            }
                            else
                            {
                                ts.Add(null);
                            }
                        }
                    }

                    if (ts.Count != 3)
                    {
                        throw new Exception.ParseException(string.Concat("syntax error near for:", tc), tc.First.BeginLine, tc.First.BeginColumn);
                    }

                    tag.Initial = ts[0];
                    tag.Test = ts[1];
                    tag.Do = ts[2];

                    while (parser.MoveNext())
                    {
                        tag.Children.Add(parser.Current);
                        if (parser.Current is EndTag)
                        {
                            return tag;
                        }
                    }

                    throw new Exception.ParseException(string.Concat("for is not properly closed by a end tag:", tc), tc.First.BeginLine, tc.First.BeginColumn);
                }
                else
                {
                    throw new Exception.ParseException(string.Concat("syntax error near for:", tc), tc.First.BeginLine, tc.First.BeginColumn);
                }
            }

            return null;
        }

        #endregion
    }

}