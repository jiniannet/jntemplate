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
    [Serializable]
    public class Token : IComparable<Token>
    {
        private string text;
        private int beginline;
        private int begincolumn;
        private int endline;
        private int endcolumn;
        private TokenKind tokenkind;
        private Token next;
        /// <summary>
        /// 开始行
        /// </summary>
        public int BeginLine
        {
            get { return this.beginline; }
            set { this.beginline = value; }
        }
        /// <summary>
        /// 开始列
        /// </summary>
        public int BeginColumn
        {
            get { return this.begincolumn; }
            set { this.begincolumn = value; }
        }
        /// <summary>
        /// 结束行
        /// </summary>
        public int EndLine
        {
            get { return this.endline; }
            set { this.endline = value; }
        }
        /// <summary>
        /// 结束列
        /// </summary>
        public int EndColumn
        {
            get { return this.endcolumn; }
            set { this.endcolumn = value; }
        }

        /// <summary>
        /// 文本
        /// </summary>
        public string Text
        {
            get { return this.text; }
        }
        /// <summary>
        /// TOKEN标记
        /// </summary>
        public TokenKind TokenKind
        {
            get { return this.tokenkind; }
        }
        /// <summary>
        /// TOKEN
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="text"></param>
        public Token(TokenKind kind, string text)
        {
            this.tokenkind = kind;
            this.text = text;
        }
        /// <summary>
        /// 下一个NEXT
        /// </summary>
        public Token Next
        {
            get { return this.next; }
            set { this.next = value; }
        }
        /// <summary>
        /// 获取文本值
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Text;
        }

        #region IComparable<Token> 成员
        /// <summary>
        /// 比较对象
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
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

        #endregion
    }

}