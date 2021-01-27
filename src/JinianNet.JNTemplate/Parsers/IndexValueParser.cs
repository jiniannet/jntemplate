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
    /// 索引标签分析器
    /// </summary>
    public class IndexValueParser : ITagParser
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
                && tc.Count >= 3
                && tc[0].TokenKind == TokenKind.LeftBracket
                && tc.Last.TokenKind == TokenKind.RightBracket)
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
                    if (tc[i].TokenKind == TokenKind.RightBracket)
                    {
                        pos++;
                        continue;
                    }
                    if (tc[i].TokenKind == TokenKind.LeftBracket)
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
                IndexValueTag tag = new IndexValueTag();
                //tag.Parent = (BasisTag)parser.Read(tc[0, x]);
                tag.Index = parser.Read(tc[x + 1, y]);

                return tag;
            }

            return null;
        }

        #endregion
    }
}
