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
using System.Globalization;

namespace JinianNet.JNTemplate.Parser
{
    public class VariableScope
    {
        private VariableScope parent;

        private Dictionary<String, Object> dic;

        public VariableScope()
            : this(null)
        {

        }

        public VariableScope(VariableScope parent)
        {
            this.parent = parent;
            this.dic = new Dictionary<String, Object>(StringComparer.InvariantCultureIgnoreCase);
        }



        /// <summary>
        /// 
        /// </summary>
        public void Clear(Boolean all)
        {
            this.dic.Clear();
            if (all)
            {
                if (this.parent != null)
                {
                    this.parent.Clear(all);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public VariableScope Parent
        {
            get { return this.parent; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Object this[String name]
        {
            get
            {
                Object val;
                if (this.dic.TryGetValue(name, out val))
                    return val;
                if (this.parent != null)
                    return this.parent[name];
                return null;
            }
            set
            {
                this.dic[name] = value;
            }
        }

        public VariableScope Copy()
        {
            VariableScope owen = new VariableScope(this.Parent);
            foreach (KeyValuePair<String, Object> value in this.dic)
            {
                owen[value.Key] = value.Value;
            }
            return owen;
        }


        public void Push(String key, Object value)
        {
            this.dic.Add(key, value);
        }

        public Boolean ContainsKey(String key)
        {
            if (this.dic.ContainsKey(key))
            {
                return true;
            }
            if (parent != null)
            {
                return this.parent.ContainsKey(key);
            }
            
            return false;
        }

        public bool Remove(String key)
        {
            return this.dic.Remove(key);
        }

    }
}