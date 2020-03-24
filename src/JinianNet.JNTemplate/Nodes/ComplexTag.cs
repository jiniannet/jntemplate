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
            if (Children.Count == 1)
            {
                return Children[0].ParseResult(context);
            }
            using (var write = new StringWriter())
            {
                Parse(context, write);
                return write.ToString();
            }
        }

#if NETCOREAPP || NETSTANDARD

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<object> ParseResultAsync(TemplateContext context)
        {
            if (Children.Count == 1)
            {
                return await Children[0].ParseResultAsync(context);
            }
            using (var write = new StringWriter())
            {
                await ParseAsync(context, write);
                return write.ToString();
            }
        }
#endif
    }
}
