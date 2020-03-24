/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 组合标签
    /// 用于执于复杂的方法或变量
    /// 类似于
    /// $User.CreateDate.ToString("yyyy-MM-dd")
    /// $Db.Query().Result.Count
    /// </summary>
    [Serializable]
    public class ReferenceTag : BasisTag
    {
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override object ParseResult(TemplateContext context)
        {
            if (Children.Count > 0)
            {
                object result = Children[0].ParseResult(context);
                for (int i = 1; i < Children.Count && result != null; i++)
                {
                    result = ((BasisTag)Children[i]).ParseResult(result, context);
                }
                return result;
            }
            return null;
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="baseValue">基本值</param>
        public override object ParseResult(object baseValue, TemplateContext context)
        {
            object result = baseValue;
            for (int i = 0; i < Children.Count && result != null; i++)
            {
                result = ((BasisTag)Children[i]).ParseResult(result, context);
            }
            return result;
        }

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override async Task<object> ParseResultAsync(TemplateContext context)
        {
            if (Children.Count > 0)
            {
                object result = await Children[0].ParseResultAsync(context);
                for (int i = 1; i < Children.Count && result != null; i++)
                {
                    result = await ((BasisTag)Children[i]).ParseResultAsync(result, context);
                }
                return result;
            }
            return null;
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="baseValue">基本值</param>
        public override async Task<object> ParseResultAsync(object baseValue, TemplateContext context)
        {
            object result = baseValue;
            for (int i = 0; i < Children.Count && result != null; i++)
            {
                result = await ((BasisTag)Children[i]).ParseResultAsync(result, context);
            }
            return result;
        }
#endif
    }
}