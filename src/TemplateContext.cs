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
using JinianNet.JNTemplate.Context;

namespace JinianNet.JNTemplate
{
    public class TemplateContext : ContextBase
    {
        public TemplateContext()
        {
            this.Charset = System.Text.Encoding.Default;
            this.Parser = new List<ITagParser>();
            this.Parser.Add(new JinianNet.JNTemplate.Parser.WordParser());
            this.Parser.Add(new JinianNet.JNTemplate.Parser.VariableParser());
            this.Parser.Add(new JinianNet.JNTemplate.Parser.VariableParser());
            this.Parser.Add(new JinianNet.JNTemplate.Parser.ForeachParser());
            this.Parser.Add(new JinianNet.JNTemplate.Parser.FunctionParser());
            //this.Analyzer.Add(new JinianNet.JNTemplate.Parser.TemplateParser.ForEachAnalyzer());
            //this.Analyzer.Add(new JinianNet.JNTemplate.Parser.TemplateParser.IfAnalyzer());
            //this.Analyzer.Add(new JinianNet.JNTemplate.Parser.TemplateParser.LoadAnalyzer(this));
            //this.Analyzer.Add(new JinianNet.JNTemplate.Parser.TemplateParser.IncludeAnalyzer(this));
            //this.Analyzer.Add(new JinianNet.JNTemplate.Parser.TemplateParser.SetAnalyzer());
            //this.Analyzer.Add(new JinianNet.JNTemplate.Parser.TemplateParser.FunctionAnalyzer());
            //this.Analyzer.Add(new JinianNet.JNTemplate.Parser.TemplateParser.VariableAnalyzer());
            //this.Analyzer.Add(new JinianNet.JNTemplate.Parser.TemplateParser.ExpressionAnalyzer());
            this.Paths = new List<String>();
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

        private List<ITagParser> _parser;
        public List<ITagParser> Parser
        {
            get { return _parser; }
            private set { _parser = value; }
        }

        private List<String> _paths;
        /// <summary>
        /// 模板搜索路径
        /// </summary>
        public List<String> Paths
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