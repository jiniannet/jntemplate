/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Template 接口
    /// </summary>
    public interface ITemplate
    {
        /// <summary>
        /// 模板上下文
        /// </summary>
        TemplateContext Context { get; set; }
        /// <summary>
        ///  模板名字
        /// </summary>
        string TemplateKey { get; set; }
        /// <summary>
        /// 模板内容
        /// </summary>
        string TemplateContent { get; set; }
        /// <summary>
        /// 结果呈现
        /// </summary>
        /// <param name="writer"></param>
        void Render(TextWriter writer);


        /// <summary>
        /// 设置实例对象
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        void Set<T>(string key, T value);

        /// <summary>
        /// 设置静态对象
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="type">静态类型</param>
        void SetStaticType(string key, Type type);
    }
}