/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Parsers;
using JinianNet.JNTemplate.Exceptions;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Provides methods for parsing template strings.
    /// </summary>
    public class TemplateParser : Executor<ITag[]>, IEnumerator<ITag>
    {
        private ITag tag;
        private Token[] tokens;
        private int index;
        private TagCollection tags;
        private TagParser tagParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateParser"/> class
        /// </summary> 
        /// <param name="p">The <see cref="TagParser"/>.</param>
        /// <param name="ts">The collection of tokens.</param>
        public TemplateParser(TagParser p, Token[] ts)
            : base()
        {
            if (ts == null)
            {
                throw new ArgumentNullException("\"ts\" cannot be null.");
            }
            this.tagParser = p;
            this.tokens = ts;
            Reset();
        }
        /// <inheritdoc />
        public ITag Current
        {
            get { return this.tag; }
        }
        /// <inheritdoc />
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
        /// Reset the <see cref="TemplateParser"/>.
        /// </summary>
        public void Reset()
        {
            this.index = 0;
            this.tag = null;
        }

        /// <summary>
        /// Reads the next tag from the tokens.
        /// </summary>
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
                catch (TemplateException)
                {
                    throw;
                }
                catch (System.Exception e)
                {
                    throw new ParseException(string.Concat("Parse error:", tc, "\r\nError message:", e.Message), tc.First.BeginLine, tc.First.BeginColumn);//标签分析异常
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
                    throw new ParseException(string.Concat("Unexpected  tag:", tc), tc.First.BeginLine, tc.First.BeginColumn); //未知的标签
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
        /// Reads the next tag from the tokens.
        /// </summary>
        /// <param name="tc">The collection of tokens.</param>
        /// <returns></returns>
        public ITag Read(TokenCollection tc)
        {
            if (tc == null || tc.Count == 0)
            {
                throw new ParseException("Invalid TokenCollection!");//无效的标签集合
            }
            return tagParser.Parsing(this, tc);
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


        /// <inheritdoc />
        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        /// <inheritdoc />
        public void Dispose()
        {

        }

        /// <inheritdoc />
        public override ITag[] Execute()
        {
            if (tags == null)
            {
                tags = new TagCollection();

                while (MoveNext())
                {
                    tags.Add(Current);
                }
            }
            return tags.ToArray();
        }
    }
}