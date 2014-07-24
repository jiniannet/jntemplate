/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    /// <summary>
    /// 基本类型标签
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public abstract class BaseTag<T> : Tag
    {
        private T baseValue;
        /// <summary>
        /// 值
        /// </summary>
        public T Value
        {
            get { return this.baseValue; }
            set { this.baseValue = value; }
        }
        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public override object Parse(TemplateContext context)
        {
            return this.Value;
        }
        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="baseValue">基本值</param>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public override object Parse(object baseValue, TemplateContext context)
        {
            return this.Value;
        }
        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <param name="write">TextWriter</param>
        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {
            write.Write(this.Value.ToString());
        }

        ///// <summary>
        ///// 输出STRING
        ///// </summary>
        ///// <returns></returns>
        //public override string ToString()
        //{
        //    if (this.Value != null)
        //    {
        //        return this.Value.ToString();
        //    }
        //    return string.Empty;
        //}
    }
}
