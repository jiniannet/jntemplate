/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class FunctaionTag : SimpleTag
    {
        public FunctaionTag(String name, Int32 line, Int32 col)
            : base(ElementType.Function, line, col)
        {
            this.Args = new List<Tag>();
            this.Name = name;
        }

        private String _name;
        public String Name
        {
            get { return _name; }
            private set { _name = value; }
        }


        private List<Tag> _args;
        public List<Tag> Args
        {
            get { return _args; }
            set { _args = value; }
        }



        public override Object Parse(TemplateContext context)
        {
            Object[] args = new Object[this.Args.Count];
            for (Int32 i = 0; i < this.Args.Count; i++)
            {
                args[i] = this.Args[i].Parse(context);
            }

            String[] list = this.Name.Split('.');

            Object value = null;

            if (list.Length == 0)
            {
                //return null;
            }
            else if (list.Length == 1)
            {
                value = context.TempData[list[0]];
            }
            else
            {
                value = context.TempData;

                for (Int32 i = 0; i < list.Length - 1; i++)
                {
                    if (list[i].IndexOfAny(new char[] { '[' }) < 0)
                        value = ParserAccessor.GetPropertyValue(value, list[i]);
                    else
                        value = ParserAccessor.GetIndexedPropertyValue(value, list[i]);

                    if (value == null)
                    {
                        break;
                    }
                }

                MethodInfo method = null;// ParserAccessor.GetMethod(value.GetType(), list[list.Length - 1], types);
                ParameterInfo[] pi = null;

                foreach (MethodInfo mi in value.GetType().GetMembers())
                {
                    pi = mi.GetParameters();
                    if (mi.Name.Equals(list[list.Length - 1], StringComparison.OrdinalIgnoreCase) && pi.Length == args.Length)
                    {
                        method = mi;
                        break;
                    }
                }

                if (method != null)
                {
                    for (Int32 i = 0; i < args.Length; i++)
                    {
                        if (args[i] != null && pi[i].ParameterType.FullName != "System.Object")
                        {
                            args[i] = Convert.ChangeType(args[i], pi[i].ParameterType);
                        }
                    }
                    return method.Invoke(value, args);
                }
                else
                {
                    value = ParserAccessor.GetPropertyValue(value, list[list.Length - 1]);
                }
                //Object obj = 

            }

            if (value != null)
            {
                if (value is FuncHandler)
                {
                    return (value as FuncHandler).Invoke(args);
                }
            }

            return null;
        }
    }
}