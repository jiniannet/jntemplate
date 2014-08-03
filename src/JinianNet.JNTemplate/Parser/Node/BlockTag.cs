/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class BlockTag : SimpleTag
    {
        private String text;
        public String TemplateContent
        {
            get { return text; }
            set { text = value; }
        }

        public override Object Parse(TemplateContext context)
        {
            using (System.IO.StringWriter writer = new StringWriter())
            {
                Render(context, writer);

                return writer.ToString();
            }
        }

        public override object Parse(Object baseValue, TemplateContext context)
        {
            return Parse(context);
        }

        public override void Parse(TemplateContext context, TextWriter write)
        {
            Render(context, write);
        }

        protected void Render(TemplateContext context, TextWriter writer)
        {
            if (!String.IsNullOrEmpty(this.TemplateContent))
            {
                TemplateLexer lexer = new TemplateLexer(this.TemplateContent);
                TemplateParser parser = new TemplateParser(lexer.Parse());

                while (parser.MoveNext())
                {
                    try
                    {
                        parser.Current.Parse(context, writer);
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
                            writer.Write(parser.Current.ToString());
                        }
                    }
                    catch (System.Exception e)
                    {
                        Exception.ParseException ex = new Exception.ParseException(e.Message);
                        if (context.ThrowExceptions)
                        {
                            throw ex;
                        }
                        else
                        {
                            context.AddError(ex);
                            writer.Write(parser.Current.ToString());
                        }
                    }
                }

            }
        }
    }
}
