/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// TemplateParser
    /// </summary>
    public class TemplateParser : Executer<ITag[]>, IEnumerator<ITag>
    {
        #region private field
        private ITag _tag;//当前标签
        private Token[] _tokens;//tokens列表
        private int _index;//当前索引
        private List<ITag> Tags;
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

        ///// <summary>
        ///// 模板分模器
        ///// </summary>
        ///// <param name="lexer">lexer</param>
        //public TemplateParser(TemplateLexer lexer)
        //{
        //    if (lexer == null)
        //    {
        //        throw new ArgumentNullException(nameof(lexer));
        //    }
        //    this._tokens = await lexer.ExecuteAsync();
        //    Reset();
        //}
        #endregion

        #region IEnumerator<Tag> 成员
        /// <summary>
        /// 当前标签
        /// </summary>
        public ITag Current
        {
            get { return this._tag; }
        }

        #endregion

        #region IEnumerator 成员

        /// <summary>
        /// 读取下一个标签
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (this._index < this._tokens.Length)
            {
                ITag t = Read();
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
                    this._index++;
                    t2.Next = GetToken();
                    t2 = t2.Next;

                    tc.Add(t2);


                } while (!IsTagEnd());

                tc.Remove(tc.Last);

                this._index++;

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
                this._index++;
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
            return Engine.Resolve(this, tc);
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
            return this._tokens[this._index];
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
            if (Tags == null)
            {
                Tags = new List<ITag>();

                while (MoveNext())
                {
                    Tags.Add(Current);
                }
            }
            return Tags.ToArray();
        }
    }
}