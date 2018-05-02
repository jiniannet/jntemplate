/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// TOKEN
    /// </summary>
    public class Token : IComparable<Token>
    {
        private String _text;
        private Int32 _beginline;
        private Int32 _begincolumn;
        private Int32 _endline;
        private Int32 _endcolumn;
        private TokenKind _tokenkind;
        private Token _next;
        /// <summary>
        /// 开始行
        /// </summary>
        public Int32 BeginLine
        {
            get { return this._beginline; }
            set { this._beginline = value; }
        }
        /// <summary>
        /// 开始列
        /// </summary>
        public Int32 BeginColumn
        {
            get { return this._begincolumn; }
            set { this._begincolumn = value; }
        }
        /// <summary>
        /// 结束行
        /// </summary>
        public Int32 EndLine
        {
            get { return this._endline; }
            set { this._endline = value; }
        }
        /// <summary>
        /// 结束列
        /// </summary>
        public Int32 EndColumn
        {
            get { return this._endcolumn; }
            set { this._endcolumn = value; }
        }

        /// <summary>
        /// 文本
        /// </summary>
        public String Text
        {
            get { return this._text; }
        }
        /// <summary>
        /// TOKEN标记
        /// </summary>
        public TokenKind TokenKind
        {
            get { return this._tokenkind; }
        }
        /// <summary>
        /// TOKEN
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="text"></param>
        public Token(TokenKind kind, String text)
        {
            this._tokenkind = kind;
            this._text = text;
        }
        /// <summary>
        /// 下一个NEXT
        /// </summary>
        public Token Next
        {
            get { return this._next; }
            set { this._next = value; }
        }
        /// <summary>
        /// 获取文本值
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return this.Text;
        }

        #region IComparable<Token> 成员
        /// <summary>
        /// 比较对象
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Int32 CompareTo(Token other)
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

        #endregion
    }

}