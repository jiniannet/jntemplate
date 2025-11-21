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
    public class TemplateParser : IEnumerator<ITag>
    {
        private ITag tag;
        private readonly Token[] tokens;
        private int index;
        private TagCollection tags;
        private readonly Resolver resolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateParser"/> class
        /// </summary> 
        /// <param name="r">The <see cref="Resolver"/>.</param>
        /// <param name="ts">The collection of tokens.</param>
        public TemplateParser(Resolver r, Token[] ts)
            : base()
        {
            if (ts == null)
            {
                throw new ArgumentNullException(nameof(ts));
            }
            this.resolver = r;
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
                if (t1 == null)
                {
                    return null;
                }
                do
                {
                    this.index++;
                    t2.Next = GetToken();
                    t2 = t2.Next;
                    if (t2 == null)
                    {
                        break;
                    }
                    tc.Add(t2);


                } while (!IsTagEnd());

                if (tc.Last != null && (tc.Last.TokenKind == TokenKind.TagEnd))
                {
                    tc.Remove(tc.Last);
                }

                this.index++; 

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
                    throw new ParseException($"Parse error:{tc.ToString()}\r\nError message:{e.Message}", tc.First.BeginLine, tc.First.BeginColumn);//标签分析异常
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
                    throw new ParseException($"Unexpected  tag: {tc.ToString()}", tc.First.BeginLine, tc.First.BeginColumn); //未知的标签
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
        public ITag ReadSimple(TokenCollection tc)
        {
            return Read(tc, m => m.IsSimple);
        }

        /// <summary>
        /// Reads the next tag from the tokens.
        /// </summary>
        /// <param name="tc">The collection of tokens.</param>
        /// <returns></returns>
        public ITag Read(TokenCollection tc)
        {
            return Read(tc, null);
        }
        /// <summary>
        /// Reads the next tag from the tokens.
        /// </summary>
        /// <param name="tc">The collection of tokens.</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public ITag Read(TokenCollection tc, Func<ITag, bool> func)
        {
            if (tc == null || tc.Count == 0)
            {
                throw new ParseException("Invalid TokenCollection!");
            }
            var t = resolver.Parsing(this, tc.TrimParentheses());//func
            if (func != null && !func(t))
                return null;
            return t;
        } 
        private bool IsTagEnd()
        {
            return IsTagEnd(GetToken());
        }
        private static bool IsTagEnd(Token t)
        {
            return t == null || t.TokenKind == TokenKind.TagEnd || t.TokenKind == TokenKind.EOF;
        }

        private bool IsTagStart()
        {
            return IsTagStart(GetToken());
        }
        private static bool IsTagStart(Token t)
        {
            return t != null && t.TokenKind == TokenKind.TagStart;
        }

        private Token GetToken()
        {
            if (this.index >= this.tokens.Length)
            {
                return null;
            }
            return this.tokens[this.index];
        }


        /// <inheritdoc />
        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        /// <inheritdoc /> 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <inheritdoc/>  
        protected virtual void Dispose(bool disposing)
        { 

        }

        /// <inheritdoc />
        public virtual TagCollection Execute()
        {
            if (tags == null)
            {
                tags = new TagCollection();

                while (MoveNext())
                {
                    tags.Add(Current);
                }
            }
            return tags;
        }
    }
}