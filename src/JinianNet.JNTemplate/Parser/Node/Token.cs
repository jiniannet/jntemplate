/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Parser.Node
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
            get { return _beginline; }
            set { _beginline = value; }
        }
        /// <summary>
        /// 开始列
        /// </summary>
        public Int32 BeginColumn
        {
            get { return _begincolumn; }
            set { _begincolumn = value; }
        }
        /// <summary>
        /// 结束行
        /// </summary>
        public Int32 EndLine
        {
            get { return _endline; }
            set { _endline = value; }
        }
        /// <summary>
        /// 结束列
        /// </summary>
        public Int32 EndColumn
        {
            get { return _endcolumn; }
            set { _endcolumn = value; }
        }

        /// <summary>
        /// 文本
        /// </summary>
        public String Text
        {
            get { return _text; }
        }
        /// <summary>
        /// TOKEN标记
        /// </summary>
        public TokenKind TokenKind
        {
            get { return _tokenkind; }
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
            get { return _next; }
            set { _next = value; }
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
                return 1;
            if (this.BeginLine < other.BeginLine)
                return -1;
            if (this.BeginColumn > other.BeginColumn)
                return 1;
            if (this.BeginColumn < other.BeginColumn)
                return -1;
            return 0;
        }

        #endregion
    }

}