/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// Foreach标签
    /// </summary>
    [Serializable]
    public class ForeachTag : ComplexTag
    {

        private string name;
        private ITag source;

        /// <summary>
        /// 节点名
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// 源对象
        /// </summary>
        public ITag Source
        {
            get { return this.source; }
            set { this.source = value; }
        }

        private void Excute(object value, TemplateContext context, TextWriter writer)
        {
            IEnumerable enumerable = ForeachTag.ToIEnumerable(value);
            TemplateContext ctx;
            if (enumerable != null)
            {
                IEnumerator ienum = enumerable.GetEnumerator();
                ctx = TemplateContext.CreateContext(context);
                int i = 0;
                while (ienum.MoveNext())
                {
                    i++;
                    ctx.TempData[this.name] = ienum.Current;
                    //为了兼容以前的用户 foreachIndex 保留
                    ctx.TempData["foreachIndex"] = i;
                    for (int n = 0; n < Children.Count; n++)
                    {
                        Children[n].Parse(ctx, writer);
                    }

                }
            }
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="writer">writer</param>
        public override void Parse(TemplateContext context, TextWriter writer)
        {
            if (Source != null)
            {
                object value = Source.ParseResult(context);
                IEnumerable enumerable = ForeachTag.ToIEnumerable(value);
                TemplateContext ctx;
                if (enumerable != null)
                {
                    IEnumerator ienum = enumerable.GetEnumerator();
                    ctx = TemplateContext.CreateContext(context);
                    int i = 0;
                    while (ienum.MoveNext())
                    {
                        i++;
                        ctx.TempData[this.name] = ienum.Current;
                        //为了兼容以前的用户 foreachIndex 保留
                        ctx.TempData["foreachIndex"] = i;
                        for (int n = 0; n < Children.Count; n++)
                        {
                            Children[n].Parse(ctx, writer);
                        }
                    }
                }
            }
        }

        #region ToIEnumerable
        /// <summary>
        /// 将对象转换为IEnumerable
        /// </summary>
        /// <param name="dataSource">源对象</param>
        /// <returns>IEnumerable</returns>
        public static IEnumerable ToIEnumerable(object dataSource)
        {
#if NET20 || NET40
            IListSource source;
#endif
            IEnumerable result;
            if (dataSource == null)
            {
                return null;
            }

            if ((result = dataSource as IEnumerable) != null)
            {
                return result;
            }
#if NET20 || NET40
            if ((source = dataSource as IListSource) != null)
            {
                IList list = source.GetList();
                if (!source.ContainsListCollection)
                {
                    return list;
                }
                if ((list != null) && (list is ITypedList))
                {
                    PropertyDescriptorCollection itemProperties = ((ITypedList)list).GetItemProperties(new PropertyDescriptor[0]);
                    if ((itemProperties == null) || (itemProperties.Count == 0))
                    {
                        return null;
                    }
                    PropertyDescriptor descriptor = itemProperties[0];
                    if (descriptor != null)
                    {
                        object component = list[0];
                        object value = descriptor.GetValue(component);
                        if ((value != null) && ((result = value as IEnumerable) != null))
                        {
                            return result;
                        }
                    }
                    return null;
                }
            }
#endif
            return null;

        }
        #endregion

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 异步解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <param name="writer">TextWriter</param>
        /// <returns></returns>
        public override async Task ParseAsync(TemplateContext context, TextWriter writer)
        {
            if (Source != null)
            {
                object value = await Source.ParseResultAsync(context);
                IEnumerable enumerable = ForeachTag.ToIEnumerable(value);
                TemplateContext ctx;
                if (enumerable != null)
                {
                    IEnumerator ienum = enumerable.GetEnumerator();
                    ctx = TemplateContext.CreateContext(context);
                    int i = 0;
                    while (ienum.MoveNext())
                    {
                        i++;
                        ctx.TempData[this.name] = ienum.Current;
                        //为了兼容以前的用户 foreachIndex 保留
                        ctx.TempData["foreachIndex"] = i;
                        for (int n = 0; n < Children.Count; n++)
                        {
                            await Children[n].ParseAsync(ctx, writer);
                        }
                    }
                }
            }
        }
#endif

    }
}