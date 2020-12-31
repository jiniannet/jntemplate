/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// IF标签
    /// </summary>
    [Serializable]
    public class IfTag : ComplexTag
    {
        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public override object ParseResult(TemplateContext context)
        {
            for (int i = 0; i < Children.Count - 1; i++) //最后面一个子对象为EndTag
            {
                var tag = (ElseifTag)Children[i];
                if (tag == null)
                {
                    continue;
                }
                if (tag.ToBoolean(context))
                {
                    return Children[i].ParseResult(context);
                }
            }

            return null;
        }

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public override Task<object> ParseResultAsync(TemplateContext context)
        {
            for (int i = 0; i < Children.Count - 1; i++) //最后面一个子对象为EndTag
            {
                var tag = (ElseifTag)Children[i];
                if (tag == null)
                {
                    continue;
                }
                if (tag.ToBoolean(context))
                {
                    return Children[i].ParseResultAsync(context);
                }
            }

            return Task<object>.FromResult((object)null);
        }
#endif
    }
}