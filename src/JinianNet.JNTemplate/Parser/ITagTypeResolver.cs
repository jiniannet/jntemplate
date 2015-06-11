/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Parser.Node;
using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser
{
    /// <summary>
    /// 标签类型分析器
    /// </summary>
    public interface ITagTypeResolver
    {
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="parser">TemplateParser</param>
        /// <param name="tc">Token集合</param>
        /// <returns></returns>
        Tag Resolver(TemplateParser parser, TokenCollection tc);
    }
}