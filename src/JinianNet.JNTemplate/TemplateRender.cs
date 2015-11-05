/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Parser;
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 基本模板呈现
    /// </summary>
    public class TemplateRender
    {
        private TemplateContext _context;
        private String _content;
        private String _key;

        /// <summary>
        /// 模板KEY(用于缓存，默认为文路径)
        /// </summary>
        public String TemplateKey
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
        public String TemplateContent
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
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            //缓存功能，待添加
            if (this._content == null)
            {
                return;
            }

            Tag[] collection = null;
            if (!String.IsNullOrEmpty(this._content))
            {
                Object value;
                if (!String.IsNullOrEmpty(this._key))
                {
                    if ((value = Common.CacheHelprs.Get(this._key)) != null)
                    {
                        collection = (Tag[])value;
                    }
                    else
                    {
                        collection = ParseTag();
                        Common.CacheHelprs.Set(this._key, collection);
                    }
                }
                else
                {
                    collection = ParseTag();
                }
            }
            else
            {
                collection = new Tag[0];
            }

            if (collection.Length > 0)
            {
                for (Int32 i = 0; i < collection.Length; i++)
                {
                    try
                    {
                        collection[i].Parse(this._context, writer);
                    }
                    catch (Exception.TemplateException e)
                    {
                        ThrowException(e, collection[i],writer);
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
        /// 
        /// </summary>
        /// <returns></returns>
        private Tag[] ParseTag()
        {
            TemplateLexer lexer = new TemplateLexer(this._content);
            TemplateParser parser = new TemplateParser(lexer.Parse());
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
