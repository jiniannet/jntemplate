using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public abstract class SimpleTag : BaseTag
    {
        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="baseValue">基本值</param>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public abstract Object Parse(Object baseValue, TemplateContext context);
    }
}
