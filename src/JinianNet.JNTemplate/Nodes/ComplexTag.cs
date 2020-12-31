/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.ObjectModel;
using System.IO;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 复合标签(Parse)
    /// </summary>
    [Serializable]
    public abstract class ComplexTag : Tag, ITag
    {
        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public override object ParseResult(TemplateContext context)
        {
            if (this.Children.Count == 0)
            {
                return null;
            }
            if (this.Children.Count == 1)
            {
                return this.Children[0].ParseResult(context);
            }
            var sb = new System.Text.StringBuilder();
            for(int i = 0; i < this.Children.Count; i++)
            {
                sb.Append(this.Children[i].ParseResult(context));
            }
            return sb.ToString();
        }
    }
}
