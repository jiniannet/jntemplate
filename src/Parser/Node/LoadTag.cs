/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class LoadTag : ComplexTag
    {
        public LoadTag(Int32 line, Int32 col)
            : base(ElementType.Load, line, col)
        {

        }

        private Tag _path;
        public Tag Path
        {
            get { return _path; }
            set { _path = value; }
        }

        private TemplateContext _context;
        public TemplateContext Context
        {
            get { return _context; }
            set { _context = value; }
        }


        public override void Parse(VariableScope vars, System.IO.TextWriter writer)
        {
            String path = this.Path.Parse(vars).ToString();
            if (!String.IsNullOrEmpty(path))
            {
                TemplateLexer lexer = new TemplateLexer(Resources.LoadResource(new String[] { this.Context.CurrentPath }, path, this.Context.Charset));
                TemplateParser parser = new TemplateParser(lexer.Parse(), this.Context.Analyzer);
                while (parser.MoveNext())
                {
                    parser.Current.Parse(this.Context.TempData, writer);
                }
            }
        }
    }
}