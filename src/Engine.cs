/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/

using System;
using System.Text;
using System.Collections.Generic;
using JinianNet.JNTemplate.Parser;

namespace JinianNet.JNTemplate
{
    public class Engine : IEngine
    {

        #region IEngine 成员

        private VariableScope _variableScope;
        public VariableScope VariableScope
        {
            get { return _variableScope; }
            set { _variableScope =value; }
        }

        private String _currentPath;
        public String CurrentPath
        {
            get { return _currentPath; }
            set { _currentPath =value; }
        }
        private Encoding _charset;
        public Encoding Charset
        {
            get { return _charset; }
            set { _charset =value; }
        }


        public Engine()
            : this(null, Encoding.UTF8)
        {

        }

        public Engine(String path, Encoding charset)
        {
            _currentPath = path;
            _charset = charset;
            _variableScope = new VariableScope();
        }

        public ITemplate CreateTemplate()
        {
            TemplateContext context =new TemplateContext();
            context.Charset = _charset;
            context.CurrentPath = _currentPath;
            context.TempData = new VariableScope(_variableScope);

            Template template = new Template(context,null);
            template.Context = context;
            return template;
        }

        #endregion
    }
}
