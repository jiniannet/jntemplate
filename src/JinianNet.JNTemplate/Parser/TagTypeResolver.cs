/*****************************************************
   Copyright (c) 2013-2015 jiniannet (http://www.jiniannet.com)

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

   Redistributions of source code must retain the above copyright notice
 *****************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate.Parser
{
    /// <summary>
    /// 分析器
    /// </summary>
    public class TagTypeResolver : ITagTypeResolver,ICollection<ITagParser>
    {
        private readonly List<ITagParser> collection;
        /// <summary>
        /// 标签类型分析器
        /// </summary>
        public TagTypeResolver()
            : this(new ITagParser[0])
        {
            this.collection = new List<ITagParser>();
        }
        /// <summary>
        /// 标签类型分析器
        /// </summary>
        /// <param name="parsers">各类标签分析器集合</param>
        public TagTypeResolver(IEnumerable<ITagParser> parsers)
        {
            this.collection = new List<ITagParser>(parsers);
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="parser">TemplateParser</param>
        /// <param name="tc">Token集合</param>
        /// <returns></returns>
        public Tag Resolver(TemplateParser parser, TokenCollection tc)
        {
            Tag t;
            for (Int32 i = 0; i < collection.Count; i++)
            {
                t = collection[i].Parse(parser, tc);
                if (t != null)
                {
                    t.FirstToken = tc.First;

                    if (t.Children.Count == 0 || (t.LastToken = t.Children[t.Children.Count - 1].LastToken ?? t.Children[t.Children.Count - 1].FirstToken) == null || tc.Last.CompareTo(t.LastToken) > 0)
                    {
                        t.LastToken = tc.Last;
                    }
                    return t;
                }
            }
            return null;
        }
        /// <summary>
        /// 添加一个标签分析器
        /// </summary>
        /// <param name="item">标签分析器</param>
        public void Add(ITagParser item)
        {
            collection.Add(item);
        }
        /// <summary>
        /// 插入一个标签分析器
        /// </summary>
        /// <param name="index">插入索引</param>
        /// <param name="item">标签分析器</param>
        public void Insert(Int32 index, ITagParser item)
        {
            collection.Insert(index, item);
        }
        /// <summary>
        /// 清除所有分析器
        /// </summary>
        public void Clear()
        {
            collection.Clear();
        }

        /// <summary>
        /// 如果在集合中找到 item，则为 true，否则为 false。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Boolean Contains(ITagParser item)
        {
            return this.collection.Contains(item);
        }
        /// <summary>
        /// 将整个 ITagParser[] 复制到兼容的一维数组中，从目标数组的指定索引位置开始放置。
        /// </summary>
        /// <param name="array">待复制的集合</param>
        /// <param name="arrayIndex">开始位置</param>
        public void CopyTo(ITagParser[] array, Int32 arrayIndex)
        {
            this.collection.CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// 返回集合个数
        /// </summary>
        public Int32 Count
        {
            get
            {
                return this.collection.Count;
            }
        }
        /// <summary>
        /// 集合是否只读
        /// </summary>
        public Boolean IsReadOnly
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// 获取或设置集合的指定索引位置的值
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>ITagParser</returns>
        public ITagParser this[Int32 index]
        {
            set { this.collection[index] = value; }
            get { return this.collection[index]; }
        }
        /// <summary>
        /// 从 分析器中 中移除特定对象的第一个匹配项。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Boolean Remove(ITagParser item)
        {
            return this.collection.Remove(item);
        }
        /// <summary>
        /// 返回循环访问 ITagParser的枚举器。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ITagParser> GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }
        /// <summary>
        /// 返回循环访问 ITagParser的枚举器。
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            for (Int32 i = 0; i < Count; i++)
                yield return this[i];
        }
    }
}
