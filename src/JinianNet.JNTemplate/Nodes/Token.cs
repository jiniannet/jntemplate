/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// TOKEN
    /// </summary>
    [Serializable]
    public class Token : IComparable<Token>, IEqualityComparer<Token>
    {
        /// <summary>
        /// Gets or sets the start line of the token.
        /// </summary>
        public int BeginLine { get; set; }
        /// <summary>
        /// Gets or sets the start cloumn of the token.
        /// </summary>
        public int BeginColumn { get; set; }
        /// <summary>
        /// Gets or sets the end line of the token.
        /// </summary>
        public int EndLine { get; set; }
        /// <summary>
        /// Gets or sets the end cloumn of the token.
        /// </summary>
        public int EndColumn { get; set; }
        /// <summary>
        /// Gets or sets the text of the token.
        /// </summary>
        public string Text { get; private set; }
        /// <summary>
        /// Gets or sets the kind of the token.
        /// </summary>
        public TokenKind TokenKind { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class
        /// </summary>
        /// <param name="kind">The <see cref="TokenKind"/>.</param>
        /// <param name="text">The text.</param>
        public Token(TokenKind kind, string text)
        {
            this.TokenKind = kind;
            this.Text = text;
        }
        /// <summary>
        /// Gets or sets the  next element of the collection.
        /// </summary>
        public Token Next { get; set; }
        /// <inheritdoc />
        public override string ToString()
        {
            return this.Text;
        }
        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is Token t)
            {
                if (CompareTo(t) != 0)
                    return false;
                return this.Text == t.Text;
            }
            return false;
        }
        /// <inheritdoc />
        public int CompareTo(Token other)
        {
            if (this.BeginLine > other.BeginLine)
            {
                return 1;
            }
            if (this.BeginLine < other.BeginLine)
            {
                return -1;
            }
            if (this.BeginColumn > other.BeginColumn)
            {
                return 1;
            }
            if (this.BeginColumn < other.BeginColumn)
            {
                return -1;
            }
            return 0;
        }
        /// <inheritdoc />
        public bool Equals(Token x, Token y)
        {
            if (x == null)
                return y == null;
            if (y == null)
                return false;
            return x.Equals(y);
        }
        /// <inheritdoc />
        public int GetHashCode(Token obj)
        {
            return obj.GetHashCode();
        }
        /// <inheritdoc />
        public override int GetHashCode()
        {
            if (Text!=null)
                return Text.GetHashCode();
            return 0;
        }
        /// <inheritdoc />
        public static bool operator ==(Token left, Token right)
        {
            if (object.ReferenceEquals(left, null))
            {
                return object.ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }
        /// <inheritdoc />
        public static bool operator >(Token left, Token right)
        {
            if (left == null || right == null)
                return false;
            return left.CompareTo(right) > 0;
        }
        /// <inheritdoc />
        public static bool operator >=(Token left, Token right)
        {
            if (left == null || right == null)
                return left == null && right == null;
            return left.CompareTo(right) >= 0;
        }
        /// <inheritdoc />
        public static bool operator <(Token left, Token right)
        {
            if (left == null || right == null)
                return false;
            return left.CompareTo(right) < 0;
        }
        /// <inheritdoc />
        public static bool operator <=(Token left, Token right)
        {
            if (left == null || right == null)
                return left == null && right == null;
            return left.CompareTo(right) <= 0;
        }
        /// <inheritdoc />
        public static bool operator !=(Token left, Token right)
        {
            return !(left == right);
        }
    }

}