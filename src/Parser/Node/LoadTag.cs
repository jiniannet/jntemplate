/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Text;
using JinianNet.JNTemplate.Context;
using JinianNet.JNTemplate.Context;

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



        public override void Parse(TemplateContext context, System.IO.TextWriter writer)
        {
            String path = this.Path.Parse(context).ToString();
            if (!String.IsNullOrEmpty(path))
            {
                System.Collections.Generic.List<string> paths = new System.Collections.Generic.List<string>();

                if (!String.IsNullOrEmpty(context.CurrentPath))
                    paths.Add(context.CurrentPath);
                paths.AddRange(context.Paths);

                TemplateLexer lexer = new TemplateLexer(Resources.LoadResource(paths.ToArray(), path, context.Charset));
                TemplateParser parser = new TemplateParser(lexer.Parse(), context.Analyzer);
                while (parser.MoveNext())
                {
                    parser.Current.Parse(context, writer);
                }
            }
        }
    }
}