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
    /// The collection of token.
    /// </summary>
    [Serializable]
    public class TokenCollection : IList<Token>, IEquatable<TokenCollection>
    {
        private List<Token> list;
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenCollection"/> class
        /// </summary>
        public TokenCollection()
        {
            this.list = new List<Token>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenCollection"/> class
        /// </summary>
        /// <param name="capacity"></param>
        public TokenCollection(int capacity)
        {
            this.list = new List<Token>(capacity);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenCollection"/> class
        /// </summary>
        /// <param name="collection"></param>
        public TokenCollection(IEnumerable<Token> collection)
        {
            this.list = new List<Token>(collection);
        }
        /// <summary> 
        /// Initializes a new instance of the <see cref="TokenCollection"/> class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The <see cref="IList{Token}"/>.</param>
        /// <param name="start">The zero-based index in collection at which copying begins.</param>
        /// <param name="end">The zero-based index in collection at which copying ended.</param>
        public TokenCollection(IList<Token> collection, int start, int end)
        {
            this.list = new List<Token>(end + 1 - start);
            for (int i = start; i <= end && i < collection.Count; i++)
            {
                Add(collection[i]);
            }
        }
        /// <summary>
        /// Returns the first element of a collection.
        /// </summary>
        public Token First
        {
            get
            {
                if (Count == 0)
                {
                    return null;
                }
                return this[0];
            }
        }
        /// <summary>
        /// Returns the last element of a collection.
        /// </summary>
        public Token Last
        {
            get
            {
                if (Count == 0)
                {
                    return null;
                }
                return this[Count - 1];
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                sb.Append(this[i].ToString());
            }
            return sb.ToString();
        }

        /// <inheritdoc />
        public int IndexOf(Token item)
        {
            return this.list.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, Token item)
        {
            if (item.TokenKind != TokenKind.Space)
            {
                this.list.Insert(index, item);
            }
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            this.list.RemoveAt(index);
        }

        /// <summary>
        /// Creates a copy of a range of elements in the source <see cref="TokenCollection"/>.
        /// </summary>
        /// <param name="start">The zero-based <see cref="TokenCollection"/> index at which the range starts.</param>
        /// <param name="end">The zero-based <see cref="TokenCollection"/> index at which the range ends.</param>
        /// <returns>A copy of a range of elements in the source <see cref="TokenCollection"/>.</returns>
        public TokenCollection this[int start, int end]
        {
            get
            {
                TokenCollection tc = new TokenCollection();
                start = GetValidIndex(start);
                end = GetValidIndex(end);
                for (int i = start; i < end && i < this.Count; i++)
                {
                    tc.Add(this[i]);
                }
                return tc;
            }
        }


        /// <summary>
        /// Splits a collection into substrings that are based on the kind in the separator array.
        /// </summary>
        /// <param name="start">The zero-based <see cref="TokenCollection"/> index at which the range starts.</param>
        /// <param name="end">The zero-based <see cref="TokenCollection"/> index at which the range ends.</param>
        /// <param name="kinds">A kind array.</param>
        /// <returns></returns>
        public TokenCollection[] Split(int start, int end, params TokenKind[] kinds)
        {
            List<TokenCollection> tc = new List<TokenCollection>();
            start = GetValidIndex(start);
            end = GetValidIndex(end);
            if (end > this.Count)
            {
                end = this.Count;
            }
            int x = start, y = 0;
            var pos = new Stack<TokenKind>();
            for (int i = start; i < end; i++)
            {
                y = i;
                if (this[i].TokenKind == TokenKind.LeftParentheses ||
                    this[i].TokenKind == TokenKind.LeftBracket
                    || this[i].TokenKind == TokenKind.LeftBrace)
                {
                    pos.Push(this[i].TokenKind);
                }
                if (this[i].TokenKind == TokenKind.RightParentheses ||
                   this[i].TokenKind == TokenKind.RightBrace
                   || this[i].TokenKind == TokenKind.RightBracket
                   )
                {
                    if (pos.Count > 0 &&
                        (
                        (pos.Peek() == TokenKind.LeftParentheses && this[i].TokenKind == TokenKind.RightParentheses)
                        ||
                        (pos.Peek() == TokenKind.LeftBrace && this[i].TokenKind == TokenKind.RightBrace)
                        ||
                        (pos.Peek() == TokenKind.LeftBracket && this[i].TokenKind == TokenKind.RightBracket)
                        ))
                    {
                        pos.Pop();
                    }
                    else
                    {
                        throw new ParseException(string.Concat("syntax error near ", this[i].TokenKind.ToString(), this), this[i].BeginLine, this[i].BeginColumn);
                    }
                }
                if ((pos.Count == 0 && IsInKinds(this[i].TokenKind, kinds))
                    || (pos.Count == 1 && pos.Peek() == TokenKind.LeftBracket && this[i].TokenKind == TokenKind.LeftBracket))
                {
                    if (y > x)
                    {
                        tc.Add(this[x, y]);
                    }
                    if (this[i].TokenKind == TokenKind.LeftBracket)
                    {
                        x = i;
                        continue;
                    }
                    else
                    {
                        x = i + 1;
                        TokenCollection coll = new TokenCollection();
                        coll.Add(this[i]);
                        tc.Add(coll);
                    }
                }

                if (i == end - 1 && y >= x)
                {
                    //if (x == 0 && y == i)
                    //{
                    //    throw new ParseException(string.Concat("Unexpected  tag:", this), this[0].BeginLine, this[0].BeginColumn);
                    //}
                    tc.Add(this[x, y + 1]);
                    x = i + 1;
                }
            }

            return tc.ToArray();
        }

        private bool IsInKinds(TokenKind kind, params TokenKind[] kinds)
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

        private int GetValidIndex(int index)
        {
            if (index < 0)
            {
                return this.Count + index;
            }
            return index;
        }

        /// <inheritdoc />
        public Token this[int index]
        {
            get
            {
                return this.list[GetValidIndex(index)];
            }
            set
            {
                if (value.TokenKind != TokenKind.Space)
                {
                    this.list[GetValidIndex(index)] = value;
                }
            }
        }

        /// <inheritdoc />
        public void Add(Token item)
        {
            if (item.TokenKind != TokenKind.Space)
            {
                this.list.Add(item);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            this.list.Clear();
        }

        /// <inheritdoc />
        public bool Contains(Token item)
        {
            return this.list.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(Token[] array, int arrayIndex)
        {
            this.list.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc />
        public bool Remove(Token item)
        {
            return this.list.Remove(item);
        }

        /// <inheritdoc />
        public IEnumerator<Token> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public bool Equals(TokenCollection other)
        {
            if (other == null)
            {
                return false;
            }
            if (this.Count != other.Count)
            {
                return false;
            }
            for (int i = 0; i < other.Count; i++)
            {
                if (this[i] != other[i])
                {
                    return false;
                }
            }
            return true;
        }


        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is TokenCollection)
            {
                return this.Equals((TokenCollection)obj);
            }
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}