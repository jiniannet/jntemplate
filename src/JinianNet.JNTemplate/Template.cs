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
using JinianNet.JNTemplate.Context;
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate
{
    public class Template : BlockTag,ITemplate
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
            this.TemplateContent = text;
        }


        public virtual void Render(TextWriter writer)
        {
            Resources.Paths = this.Context.Paths;
            base.Render(this.Context, writer);
            Resources.Paths.Clear();
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
