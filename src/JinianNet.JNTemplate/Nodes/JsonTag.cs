/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
#if NETCOREAPP || NETSTANDARD
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 简单JSON标签
    /// 注意：该标签仅只支持简单的JSON解析
    /// </summary>
    [Serializable]
    public class JsonTag : ComplexTag
    {
        /// <summary>
        /// JsonTag
        /// </summary>
        public JsonTag()
        {
            this.Dict = new Dictionary<ITag, ITag>();
        }

        /// <summary>
        /// 集合
        /// </summary>
        public Dictionary<ITag, ITag> Dict { get; private set; }

        /// <summary>
        ///  解析JSON
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override object ParseResult(TemplateContext context)
        {
            var result = new Dictionary<object, object>();
            foreach (var kv in Dict)
            {
                var key = kv.Key == null ? null : kv.Key.ParseResult(context);
                var value = kv.Value == null ? null : kv.Value.ParseResult(context);
                result.Add(key, value);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="write"></param>
        public override void Parse(TemplateContext context, TextWriter write)
        {
            write.Write(ParseResult(context));
        }

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        ///  解析JSON
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<object> ParseResultAsync(TemplateContext context)
        {
            var result = new Dictionary<object, object>();
            foreach (var kv in Dict)
            {
                var key = kv.Key == null ? null : await kv.Key.ParseResultAsync(context);
                var value = kv.Value == null ? null : await kv.Value.ParseResultAsync(context);
                result.Add(key, value);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="write"></param>
        public override async Task ParseAsync(TemplateContext context, TextWriter write)
        {
            var data = await ParseResultAsync(context);
            await write.WriteAsync(data?.ToString());
        }
#endif
    }
}
