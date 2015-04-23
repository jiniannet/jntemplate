/*****************************************************
   Copyright (c) 2013-2015 jiniannet (http://www.jiniannet.com)

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

   Redistributions of source code must retain the above copyright notice
 *****************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate.Parser
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

            if (tc.Count > 3 && Common.ParserHelpers.IsEqual(Field.KEY_FOR, tc.First.Text))
            {

                if (tc[1].TokenKind == TokenKind.LeftParentheses
                   && tc.Last.TokenKind == TokenKind.RightParentheses)
                {
                    Int32 pos = 0,
                        start = 2,
                        end;

                    List<Tag> ts = new List<Tag>(3);

                    ForTag tag = new ForTag();
                    for (Int32 i = 2; i < tc.Count - 1; i++)
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
                        throw new Exception.ParseException(String.Concat("syntax error near for:", tc), tc.First.BeginLine, tc.First.BeginColumn);
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

                    throw new Exception.ParseException(String.Concat("for is not properly closed by a end tag:", tc), tc.First.BeginLine, tc.First.BeginColumn);
                }
                else
                {
                    throw new Exception.ParseException(String.Concat("syntax error near for:", tc), tc.First.BeginLine, tc.First.BeginColumn);
                }
            }

            return null;
        }

        #endregion
    }

}
