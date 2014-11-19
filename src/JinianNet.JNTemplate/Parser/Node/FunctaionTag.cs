/*****************************************************
   Copyright (c) 2013-2014 jiniannet (http://www.jiniannet.com)

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

   Redistributions of source code must retain the above copyright notice
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

                //MethodInfo method = null;// ParserAccessor.GetMethod(value.GetType(), list[list.Length - 1], types);F:\Work\工作区\我的项目\JinianNet.JNTemplate\JinianNet.JNTemplate1.2\src\JinianNet.JNTemplate\Parser\Node\FunctaionTag.cs

                //判断是否委托
                if (value is FuncHandler)
                {
                    return (value as FuncHandler).Invoke(args);
                }



                //不是委托，则在取方法
                MethodInfo method = Common.ReflectionHelpers.GetMethod( value.GetType(),this.Name, types);
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
                result = Common.ReflectionHelpers.GetPropertyValue(baseValue, this.Name);
                if (result != null && result is FuncHandler)
                {
                    return (result as FuncHandler).Invoke(args);
                }
            }

            return null;
        }

    }
}