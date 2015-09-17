/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace JinianNet.JNTemplate.Parser.Node
{
    /// <summary>
    /// 标签块
    /// </summary>
    public class BlockTag : BaseTag
    {
        private String text;
        /// <summary>
        /// 模板上下文
        /// </summary>
        public String TemplateContent
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override Object Parse(TemplateContext context)
        {
            using (System.IO.StringWriter writer = new StringWriter())
            {
                Render(context, writer);

                return writer.ToString();
            }
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="write">write</param>
        public override void Parse(TemplateContext context, TextWriter write)
        {
            Render(context, write);
        }
        /// <summary>
        /// 呈现标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="writer">writer</param>
        protected void Render(TemplateContext context, TextWriter writer)
        {
            //缓存功能，待添加
            if (context == null)
            {
                writer.Write(this.TemplateContent);
                return;
            }
            Tag[] collection;

            if (!String.IsNullOrEmpty(this.TemplateContent))
            {
                TemplateLexer lexer = new TemplateLexer(this.TemplateContent, context.Config.TagFlag, context.Config.TagPrefix, context.Config.TagSuffix);

                TemplateParser parser = new TemplateParser(lexer.Parse(), context.Config.Resolver);

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