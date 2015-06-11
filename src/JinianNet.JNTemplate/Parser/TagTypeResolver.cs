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

        public TagTypeResolver()
            : this(new ITagParser[0])
        {
            this.collection = new List<ITagParser>();
        }

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
        /// 如果在 System.Collections.Generic.List<T> 中找到 item，则为 true，否则为 false。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Boolean Contains(ITagParser item)
        {
            return this.collection.Contains(item);
        }

        public void CopyTo(ITagParser[] array, int arrayIndex)
        {
            this.collection.CopyTo(array, arrayIndex);
        }

        public Int32 Count
        {
            get
            {
                return this.collection.Count;
            }
        }

        public Boolean IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ITagParser this[Int32 index]
        {
            set { this.collection[index] = value; }
            get { return this.collection[index]; }
        }

        public Boolean Remove(ITagParser item)
        {
            return this.collection.Remove(item);
        }

        public IEnumerator<ITagParser> GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            for (Int32 i = 0; i < Count; i++)
                yield return this[i];
        }
    }
}
