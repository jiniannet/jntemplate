/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using JinianNet.JNTemplate.Parser;

namespace JinianNet.JNTemplate
{
    public class Template : ITemplate
    {
        private TemplateContext _context;
        public TemplateContext Context
        {
            get
            {
                return _context;
            }
            set { _context = value; }
        }

        private String _text;
        public String TemplateContent
        {
            get { return _text; }
            set { _text = value; }
        }

        public Template()
            : this(null)
        {

        }

        public Template(String text)
            : this(new TemplateContext(), text)
        {

        }

        public Template(TemplateContext context, String text)
        {
            this._context = context;
            this._text = text;
        }


        public virtual void Render(TextWriter writer)
        {
            if (!String.IsNullOrEmpty(TemplateContent))
            {
                TemplateLexer lexer = new TemplateLexer(TemplateContent);

                TemplateParser parser = new TemplateParser(lexer.Parse(), this.Context.Analyzer);

                while (parser.MoveNext())
                {
                    parser.Current.Parse(this.Context, writer);
                }
            }
        }



        public String Render()
        {
            String document;

            using (StringWriter writer = new StringWriter())
            {
                Render(writer);
                document = writer.ToString();
            }

            return document;
        }


        public void Set(String key, Object value)
        {
            Context.TempData[key] = value;
        }

        public void Set(Dictionary<String, Object> dictionary)
        {
            foreach (KeyValuePair<String, Object> value in dictionary)
            {
                Set(value.Key, value.Value);
            }
        }

    }
}
