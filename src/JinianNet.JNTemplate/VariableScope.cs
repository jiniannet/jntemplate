/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Variable Scope
    /// </summary>
    public class VariableScope
    {
        private VariableScope parent;
        private IDictionary<string, VariableElement> dic;
        /// <summary>
        /// Gets or sets the  detect patterns.
        /// </summary>
        public TypeDetect DetectPattern { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableScope"/> class
        /// </summary>
        /// <param name="parent">The parent <see cref="VariableScope"/>.</param> 
        public VariableScope(VariableScope parent)
            : this(parent, parent?.DetectPattern ?? TypeDetect.Standard)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableScope"/> class
        /// </summary>
        /// <param name="parent">The parent <see cref="VariableScope"/>.</param> 
        /// <param name="pattern">The <see cref="TypeDetect"/>.</param> 
        public VariableScope(VariableScope parent, TypeDetect pattern)
        {
            this.parent = parent;
            this.dic = new Dictionary<string, VariableElement>(StringComparer.OrdinalIgnoreCase);
            this.DetectPattern = pattern;
        }

        /// <summary>
        /// Removes all items from the <see cref="VariableScope"/>.
        /// </summary>
        /// <param name="all">is removes all</param>
        public void Clear(bool all)
        {
            this.dic.Clear();
            if (all
                && this.parent != null)
            {
                this.parent.Clear(all);
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="VariableScope"/>.
        /// </summary>
        public void Clear()
        {
            Clear(false);
        }

        /// <summary>
        /// gets the parent from the <see cref="VariableScope"/>.
        /// </summary>
        public VariableScope Parent
        {
            get { return this.parent; }
        }

        /// <summary>
        /// Gets the element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get.</param>
        /// <returns>The element with the specified key.</returns>
        public object this[string key]
        {
            get
            {
                VariableElement val = GetElement(key);
                if (val != null)
                {
                    return val.Value;
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

        /// <summary>
        /// update the element with the specified key from the <see cref="VariableScope"/>.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="VariableScope"/>.</param>
        /// <param name="value">The value with the specified key.</param>
        /// <returns>true if the element is successfully updated; otherwise, false.</returns>
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

        /// <summary>
        /// Determines whether the <see cref="VariableScope"/>. contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="VariableScope"/>.</param>
        /// <returns>true if the <see cref="VariableScope"/> contains an element with the key; otherwise, false.</returns>
        public bool ContainsKey(string key)
        {
            if (this.dic.ContainsKey(key))
            {
                return true;
            }
            if (this.parent != null)
            {
                return this.parent.ContainsKey(key);
            }

            return false;
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="VariableScope"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>true if the element is successfully removed; otherwise, false. This method also returns false if key was not found in the original <see cref="VariableScope"/>.</returns>
        public bool Remove(string key)
        {
            return this.dic.Remove(key);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="VariableScope"/>.
        /// </summary>
        public int Count => this.dic.Count + (this.parent == null ? 0 : this.parent.Count);

        /// <summary>
        /// Get a <see cref="VariableElement"/> for variables
        /// </summary>
        /// <param name="key">The key of the element to get</param> 
        /// <returns>The <see cref="VariableElement"/> with the specified key.</returns>
        private VariableElement GetElement(string key)
        {
            VariableElement val;
            if (this.dic.TryGetValue(key, out val))
            {
                return val;
            }
            if (this.parent != null)
            {
                return this.parent.GetElement(key);
            }
            return null;
        }
        /// <summary>
        /// Get a <see cref="Type"/> for variables
        /// </summary>
        /// <param name="key">The key of the element to get</param> 
        /// <returns>The <see cref="Type"/> with the specified key.</returns>
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
            return null;
        }

        /// <summary>
        /// Set a new value for variables.
        /// </summary>
        /// <param name="key">The key of the element to get</param> 
        /// <param name="value">The element with the specified key.</param>
        /// <typeparam name="T">The type of elements in the  <see cref="VariableScope"/>.</typeparam>
        public void Set<T>(string key, T value)
        {
            if (this.DetectPattern == TypeDetect.None
                || (this.DetectPattern == TypeDetect.Standard && value != null)
                || (this.DetectPattern == TypeDetect.Auto && value != null && value.GetType() == typeof(T)))
            {
                Set(key, value, null);
            }
            else
            {
                Set(key, value, typeof(T));
            }
        }

        /// <summary>
        /// Set a new <see cref="object"/> for variables
        /// </summary>
        /// <param name="key">The key of the element to get</param> 
        /// <param name="value">The element with the specified key.</param>
        /// <param name="type"><see cref="Type"/> of the value.</param>
        public void Set(string key, object value, Type type)
        {
            SetElement(key, new VariableElement(type, value));
        }
        /// <summary>
        /// Set a new <see cref="VariableElement"/> for variables
        /// </summary>
        /// <param name="key">The key of the element to get</param> 
        /// <param name="element">The element with the specified key.</param>
        public void SetElement(string key, VariableElement element)
        {
            this.dic[key] = element;
        }
    }
}