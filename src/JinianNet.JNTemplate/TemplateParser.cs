/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Node;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// TemplateParser
    /// </summary>
    public class TemplateParser : IEnumerator<Tag>
    {
        #region private field
        private Tag _tag;//当前标签
        private Token[] _tokens;//tokens列表
        private Int32 _index;//当前索引
        #endregion

        #region ctox
        /// <summary>
        /// 模板分模器
        /// </summary>
        /// <param name="ts">TOKEN集合</param>
        public TemplateParser(Token[] ts)
        {
            if (ts == null)
            {
                throw new ArgumentNullException("\"ts\" cannot be null.");
            }
            this._tokens = ts;
            Reset();
        }
        #endregion

        #region IEnumerator<Tag> 成员
        /// <summary>
        /// 当前标签
        /// </summary>
        public Tag Current
        {
            get { return this._tag; }
        }

        #endregion

        #region IEnumerator 成员

        /// <summary>
        /// 读取下一个标签
        /// </summary>
        /// <returns></returns>
        public Boolean MoveNext()
        {
            if (this._index < this._tokens.Length)
            {
                Tag t = Read();
                if (t != null)
                {
                    this._tag = t;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            this._index = 0;
            this._tag = null;
        }

        private Tag Read()
        {
            Tag t = null;
            if (IsTagStart())
            {
                Token t1, t2;
                t1 = t2 = GetToken();
                TokenCollection tc = new TokenCollection();

                do
                {
                    this._index++;
                    t2.Next = GetToken();
                    t2 = t2.Next;

                    tc.Add(t2);


                } while (!IsTagEnd());

                tc.Remove(tc.Last);

                this._index++;

                try
                {
                    t = Read(tc);
                }
                catch (Exception.TemplateException)
                {
                    throw;
                }
                catch (System.Exception e)
                {
                    throw new Exception.ParseException(String.Concat("Parse error:", tc, "\r\nError message:", e.Message), tc.First.BeginLine, tc.First.BeginColumn);//标签分析异常
                }

                if (t != null)
                {
                    t.FirstToken = t1;
                    if (t.Children.Count == 0 || t.LastToken == null || t2.CompareTo(t.LastToken) > 0)
                    {
                        t.LastToken = t2;
                    }
                }
                else
                {
                    throw new Exception.ParseException(String.Concat("Unexpected  tag:", tc), tc.First.BeginLine, tc.First.BeginColumn); //未知的标签
                }
            }
            else
            {
                t = new TextTag();
                t.FirstToken = GetToken();
                t.LastToken = null;
                this._index++;
            }
            return t;
        }
        /// <summary>
        /// 读取一个标签
        /// </summary>
        /// <param name="tc">TOKEN集合</param>
        /// <returns></returns>
        public Tag Read(TokenCollection tc)
        {
            if (tc == null || tc.Count == 0)
            {
                throw new Exception.ParseException("Invalid TokenCollection!");//无效的标签集合
            }
            return Engine.Resolve(this, tc);
        }


        private Boolean IsTagEnd()
        {
            return IsTagEnd(GetToken());
        }

        private Boolean IsTagStart()
        {
            return IsTagStart(GetToken());
        }

        private Boolean IsTagEnd(Token t)
        {
            return t == null || t.TokenKind == TokenKind.TagEnd || t.TokenKind == TokenKind.EOF;
        }

        private Boolean IsTagStart(Token t)
        {
            return t.TokenKind == TokenKind.TagStart;
        }

        private Token GetToken()
        {
            return this._tokens[this._index];
        }

        //private Token GetToken(Int32 i)
        //{
        //    return tokens[this.index + 1];
        //}

        #endregion

        #region IEnumerator 成员

        Object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        #endregion

        #region IDispose 成员
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {

        }
        #endregion

        /// <summary>
        /// 将解析结果复制到数组中
        /// </summary>
        /// <returns>Tag[]</returns>
        public Tag[] ToArray()
        {
            List<Tag> arr = new List<Tag>();
            while (MoveNext())
            {
                arr.Add(Current);
            }
            return arr.ToArray();
        }
    }
}