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
        private TemplateContext context;
        private String content;

        /// <summary>
        /// 模板上下文
        /// </summary>
        public TemplateContext Context
        {
            get { return context; }
            set { context = value; }
        }

        /// <summary>
        /// 模板内容
        /// </summary>
        public String TemplateContent
        {
            get { return content; }
            set { content = value; }
        }

        /// <summary>
        /// 吴现内容
        /// </summary>
        /// <param name="writer"></param>
        public virtual void Render(System.IO.TextWriter writer)
        {
            //缓存功能，待添加
            if (this.Context == null)
            {
                writer.Write(this.TemplateContent);
                return;
            }

            Tag[] collection;

            if (!String.IsNullOrEmpty(this.TemplateContent))
            {

                TemplateLexer lexer = new TemplateLexer(this.TemplateContent);

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
                        collection[i].Parse(context, writer);
                    }
                    catch (Exception.TemplateException e)
                    {
                        if (context.ThrowExceptions)
                        {
                            throw e;
                        }
                        else
                        {
                            context.AddError(e);
                            writer.Write(collection[i].ToString());
                        }
                    }
                    catch (System.Exception e)
                    {
                        System.Exception baseException = e.GetBaseException();

                        Exception.ParseException ex = new Exception.ParseException(baseException.Message, baseException);
                        if (context.ThrowExceptions)
                        {
                            throw ex;
                        }
                        else
                        {
                            context.AddError(ex);
                            writer.Write(collection[i].ToString());
                        }
                    }
                }
            }
        }
    }
}
