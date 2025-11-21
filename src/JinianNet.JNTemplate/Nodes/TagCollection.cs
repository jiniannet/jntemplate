/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 
    /// </summary>
    public class TagCollection : IList<ITag>
    {
        private readonly List<ITag> list;
        /// <summary>
        /// 
        /// </summary>
        public TagCollection()
        {
            list = new List<ITag>();
        }
        /// <inheritdoc />
        public int Count => list.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public void Add(ITag item)
        {
            if (item != null)
            {
                if (Count > 0)
                {
                    item.Previous = list[list.Count - 1].Out;
                    list[list.Count - 1].Next = item.Out;
                }
                this.list.Add(item);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            list.Clear();
        }

        /// <inheritdoc />
        public bool Contains(ITag item)
        {
            return list.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(ITag[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public ITag this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
                if (index > 0)
                {
                    value.Previous = list[index - 1].Out;
                    list[index - 1].Next = value.Out;
                }
                var next = index + 1;
                if (next < Count)
                {
                    this[next].Previous = value.Out;
                    value.Next = this[next].Out;
                }
            }
        }

        /// <inheritdoc />
        public IEnumerator<ITag> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        /// <inheritdoc />
        public bool Remove(ITag item)
        {
            var index = list.IndexOf(item);
            if (index < 0)
            {
                return false;
            }
            RemoveSet(index);
            list.RemoveAt(index);
            return true;
        }

        private void RemoveSet(int index)
        {
            var next = index + 1;
            if (next < Count)
            {
                this[next].Previous = this[index].Previous; 
            }
            var prev = index - 1;
            if(prev > 0)
            {
                this[prev].Next = this[index].Next;
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        /// <inheritdoc />
        public ITag[] ToArray()
        {
            return list.ToArray();
        }

        /// <inheritdoc />
        public int IndexOf(ITag item)
        {
            return list.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, ITag item)
        {
            list.Insert(index, item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            RemoveSet(index);
            list.RemoveAt(index);
        }
    }
}
