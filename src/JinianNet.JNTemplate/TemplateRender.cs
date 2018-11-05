/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Parsers;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 基本模板呈现
    /// </summary>
    public class TemplateRender
    {
        private TemplateContext _context;
        private string _content;
        private string _key;

        /// <summary>
        /// 模板KEY(用于缓存，默认为文路径)
        /// </summary>
        public string TemplateKey
        {
            get { return this._key; }
            set { this._key = value; }
        }

        /// <summary>
        /// 模板上下文
        /// </summary>
        public TemplateContext Context
        {
            get { return this._context; }
            set { this._context = value; }
        }

        /// <summary>
        /// 模板内容
        /// </summary>
        public string TemplateContent
        {
            get { return this._content; }
            set { this._content = value; }
        }

        /// <summary>
        /// 呈现内容
        /// </summary>
        /// <param name="writer">TextWriter</param>
        public virtual void Render(System.IO.TextWriter writer)
        {
            Render(writer, ReadTags());
        }

        /// <summary>
        /// 呈现内容
        /// </summary>
        /// <param name="writer">TextWriter</param>
        /// <param name="collection">Tags</param>
        public virtual void Render(System.IO.TextWriter writer, Tag[] collection)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("\"writer\" cannot be null.");
            }

            if (collection != null && collection.Length > 0)
            {
                for (int i = 0; i < collection.Length; i++)
                {
                    try
                    {
                        collection[i].Parse(this._context, writer);
                    }
                    catch (Exception.TemplateException e)
                    {
                        ThrowException(e, collection[i], writer);
                    }
                    catch (System.Exception e)
                    {
                        System.Exception baseException = e.GetBaseException();
                        Exception.ParseException ex = new Exception.ParseException(baseException.Message, baseException);
                        ThrowException(ex, collection[i], writer);
                    }
                }
            }
        }

        /// <summary>
        /// read all tags
        /// </summary>
        /// <returns></returns>
        public Tag[] ReadTags()
        {
            if (!string.IsNullOrEmpty(this._content))
            {
                //object value;
                //if (Engine.Cache != null && !string.IsNullOrEmpty(this._key))
                //{
                //    if ((value = Engine.Cache.Get(this._key)) != null)
                //    {
                //        collection = (Tag[])value;
                //    }
                //    else
                //    {
                //        collection = ParseTag();
                //        Engine.Cache.Set(this._key, collection);
                //    }
                //}
                //else
                //{
                return ParseTags();
                //}
            }
            else
            {
                return new Tag[0];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Tag[] ParseTags()
        {
            TemplateLexer lexer = new TemplateLexer(this._content);
            Token[] ts = lexer.ToArray();
            TemplateParser parser = new TemplateParser(ts);
            return parser.ToArray();
        }

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="e">异常信息</param>
        /// <param name="tag">影响标签</param>
        /// <param name="writer">TextWriter</param>
        private void ThrowException(Exception.TemplateException e, Tag tag, System.IO.TextWriter writer)
        {
            if (this._context.ThrowExceptions)
            {
                throw e;
            }
            else
            {
                this._context.AddError(e);
                writer.Write(tag.ToString());
            }
        }
    }
}