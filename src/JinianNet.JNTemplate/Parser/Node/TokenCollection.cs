/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace JinianNet.JNTemplate.Parser.Node
{
    /// <summary>
    /// TOKEN集合
    /// </summary>
    public class TokenCollection : IList<Token>
    {
        private List<Token> list;
        /// <summary>
        /// TOKEN集合
        /// </summary>
        public TokenCollection()
        {
            this.list = new List<Token>();
        }
        /// <summary>
        /// TOKEN集合
        /// </summary>
        /// <param name="capacity"></param>
        public TokenCollection(Int32 capacity)
        {
            list = new List<Token>(capacity);
        }
        /// <summary>
        /// TOKEN集合
        /// </summary>
        /// <param name="collection">集合</param>
        public TokenCollection(IEnumerable<Token> collection)
        {
            list = new List<Token>(collection);
        }
        /// <summary>
        /// TOKEN集合
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public TokenCollection(IList<Token> collection, Int32 start, Int32 end)
        {
            list = new List<Token>(end + 1 - start);
            for (Int32 i = start; i <= end && i < collection.Count; i++)
            {
                this.Add(collection[i]);
            }
        }
        /// <summary>
        /// 获取第一个FTOKEN
        /// </summary>
        public Token First
        {
            get
            {
                return this.Count == 0 ? null : this[0];
            }
        }
        /// <summary>
        /// 获取最后一个FTOKEN
        /// </summary>
        public Token Last
        {
            get
            {
                return this.Count == 0 ? null : this[this.Count - 1];
            }
        }
        /// <summary>
        /// 添加多个TOKEN
        /// </summary>
        /// <param name="list"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void Add(IList<Token> list, Int32 start, Int32 end)
        {
            for (Int32 i = start; i <= end && i < list.Count; i++)
            {
                this.Add(list[i]);
            }
        }

        /// <summary>
        /// 获取所有TOKEN的字符串值
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            //return String.Concat<Token>(this.list); //不兼容2.0
            StringBuilder sb = new StringBuilder();
            for (Int32 i = 0; i < this.Count; i++)
            {
                sb.Append(this[i].ToString());
            }
            return sb.ToString();
        }

        #region IList<Token> 成员
        /// <summary>
        /// 搜索指定的对象，并返回整个集合中第一个匹配项的从零开始的索引。
        /// </summary>
        /// <param name="item">要在集合中查找的对象。对于引用类型，该值可以为 null。</param>
        /// <returns>如果在整个集合中找到 item 的第一个匹配项，则为该项的从零开始的索引；否则为-1</returns>
        public Int32 IndexOf(Token item)
        {
            return list.IndexOf(item);
        }
        /// <summary>
        /// 将元素插入集合的指定索引处。
        /// </summary>
        /// <param name="index">从零开始的索引，应在该位置插入 item。</param>
        /// <param name="item">要插入的对象。对于引用类型，该值可以为 null。</param>
        public void Insert(Int32 index, Token item)
        {
            if (item.TokenKind != TokenKind.Space)
            {
                list.Insert(index, item);
            }
        }
        /// <summary>
        /// 移除集合的指定索引处的元素。
        /// </summary>
        /// <param name="index">要移除的元素的从零开始的索引。</param>
        public void RemoveAt(Int32 index)
        {
            list.RemoveAt(index);
        }

        /// <summary>
        /// 获取或设置指定索引的值
        /// </summary>
        /// <param name="index">从零开始的索引。</param>
        /// <returns></returns>
        public Token this[Int32 index]
        {
            get
            {
                return list[index];
            }
            set
            {
                if (value.TokenKind != TokenKind.Space)
                {
                    list[index] = value;
                }
            }
        }

        #endregion

        #region ICollection<Token> 成员
        /// <summary>
        /// 将对象添加到集合的结尾处。
        /// </summary>
        /// <param name="item">要添加到集合的末尾处的对象。</param>
        public void Add(Token item)
        {
            if (item.TokenKind != TokenKind.Space)
            {
                list.Add(item);
            }
        }

        /// <summary>
        ///  从集合中移除所有元素。
        /// </summary>
        public void Clear()
        {
            list.Clear();
        }

        /// <summary>
        /// 确定某元素是否在集合中。
        /// </summary>
        /// <param name="item">要在集合中查找的对象</param>
        /// <returns></returns>
        public Boolean Contains(Token item)
        {
            return list.Contains(item);
        }

        /// <summary>
        /// 将整个集合复制到兼容的一维数组中，从目标数组的指定索引位置开始放置。
        /// </summary>
        /// <param name="array"> 作为从集合复制的元素的目标位置的一维Token数组</param>
        /// <param name="arrayIndex">必须具有从零开始的索引。</param>
        public void CopyTo(Token[] array, Int32 arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 集合的对象
        /// </summary>
        public Int32 Count
        {
            get
            {
                return list.Count;
            }
        }
        /// <summary>
        /// 是否只读集合
        /// </summary>
        public Boolean IsReadOnly
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        ///  从集合中移除特定对象的第一个匹配项。
        /// </summary>
        /// <param name="item">要从集合中移除的对象</param>
        /// <returns></returns>
        public Boolean Remove(Token item)
        {
            return list.Remove(item);
        }

        #endregion

        #region IEnumerable<Token> 成员
        /// <summary>
        /// 返回循环访问集合的枚举器。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Token> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员
        /// <summary>
        /// 返回循环访问集合的枚举器。
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}