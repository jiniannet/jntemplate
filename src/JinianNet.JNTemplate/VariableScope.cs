/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 变量域
    /// </summary>
    public class VariableScope
    {
        private VariableScope parent;
        private IDictionary<string, VariableElement> dic;


        /// <summary>
        /// 无参构造函数
        /// </summary>
        public VariableScope()
            : this(null)
        {

        }

        /// <summary>
        /// 以父VariableScope与字典来初始化对象
        /// </summary>
        /// <param name="parent">父VariableScope</param> 
        public VariableScope(VariableScope parent)
        {
            this.parent = parent;
            this.dic = new Dictionary<string, VariableElement>(Runtime.Store.ComparerIgnoreCase);

        }

        /// <summary>
        /// 清空数据
        /// </summary>
        /// <param name="all">是否清空父数据</param>
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
        /// <returns>object</returns>
        public object this[string name]
        {
            get
            {
                VariableElement val = GetElement(name);
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
        /// 为已有键设置新的值(本方法供set标签做特殊处理使用)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void SetValue<T>(string key, T value)
        {
            if (this.Parent != null && this.Parent.ContainsKey(key))
            {
                this.Parent.Set(key, value);
            }
            else
            {
                Set<T>(key, value);
            }
        }

        /// <summary>
        /// 是否包含指定键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>bool</returns>
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
        /// 移除指定对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns>是否移除成功</returns>
        public bool Remove(string key)
        {
            return this.dic.Remove(key);
        }

        /// <summary>
        /// count
        /// </summary>
        public int Count
        {
            get
            {
                return this.dic.Count + (this.parent == null ? 0 : this.parent.Count);
            }
        }

        /// <summary>
        /// 获取索引值
        /// </summary>
        /// <param name="name">索引名称</param>
        /// <returns>VariableElement</returns>
        private VariableElement GetElement(string name)
        {
            VariableElement val;
            if (this.dic.TryGetValue(name, out val))
            {
                return val;
            }
            if (this.parent != null)
            {
                return this.parent.GetElement(name);
            }
            return null;
        }
        /// <summary>
        /// 获取结果类型
        /// </summary>
        /// <param name="name">索引名称</param>
        /// <returns>Type</returns>
        public Type GetType(string name)
        {
            VariableElement val = GetElement(name);
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
        /// 添加数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <typeparam name="T">值类型</typeparam>
        public void Set<T>(string key, T value)
        {
            Set(key, value, typeof(T));
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="type">值类型</param>
        public void Set(string key, object value, Type type)
        {
            SetElement(key, new VariableElement(type, value)); 
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key">键</param> 
        /// <param name="element">值</param>
        public void SetElement(string key, VariableElement element)
        {
            this.dic[key] = element;
        } 
    }
}