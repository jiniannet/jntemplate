/*****************************************************
   Copyright (c) 2013-2014 jiniannet (http://www.jiniannet.com)

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
using System.Collections.Generic;
using JinianNet.JNTemplate.Parser.Node;
using JinianNet.JNTemplate.Parser;


namespace JinianNet.JNTemplate.Parser
{
    /// <summary>
    /// TemplateParser
    /// </summary>
    public class TemplateParser : IEnumerator<Tag>
    {
        const StringComparison stringComparer = StringComparison.OrdinalIgnoreCase;

        #region private field
        private Tag tag;//当前标签
        private Token[] tokens;//tokens列表
        private Int32 index;//当前索引
        private static List<ITagParser> parsers;
        #endregion

        #region ctox
        public TemplateParser(Token[] ts)
        {
            parsers = new List<ITagParser>();
            //parsers.Add(new TestParser());
            this.tokens = ts;
            Reset();
        }
        #endregion

        #region IEnumerator<Tag> 成员
        /// <summary>
        /// 当前标签
        /// </summary>
        public Tag Current
        {
            get
            {
                return tag;
            }
        }

        #endregion

        #region IEnumerator 成员

        /// <summary>
        /// 读取下一个标签
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (this.index < this.tokens.Length)
            {
                Tag t = Read();
                if (t != null)
                {
                    this.tag = t;
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
            this.index = 0;
            this.tag = null;
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
                    this.index++;
                    t2.Next = GetToken();
                    t2 = t2.Next;

                    tc.Add(t2);


                } while (!IsTagEnd());

                tc.Remove(tc.Last);

                this.index++;

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
                this.index++;
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
                throw new Exception.ParseException("Invalid TokenCollection!");//无效的标签集合
            return Parser.Parse(this, tc);
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
            return tokens[this.index];
        }

        private Token GetToken(Int32 i)
        {
            return tokens[this.index + 1];
        }

        #endregion

        #region IEnumerator 成员

        Object System.Collections.IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        #endregion

        #region IDispose 成员
        public void Dispose()
        {

        }
        #endregion
    }
}