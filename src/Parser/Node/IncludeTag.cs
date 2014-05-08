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
    public class IncludeTag : ComplexTag
    {
        public IncludeTag(Int32 line, Int32 col)
            : base(ElementType.Include, line, col)
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

            System.Collections.Generic.List<string> paths = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrWhiteSpace(context.CurrentPath))
                paths.Add(context.CurrentPath);
            paths.AddRange(context.Paths);

            if (!String.IsNullOrEmpty(path))
            {
                writer.Write(Resources.LoadResource(paths.ToArray(), path, context.Charset));
            }
        }
    }
}