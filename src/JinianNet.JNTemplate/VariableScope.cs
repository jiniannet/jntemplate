/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Variable Scope
    /// </summary>
    public class VariableScope : IVariableScope
    {
        private IDictionary<string, VariableElement> dic;


        /// <inheritdoc />
        public TypeDetect DetectPattern { get; set; }
        /// <inheritdoc />
        public IVariableScope Parent { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableScope"/> class
        /// </summary>
        public VariableScope()
        {
            this.Parent = null;
            this.dic = new Dictionary<string, VariableElement>(StringComparer.OrdinalIgnoreCase);
            this.DetectPattern = TypeDetect.Absolute;
        }

        /// <inheritdoc />
        public void Clear(bool all)
        {
            this.dic.Clear();
            if (all
                && this.Parent != null)
            {
                this.Parent.Clear(all);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            Clear(false);
        }

        /// <inheritdoc />
        public object this[string key]
        {
            get
            {
                var val = GetElement(key);
                if (val != null)
                {
                    return val.Value;
                }
                if (Parent != null)
                {
                    return Parent[key];
                }
                return null;
            }
        }


        private Type GetValueType(object value)
        {
            if (value != null)
            {
                return value.GetType();
            }
            else
            {
                return typeof(object);
            }
        }

        /// <inheritdoc />
        public bool Update<T>(string key, T value)
        {
            if (this.dic.ContainsKey(key))
            {
                Set<T>(key, value);
                return true;
            }
            if (this.Parent != null)
            {
                return this.Parent.Update<T>(key, value);
            }
            return false;
        }

        /// <inheritdoc />
        public bool ContainsKey(string key)
        {
            if (this.dic.ContainsKey(key))
            {
                return true;
            }
            if (this.Parent != null)
            {
                return this.Parent.ContainsKey(key);
            }

            return false;
        }

        /// <inheritdoc />
        public bool Remove(string key)
        {
            return this.dic.Remove(key);
        }

        /// <inheritdoc />
        public int Count => this.dic.Count + (this.Parent == null ? 0 : this.Parent.Count);

        /// <inheritdoc />
        private VariableElement GetElement(string key)
        {
            VariableElement val;
            if (this.dic.TryGetValue(key, out val))
            {
                return val;
            }
            return null;
        }
        /// <inheritdoc />
        public Type GetType(string key)
        {
            VariableElement val = GetElement(key);
            if (val != null)
            {
                if (val.Type != null)
                {
                    return val.Type;
                }
                if (val.Value != null)
                {
                    return val.Value.GetType();
                }
            }
            if (Parent != null)
            {
                return Parent.GetType(key);
            }
            return null;
        }
        /// <inheritdoc />
        public void Set<T>(string key, T value)
        {
            Set(key, value, typeof(T));
        }
        /// <inheritdoc />
        public void Set(string key, object value, Type type)
        {
            if (this.DetectPattern == TypeDetect.None
            || (this.DetectPattern == TypeDetect.Standard && value != null)
            || (this.DetectPattern == TypeDetect.Auto && value != null && value.GetType() == type))
            {
                SetElement(key, new VariableElement(null, value));
            }
            else
            {
                SetElement(key, new VariableElement(type, value));
            }
        }

        /// <inheritdoc />
        public void SetElement(string key, VariableElement element)
        {
            this.dic[key] = element;
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (KeyValuePair<string, VariableElement> kv in dic)
            {
                yield return new KeyValuePair<string, object>(kv.Key, kv.Value);
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public ICollection<string> Keys => this.dic.Keys;
    }
}