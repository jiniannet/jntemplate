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
    public class ContextBase : ICloneable
    {
        private VariableScope variableScope;
        public VariableScope TempData
        {
            get { return variableScope;}
            set { variableScope = value; }
        }

        public ContextBase()
        {
            variableScope = new VariableScope();
        }

        #region ICloneable 成员

        public Object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}
