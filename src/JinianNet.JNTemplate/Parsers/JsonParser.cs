/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.Parsers
{
    class JsonParser : ITagParser
    {
        #region ITagParser 成员
        /// <summary>
        /// JSON分析标签
        /// </summary>
        /// <param name="parser">TemplateParser</param>
        /// <param name="tc">Token集合</param>
        /// <returns></returns>
        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count > 2
                         && (tc[0].TokenKind == TokenKind.LeftBrace)
                         && tc.Last.TokenKind == TokenKind.RightBrace)
            {
                JsonTag tag = new JsonTag();
                TokenCollection[] tcs = tc.Split(1, tc.Count - 1, TokenKind.Comma);
                for (int i = 0; i < tcs.Length; i++)
                {
                    if (tcs[i].Count == 1 && tcs[i][0].TokenKind == TokenKind.Comma)
                    {
                        continue;
                    }
                    TokenCollection[] keyValuePair = tcs[i].Split(0, tcs[i].Count, TokenKind.Colon);
                    if (keyValuePair.Length != 3)
                    {
                        //不符合规范
                        return null;
                    }
                    var key = parser.Read(keyValuePair[0]);
                    var value = parser.Read(keyValuePair[2]);
                    tag.Dict.Add(key, value);
                }
                return tag;
            }

            return null;
        }
        #endregion
    }
}