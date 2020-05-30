/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// TOKEN集合
    /// </summary>
    [Serializable]
    public class TokenCollection : IList<Token>, IEquatable<TokenCollection>
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
        public TokenCollection(int capacity)
        {
            this.list = new List<Token>(capacity);
        }
        /// <summary>
        /// TOKEN集合
        /// </summary>
        /// <param name="collection">集合</param>
        public TokenCollection(IEnumerable<Token> collection)
        {
            this.list = new List<Token>(collection);
        }
        /// <summary>
        /// TOKEN集合
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public TokenCollection(IList<Token> collection, int start, int end)
        {
            this.list = new List<Token>(end + 1 - start);
            for (int i = start; i <= end && i < collection.Count; i++)
            {
                Add(collection[i]);
            }
        }
        /// <summary>
        /// 获取第一个FTOKEN
        /// </summary>
        public Token First
        {
            get
            {
                if (Count == 0)
                {
                    return null;
                }
                return this[0];
            }
        }
        /// <summary>
        /// 获取最后一个FTOKEN
        /// </summary>
        public Token Last
        {
            get
            {
                if (Count == 0)
                {
                    return null;
                }
                return this[Count - 1];
            }
        }
        ///// <summary>
        ///// 添加多个TOKEN
        ///// </summary>
        ///// <param name="list"></param>
        ///// <param name="start"></param>
        ///// <param name="end"></param>
        //public void Add(IList<Token> list, int start, int end)
        //{
        //    for (int i = start; i <= end && i < list.Count; i++)
        //    {
        //        Add(list[i]);
        //    }
        //}

        /// <summary>
        /// 获取所有TOKEN的字符串值
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
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
        public int IndexOf(Token item)
        {
            return this.list.IndexOf(item);
        }
        /// <summary>
        /// 将元素插入集合的指定索引处。
        /// </summary>
        /// <param name="index">从零开始的索引，应在该位置插入 item。</param>
        /// <param name="item">要插入的对象。对于引用类型，该值可以为 null。</param>
        public void Insert(int index, Token item)
        {
            if (item.TokenKind != TokenKind.Space)
            {
                this.list.Insert(index, item);
            }
        }
        /// <summary>
        /// 移除集合的指定索引处的元素。
        /// </summary>
        /// <param name="index">要移除的元素的从零开始的索引。</param>
        public void RemoveAt(int index)
        {
            this.list.RemoveAt(index);
        }
        /// <summary>
        /// 获取指定数量的TOKEN
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public TokenCollection this[int start, int end]
        {
            get
            {
                TokenCollection tc = new TokenCollection();
                start = GetValidIndex(start);
                end = GetValidIndex(end);
                for (int i = start; i < end && i < this.Count; i++)
                {
                    tc.Add(this[i]);
                }
                return tc;
            }
        }


        /// <summary>
        /// 分隔TokenCollection
        /// </summary>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="kinds">分隔类型</param>
        /// <returns></returns>
        public TokenCollection[] Split(int start, int end, params TokenKind[] kinds)
        {
            List<TokenCollection> tc = new List<TokenCollection>();
            start = GetValidIndex(start);
            end = GetValidIndex(end);
            if (end > this.Count)
            {
                end = this.Count;
            }
            int x = start, y = 0;
            var pos = new Stack<TokenKind>();
            for (int i = start; i < end; i++)
            {
                y = i;
                if (this[i].TokenKind == TokenKind.LeftParentheses ||
                    this[i].TokenKind == TokenKind.LeftBracket
                    || this[i].TokenKind == TokenKind.LeftBrace)
                {
                    pos.Push(this[i].TokenKind);
                }
                else if (this[i].TokenKind == TokenKind.RightParentheses ||
                    this[i].TokenKind == TokenKind.RightBrace
                    || this[i].TokenKind == TokenKind.RightBracket
                    )
                {
                    if (pos.Count > 0 &&
                        (
                        (pos.Peek() == TokenKind.LeftParentheses && this[i].TokenKind == TokenKind.RightParentheses)
                        ||
                        (pos.Peek() == TokenKind.LeftBrace && this[i].TokenKind == TokenKind.RightBrace)
                        ||
                        (pos.Peek() == TokenKind.LeftBracket && this[i].TokenKind == TokenKind.RightBracket)
                        ))
                    {
                        pos.Pop();
                    }
                    else
                    {
                        throw new Exception.ParseException(string.Concat("syntax error near ", this[i].TokenKind.ToString(), this), this[i].BeginLine, this[i].BeginColumn);
                    }
                }
                else if (pos.Count == 0 && IsInKinds(this[i].TokenKind, kinds))
                {
                    if (y > x)
                    {
                        tc.Add(this[x, y]);
                    }
                    x = i + 1;
                    TokenCollection coll = new TokenCollection();
                    coll.Add(this[i]);
                    tc.Add(coll);
                }

                if (i == end - 1 && y >= x)
                {
                    //if (x == 0 && y == i)
                    //{
                    //    throw new Exception.ParseException(string.Concat("Unexpected  tag:", this), this[0].BeginLine, this[0].BeginColumn);
                    //}
                    tc.Add(this[x, y + 1]);
                    x = i + 1;
                }
            }

            return tc.ToArray();
        }

        private bool IsInKinds(TokenKind kind, params TokenKind[] kinds)
        {
            if (kinds == null || kinds.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < kinds.Length; i++)
            {
                if (kind == kinds[i])
                {
                    return true;
                }
            }
            return false;
        }

        private int GetValidIndex(int index)
        {
            if (index < 0)
            {
                return this.Count + index;
            }
            return index;
        }

        /// <summary>
        /// 获取或设置指定索引的值
        /// </summary>
        /// <param name="index">从零开始的索引。</param>
        /// <returns></returns>
        public Token this[int index]
        {
            get
            {
                return this.list[GetValidIndex(index)];
            }
            set
            {
                if (value.TokenKind != TokenKind.Space)
                {
                    this.list[GetValidIndex(index)] = value;
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
                this.list.Add(item);
            }
        }

        /// <summary>
        ///  从集合中移除所有元素。
        /// </summary>
        public void Clear()
        {
            this.list.Clear();
        }

        /// <summary>
        /// 确定某元素是否在集合中。
        /// </summary>
        /// <param name="item">要在集合中查找的对象</param>
        /// <returns></returns>
        public bool Contains(Token item)
        {
            return this.list.Contains(item);
        }

        /// <summary>
        /// 将整个集合复制到兼容的一维数组中，从目标数组的指定索引位置开始放置。
        /// </summary>
        /// <param name="array"> 作为从集合复制的元素的目标位置的一维Token数组</param>
        /// <param name="arrayIndex">必须具有从零开始的索引。</param>
        public void CopyTo(Token[] array, int arrayIndex)
        {
            this.list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 集合的对象
        /// </summary>
        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }
        /// <summary>
        /// 是否只读集合
        /// </summary>
        public bool IsReadOnly
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
        public bool Remove(Token item)
        {
            return this.list.Remove(item);
        }

        #endregion

        #region IEnumerable<Token> 成员
        /// <summary>
        /// 返回循环访问集合的枚举器。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Token> GetEnumerator()
        {
            return this.list.GetEnumerator();
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
        /// <summary>
        /// 比列二个集合是否相同
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(TokenCollection other)
        {
            if (other == null)
            {
                return false;
            }
            if (this.Count != other.Count)
            {
                return false;
            }
            for (int i = 0; i < other.Count; i++)
            {
                if (this[i] != other[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 重载Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is TokenCollection)
            {
                return this.Equals((TokenCollection)obj);
            }
            return false;
        }

        /// <summary>
        /// 计算HASH CODE
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}