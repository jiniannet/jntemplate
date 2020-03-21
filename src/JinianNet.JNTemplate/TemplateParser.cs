/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Parsers;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// TemplateParser
    /// </summary>
    public class TemplateParser : Executer<ITag[]>, IEnumerator<ITag>
    {
        #region private field
        private ITag tag;//当前标签
        private Token[] tokens;//tokens列表
        private int index;//当前索引
        private List<ITag> tags;
        private TagParser tagParse;
        #endregion

        #region ctox
        /// <summary>
        /// 模板分析器
        /// </summary>
        /// <param name="parser">标签分析器</param>
        /// <param name="ts">TOKEN集合</param>
        public TemplateParser(TagParser parser, Token[] ts)
            : base()
        {
            if (ts == null)
            {
                throw new ArgumentNullException("\"ts\" cannot be null.");
            }
            this.tagParse = parser;
            this.tokens = ts;
            Reset();
        }
        #endregion

        #region IEnumerator<Tag> 成员
        /// <summary>
        /// 当前标签
        /// </summary>
        public ITag Current
        {
            get { return this.tag; }
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
                ITag t = Read();
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

        private ITag Read()
        {
            ITag t = null;
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

                //if (tc.Count == 1 && tc[0] != null && tc[0].TokenKind == TokenKind.Comment)
                //{
                //    return new TextTag();
                //}

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
                    throw new Exception.ParseException(string.Concat("Parse error:", tc, "\r\nError message:", e.Message), tc.First.BeginLine, tc.First.BeginColumn);//标签分析异常
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
                    throw new Exception.ParseException(string.Concat("Unexpected  tag:", tc), tc.First.BeginLine, tc.First.BeginColumn); //未知的标签
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
        public ITag Read(TokenCollection tc)
        {
            if (tc == null || tc.Count == 0)
            {
                throw new Exception.ParseException("Invalid TokenCollection!");//无效的标签集合
            }
            return this.tagParse.Parsing(this, tc);
        }


        private bool IsTagEnd()
        {
            return IsTagEnd(GetToken());
        }

        private bool IsTagStart()
        {
            return IsTagStart(GetToken());
        }

        private bool IsTagEnd(Token t)
        {
            return t == null || t.TokenKind == TokenKind.TagEnd || t.TokenKind == TokenKind.EOF;
        }

        private bool IsTagStart(Token t)
        {
            return t.TokenKind == TokenKind.TagStart;
        }

        private Token GetToken()
        {
            return this.tokens[this.index];
        }

        //private Token GetToken(int i)
        //{
        //    return tokens[this.index + 1];
        //}

        #endregion

        #region IEnumerator 成员

        object System.Collections.IEnumerator.Current
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
        [Obsolete("This method has been deprecated. Please use Execute() instead")]
        public ITag[] ToArray()
        {
            return Execute();
        }

        /// <summary>
        /// 执行TAG解析
        /// </summary>
        /// <returns></returns>
        public override ITag[] Execute()
        {
            if (tags == null)
            {
                tags = new List<ITag>();

                while (MoveNext())
                {
                    tags.Add(Current);
                }
            }
            return tags.ToArray();
        }
    }
}