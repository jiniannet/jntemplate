using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    /// <summary>
    /// 用于执于复杂的方法或变量
    /// 类似于
    /// $User.CreateDate.ToString("yyyy-MM-dd")
    /// $Db.Query().Result.Count
    /// </summary>
    public class ReferenceTag : SimpleTag
    {
        public override Object Parse(JinianNet.JNTemplate.Context.TemplateContext context)
        {
            if (this.Children.Count > 0)
            {
                Object result = this.Children[0].Parse(context);
                for (Int32 i = 1; i < this.Children.Count; i++)
                {
                    result = this.Children[i].Parse(result, context);
                }
                return result;
            }
            return null;
        }

        public override Object Parse(Object baseValue, JinianNet.JNTemplate.Context.TemplateContext context)
        {
            Object result = baseValue;
            for (Int32 i = 0; i < this.Children.Count; i++)
            {
                result = this.Children[i].Parse(result, context);
            }
            return result;
        }
    }
}
