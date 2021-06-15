/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using JinianNet.JNTemplate.Exceptions;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 
    /// </summary>
    public static class TokenCollectionExtensions
    {

        /// <summary>
        /// Splits a collection into substrings that are based on the kind in the separator array.
        /// </summary>
        /// <param name="coll">The <see cref="TokenCollection"/>.</param>
        /// <param name="kinds">A kind array.</param>
        /// <returns></returns>
        public static TokenCollection[] Split(this TokenCollection coll, params TokenKind[] kinds)
        {
            return Split(coll, 0, coll.Count, kinds);
        }

        /// <summary>
        /// Splits a collection into substrings that are based on the kind in the separator array.
        /// </summary>
        /// <param name="coll">The <see cref="TokenCollection"/>.</param>
        /// <param name="start">The zero-based <see cref="TokenCollection"/> index at which the range starts.</param>
        /// <param name="end">The zero-based <see cref="TokenCollection"/> index at which the range ends.</param>
        /// <param name="kinds">A kind array.</param>
        /// <returns></returns>
        public static TokenCollection[] Split(this TokenCollection coll, int start, int end, params TokenKind[] kinds)
        {
            //var kinds = new TokenKind[] { TokenKind.Arithmetic, TokenKind.Colon, TokenKind.Comma, TokenKind.Dot, TokenKind.Logic, TokenKind.Punctuation };
            List<TokenCollection> tcs = new List<TokenCollection>();
            //start =  GetValidIndex(start);
            //end = GetValidIndex(end);
            if (end > coll.Count)
            {
                end = coll.Count;
            }
            int x = start, y = 0;
            var pos = new Stack<TokenKind>();
            for (int i = start; i < end; i++)
            {
                y = i;
                if (coll[i].TokenKind == TokenKind.LeftParentheses ||
                    coll[i].TokenKind == TokenKind.LeftBracket
                    || coll[i].TokenKind == TokenKind.LeftBrace)
                {
                    pos.Push(coll[i].TokenKind);
                    if (pos.Count == 1 && (IsInKinds(coll[i].TokenKind, kinds)))
                    {
                        if (x != y)
                        {
                            tcs.Add(coll[x, y]);
                            x = y;
                        }
                        continue;
                    }
                }
                if (coll[i].TokenKind == TokenKind.RightParentheses ||
                   coll[i].TokenKind == TokenKind.RightBrace
                   || coll[i].TokenKind == TokenKind.RightBracket
                   )
                {
                    if (pos.Count > 0 &&
                        (
                        (pos.Peek() == TokenKind.LeftParentheses && coll[i].TokenKind == TokenKind.RightParentheses)
                        ||
                        (pos.Peek() == TokenKind.LeftBrace && coll[i].TokenKind == TokenKind.RightBrace)
                        ||
                        (pos.Peek() == TokenKind.LeftBracket && coll[i].TokenKind == TokenKind.RightBracket)
                        ))
                    {
                        pos.Pop();
                        if (pos.Count == 0 && (IsInKinds(coll[i].TokenKind, kinds)))
                        {
                            if (x != y)
                            {
                                tcs.Add(coll[x, ++y]);
                                x = y;
                            }
                            continue;
                        }
                    }
                    else
                    {
                        throw new ParseException($"syntax error near {coll[i].ToString()} on {coll.ToString()}", coll[i].BeginLine, coll[i].BeginColumn);
                    }
                }
                if (pos.Count == 0 && IsInKinds(coll[i].TokenKind, kinds))
                {
                    if (y > x)
                    {
                        tcs.Add(coll[x, y]);
                    }
                    x = i + 1;
                    var child = new TokenCollection();
                    child.Add(coll[i]);
                    tcs.Add(child);
                }

                if (i == end - 1 && y >= x)
                {
                    tcs.Add(coll[x, y + 1]);
                    x = i + 1;
                }
            }
            return tcs.ToArray();
        }

        private static bool IsInKinds(TokenKind kind, params TokenKind[] kinds)
        {
            if (kinds == null || kinds.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < kinds.Length; i++)
            {
                if (kind == kinds[i])
                {
                    return true;
                }
            }
            return false;
        }
    }
}
