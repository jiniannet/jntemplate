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