/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class TokenCollection : IList<Token>
    {
        private List<Token> list;

        public TokenCollection()
        {
            this.list = new List<Token>();
        }

        public TokenCollection(Int32 capacity)
        {
            list = new List<Token>(capacity);
        }

        public TokenCollection(IEnumerable<Token> collection)
        {
            list = new List<Token>(collection);
        }

        public TokenCollection(IList<Token> collection, Int32 start, Int32 end)
        {
            list = new List<Token>(end + 1 - start);
            for (Int32 i = start; i <= end && i < collection.Count; i++)
            {
                this.Add(collection[i]);
            }
        }

        public Token First
        {
            get
            {
                return this.Count == 0 ? null : this[0];
            }
        }

        public Token Last
        {
            get
            {
                return this.Count == 0 ? null : this[this.Count - 1];
            }
        }

        public void Add(IList<Token> list, Int32 start, Int32 end)
        {
            for (Int32 i = start; i <= end && i < list.Count; i++)
            {
                this.Add(list[i]);
            }
        }

        public override string ToString()
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

        public Int32 IndexOf(Token item)
        {
            return list.IndexOf(item);
        }

        public void Insert(Int32 index, Token item)
        {
            if (item.TokenKind != TokenKind.Space)
            {
                list.Insert(index, item);
            }
        }

        public void RemoveAt(Int32 index)
        {
            list.RemoveAt(index);
        }

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

        public void Add(Token item)
        {
            if (item.TokenKind != TokenKind.Space)
            {
                list.Add(item);
            }
        }

        public void Clear()
        {
            list.Clear();
        }

        public Boolean Contains(Token item)
        {
            return list.Contains(item);
        }

        public void CopyTo(Token[] array, Int32 arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public Int32 Count
        {
            get
            {
                return list.Count;
            }
        }

        public Boolean IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public Boolean Remove(Token item)
        {
            return list.Remove(item);
        }

        #endregion

        #region IEnumerable<Token> 成员

        public IEnumerator<Token> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
