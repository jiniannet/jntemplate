/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.IO;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 引擎
    /// </summary>
    public interface IEngine
    {
        //void Render(TemplateContext context, TextWriter writer);

        /// <summary>
        /// 创建Template
        /// </summary>
        /// <returns></returns>
        ITemplate CreateTemplate(String path);
    }
}