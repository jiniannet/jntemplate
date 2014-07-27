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
    /// <summary>
    /// ContextBase 对象
    /// </summary>
    public class ContextBase : ICloneable
    {

        private VariableScope variableScope;
        /// <summary>
        /// 模板数据
        /// </summary>
        public VariableScope TempData
        {
            get { return variableScope; }
            set { variableScope = value; }
        }

        /// <summary>
        /// ContextBase 
        /// </summary>
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
