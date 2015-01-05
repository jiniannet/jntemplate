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
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace JinianNet.JNTemplate.Parser
{
    /// <summary>
    /// 变量域
    /// </summary>
    public class VariableScope 
    {
        private VariableScope parent;

        private Dictionary<String, Object> dic;

        /// <summary>
        /// VariableScope
        /// </summary>
        public VariableScope()
            : this(null)
        {

        }

        /// <summary>
        /// VariableScope
        /// </summary>
        public VariableScope(VariableScope parent)
        {
            this.parent = parent;
            this.dic = new Dictionary<String, Object>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        /// <param name="all">是否清空父数据</param>
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
        /// 清空数据
        /// </summary>
        public void Clear()
        {
            Clear(false);
        }

        /// <summary>
        /// 父对象
        /// </summary>
        public VariableScope Parent
        {
            get { return this.parent; }
        }

        /// <summary>
        /// 获取索引值
        /// </summary>
        /// <param name="name">索引名称</param>
        /// <returns></returns>
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

        ///// <summary>
        ///// 复制数据
        ///// </summary>
        ///// <returns></returns>
        //public VariableScope Copy()
        //{
        //    VariableScope owen = new VariableScope(this.Parent);
        //    foreach (KeyValuePair<String, Object> value in this.dic)
        //    {
        //        owen[value.Key] = value.Value;
        //    }
        //    return owen;
        //}

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Push(String key, Object value)
        {
            this.dic.Add(key, value);
        }

        /// <summary>
        /// 是否包含指定键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
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

        /// <summary>
        /// 移除指定对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(String key)
        {
            return this.dic.Remove(key);
        }

    }
}