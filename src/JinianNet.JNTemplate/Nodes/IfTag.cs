/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// IF标签
    /// </summary>
    public class IfTag : TagBase
    {
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override object Parse(TemplateContext context)
        {
            for (int i = 0; i < Children.Count-1; i++) //最后面一个子对象为EndTag
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