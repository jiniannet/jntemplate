/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 *****************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Globalization;

namespace JinianNet.JNTemplate.Parser
{
    public class VariableScope
    {
        private VariableScope _parent;

        private Dictionary<String, Object> _dictionary;

        public VariableScope()
            : this(null)
        {

        }

        public VariableScope(VariableScope parent)
        {
            this._parent = parent;
            this._dictionary = new Dictionary<String, Object>(StringComparer.InvariantCultureIgnoreCase);
        }



        /// <summary>
        /// 
        /// </summary>
        public void Clear(Boolean all)
        {
            this._dictionary.Clear();
            if (all)
            {
                if (this._parent != null)
                {
                    this._parent.Clear(all);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public VariableScope Parent
        {
            get { return this._parent; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Object this[String name]
        {
            get
            {
                Object val;
                if (this._dictionary.TryGetValue(name, out val))
                    return val;
                if (this._parent != null)
                    return this._parent[name];
                return null;
            }
            set
            {
                this._dictionary[name] = value;
            }
        }

        public VariableScope Copy()
        {
            VariableScope owen = new VariableScope(this.Parent);
            foreach (KeyValuePair<String, Object> value in this._dictionary)
            {
                owen[value.Key] = value.Value;
            }
            return owen;
        }


        public void Push(String key, Object value)
        {
            this._dictionary.Add(key, value);
        }

        public Boolean ContainsKey(String key)
        {
            if (this._dictionary.ContainsKey(key))
            {
                return true;
            }
            if (_parent != null)
            {
                return this._parent.ContainsKey(key);
            }
            
            return false;
        }

        public bool Remove(String key)
        {
            return this._dictionary.Remove(key);
        }

    }
}