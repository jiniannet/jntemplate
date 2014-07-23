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
        private String name;
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        private Object Excute(Object value, TemplateContext context,Object[] args,Type[] types)
        {
            if (value != null)
            {

                //MethodInfo method = null;// ParserAccessor.GetMethod(value.GetType(), list[list.Length - 1], types);

                //判断是否委托
                if (value is FuncHandler)
                {
                    return (value as FuncHandler).Invoke(args);
                }



                //不是委托，则在取方法
                MethodInfo method = value.GetType().GetMethod(this.Name, types);
                if (method != null)
                {
                    return method.Invoke(value, args);
                }
            }
            return null;
        }

        public override Object Parse(TemplateContext context)
        {
            Object[] args = new Object[this.Children.Count];
            Type[] argsType = new Type[this.Children.Count];
            for (Int32 i = 0; i < this.Children.Count; i++)
            {
                args[i] = this.Children[i].Parse(context);
                argsType[i] = args[i].GetType();
            }

            return Excute(context.TempData[this.Name], context, args, argsType);
        }

        public override Object Parse(Object baseValue, TemplateContext context)
        {
            if (baseValue != null)
            {
                Object[] args = new Object[this.Children.Count];
                Type[] argsType = new Type[this.Children.Count];
                for (Int32 i = 0; i < this.Children.Count; i++)
                {
                    args[i] = this.Children[i].Parse(context);
                    if (args[i] != null)
                    {
                        argsType[i] = args[i].GetType();
                    }
                    else
                    {

                    }
                }


                Object result = Excute(baseValue, context, args, argsType);
                if (result != null)
                {
                    return result;
                }
                result = ReflectionHelpers.GetPropertyValue(baseValue, this.Name);
                if (result != null && result is FuncHandler)
                {
                    return (result as FuncHandler).Invoke(args);
                }
            }

            return null;
        }

    }
}