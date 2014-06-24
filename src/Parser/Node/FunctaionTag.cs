/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;
using JinianNet.JNTemplate.Context;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class FunctaionTag : SimpleTag
    {
        private String name;
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        private Object Excute(Object value, TemplateContext context)
        {
            if (value != null)
            {
                if (value is FuncHandler)
                {
                    return (value as FuncHandler).Invoke(args);
                }

                MethodInfo method = null;// ParserAccessor.GetMethod(value.GetType(), list[list.Length - 1], types);
                ParameterInfo[] pi = null;

                Object[] args = new Object[this.Children.Count];
                Type[] argsType = new Type[this.Children.Count];
                for (Int32 i = 0; i < this.Children.Count; i++)
                {
                    args[i] = this.Children[i].Parse(context);
                    argsType[i] = args[i].GetType();
                }

                MethodInfo method = value.GetType().GetMethod(this.Name, argsType);
                if (method != null)
                {
                    return method.Invoke(value, args);
                }

                //不够精准，但是命中率高
                //foreach (MethodInfo mi in value.GetType().GetMembers())
                //{
                //    pi = mi.GetParameters();
                //    if (mi.Name.Equals(list[list.Length - 1], StringComparison.OrdinalIgnoreCase) && pi.Length == args.Length)
                //    {
                //        method = mi;
                //        break;
                //    }
                //}

                //if (method != null)
                //{
                //    for (Int32 i = 0; i < args.Length; i++)
                //    {
                //        if (args[i] != null && pi[i].ParameterType.FullName != "System.Object")
                //        {
                //            args[i] = Convert.ChangeType(args[i], pi[i].ParameterType);
                //        }
                //    }
                //    return method.Invoke(value, args);
                //}
            }
            return null;
        }

        public override Object Parse(TemplateContext context)
        {
            return Excute(context.TempData[this.Name], context);
        }

        public override Object Parse(Object baseValue, TemplateContext context)
        {
            if (baseValue != null)
            {
                Object value = ParserAccessor.GetPropertyValue(baseValue, this.Name);
                return Excute(value, context);
            }

            return null;
        }

    }
}