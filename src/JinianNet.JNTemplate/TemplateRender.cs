using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Parser;
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 基本模板
    /// </summary>
    public class TemplateRender
    {
        private TemplateContext _context;
        private String _content;

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

            Tag[] collection;
            //缓存功能，待添加
            if (this._content == null)
            {
                writer.Write(this.TemplateContent);
                return;
            }

            if (!String.IsNullOrEmpty(this._content))
            {
                TemplateLexer lexer = new TemplateLexer(this._content);
                TemplateParser parser = new TemplateParser(lexer.Parse());
                collection = parser.ToArray();
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
