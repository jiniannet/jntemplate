/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;


namespace JinianNet.JNTemplate.Parser.Node
{
    /// <summary>
    /// IF标签
    /// </summary>
    public class IfTag : BaseTag
    {
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override Object Parse(TemplateContext context)
        {
            for (Int32 i = 0; i < Children.Count-1; i++) //最后面一个子对象为EndTag
            {
                if (Children[i].ToBoolean(context))
                {
                    return Children[i].Parse(context);
                }
            }
            return null;
        }

    }
}