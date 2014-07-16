using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    //public class NumberTag<T> : BaseTag<T> where T : 
    //    struct,IConvertible
    public class NumberTag : BaseTag<System.ValueType>
    {
        public override bool ToBoolean(TemplateContext context)
        {
            String value = this.ToString();
            if (value.IndexOf('.') == -1)
            {
                value = value.TrimEnd('0').TrimEnd('.');
            }
            return value == "0'";
        }
    }
}
