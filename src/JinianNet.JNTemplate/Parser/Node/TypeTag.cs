/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/

namespace JinianNet.JNTemplate.Parser.Node
{
    /// <summary>
    /// 基本类型标签
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public abstract class TypeTag<T> : Tag
    {
        private T _baseValue;
        /// <summary>
        /// 值
        /// </summary>
        public T Value
        {
            get { return this._baseValue; }
            set { this._baseValue = value; }
        }
        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public override object Parse(TemplateContext context)
        {
            return this._baseValue;
        }

        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <param name="write">TextWriter</param>
        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {
            if (this._baseValue != null)
            {
                write.Write(this._baseValue.ToString());
            }
        }
    }
}