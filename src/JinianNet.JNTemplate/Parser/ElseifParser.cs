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
    /// ELSE IF标签分析器
    /// </summary>
    public class ElseifParser : ITagParser
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
            if (tc.Count > 3
                && Common.ParserHelpers.IsEqual(tc.First.Text, Field.KEY_ELSEIF))
            {

                if (tc[1].TokenKind == TokenKind.LeftParentheses
                   && tc.Last.TokenKind == TokenKind.RightParentheses)
                {
                    ElseifTag tag = new ElseifTag();

                    TokenCollection coll = new TokenCollection();
                    coll.Add(tc, 2, tc.Count - 2);
                    tag.Test = parser.Read(coll);

                    return tag;
                }
                else
                {
                    throw new Exception.ParseException(String.Concat("syntax error near if:", tc), tc.First.BeginLine, tc.First.BeginColumn);
                }
            }

            return null;
        }

        #endregion
    }
}
