/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// The collection of token.
    /// </summary>
    [Serializable]
    public sealed class TokenCollection : IList<Token>, IEquatable<TokenCollection>, IEqualityComparer<TokenCollection>
    {
        private string text;
        private List<Token> list;
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenCollection"/> class
        /// </summary>
        public TokenCollection()
        {
            this.list = new List<Token>();
            text = null;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenCollection"/> class
        /// </summary>
        /// <param name="capacity"></param>
        public TokenCollection(int capacity)
        {
            this.list = new List<Token>(capacity);
            text = null;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenCollection"/> class
        /// </summary>
        /// <param name="collection"></param>
        public TokenCollection(IEnumerable<Token> collection)
        {
            this.list = new List<Token>(collection);
            text = null;
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
            text = null;
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
            if (text != null)
                return text;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                sb.Append(this[i].ToString());
            }
            text = sb.ToString();
            return text;
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
            text=null;
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            this.list.RemoveAt(index);
            text=null;
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
                text=null;
            }
        }

        /// <inheritdoc />
        public void Add(Token item)
        {
            if (item.TokenKind != TokenKind.Space)
            {
                this.list.Add(item);
            }
            text=null;
        }

        /// <inheritdoc />
        public void Clear()
        {
            this.list.Clear();
            text=null;
        }

        /// <inheritdoc />
        public bool Contains(Token item)
        {
            return this.list.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public bool Any(TokenKind kind)
        {
            return list.FindIndex(m => m.TokenKind == kind) != -1;
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
            text=null;
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
            if (obj is TokenCollection tc)
            {
                return this.Equals(tc);
            }
            return false;
        }

        /// <inheritdoc />
        public bool Equals(TokenCollection x, TokenCollection y)
        {
            if (x == null)
                return y == null;
            if (y == null)
                return false;
            return x.Equals(y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns> 
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        /// <inheritdoc />
        public int GetHashCode(TokenCollection obj)
        {
            return obj.GetHashCode();
        }
    }
}