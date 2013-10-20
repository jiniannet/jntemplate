/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Parser;

namespace JinianNet.JNTemplate
{
    public class TemplateContext : ContextBase
    {
        public TemplateContext()
        {
            this.Charset = System.Text.Encoding.Default;
            this.Analyzer = new Analyzers();
            this.Analyzer.Add(new JinianNet.JNTemplate.Parser.TemplateParser.ForEachAnalyzer());
            this.Analyzer.Add(new JinianNet.JNTemplate.Parser.TemplateParser.IfAnalyzer());
            this.Analyzer.Add(new JinianNet.JNTemplate.Parser.TemplateParser.LoadAnalyzer(this));
            this.Analyzer.Add(new JinianNet.JNTemplate.Parser.TemplateParser.IncludeAnalyzer(this));
            this.Analyzer.Add(new JinianNet.JNTemplate.Parser.TemplateParser.SetAnalyzer());
            this.Analyzer.Add(new JinianNet.JNTemplate.Parser.TemplateParser.FunctionAnalyzer());
            this.Analyzer.Add(new JinianNet.JNTemplate.Parser.TemplateParser.VariableAnalyzer());
            this.Analyzer.Add(new JinianNet.JNTemplate.Parser.TemplateParser.ExpressionAnalyzer());
            this.Paths = new List<string>();
        }

        private String _currentPath;
        public String CurrentPath
        {
            get { return _currentPath; }
            set { _currentPath = value; }
        }
        private Encoding _charset;
        public Encoding Charset
        {
            get { return _charset; }
            set { _charset = value; }
        }

        private Analyzers _analyzer;
        public Analyzers Analyzer
        {
            get { return _analyzer; }
            private set { _analyzer = value; }
        }

        private List<string> _paths;
        /// <summary>
        /// 模板搜索路径
        /// </summary>
        public List<string> Paths
        {
            get { return _paths; }
            private set { _paths = value; }
        }

        public static TemplateContext CreateContext(TemplateContext context)
        {
            TemplateContext ctx = (TemplateContext)context.Clone();
            ctx.TempData = new VariableScope(context.TempData);
            return ctx;
        }

    }
}