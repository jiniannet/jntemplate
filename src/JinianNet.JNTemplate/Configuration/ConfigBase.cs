/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JinianNet.JNTemplate.Configuration
{
    /// <summary>
    /// 基本配置信息
    /// </summary>
    public class ConfigBase
    {
        private char _tagFlag;
        private string _tagPrefix;
        private string _tagSuffix;
        private bool _throwExceptions;
        private bool _stripWhiteSpace;
        private bool _ignoreCase;
        private string _charset;
        private Caching.ICache _cachingProvider;
        private ILoader _loadProvider;
        private ICallProxy _callProvider;
        private Parsers.ITagParser[] _tagParsers;
        private string[] _resourceDirectories;


        /// <summary>
        /// 资源路径
        /// </summary>
        public string[] ResourceDirectories
        {
            get { return this._resourceDirectories; }
            set { this._resourceDirectories = value; }
        }
        /// <summary>
        /// 字符编码
        /// </summary>
        [Variable]
        public string Charset
        {
            get { return this._charset; }
            set { this._charset = value; }
        }
        /// <summary>
        /// 标签前缀
        /// </summary>
        [Variable]
        public string TagPrefix
        {
            get { return this._tagPrefix; }
            set { this._tagPrefix = value; }
        }

        /// <summary>
        /// 标签后缀
        /// </summary>
        [Variable]
        public string TagSuffix
        {
            get { return this._tagSuffix; }
            set { this._tagSuffix = value; }
        }


        /// <summary>
        /// 简写标签前缀
        /// </summary>
        [Variable]
        public char TagFlag
        {
            get { return this._tagFlag; }
            set { this._tagFlag = value; }
        }

        /// <summary>
        /// 是否抛出异常
        /// </summary>
        [Variable]
        public bool ThrowExceptions
        {
            get { return this._throwExceptions; }
            set { this._throwExceptions = value; }
        }


        /// <summary>
        /// 是否处理标签前后空白字符
        /// </summary>
        [Variable]
        public bool StripWhiteSpace
        {
            get { return this._stripWhiteSpace; }
            set { this._stripWhiteSpace = value; }
        }


        /// <summary>
        /// 是否忽略大小写
        /// </summary>
        [Variable]
        public bool IgnoreCase
        {
            get { return this._ignoreCase; }
            set { this._ignoreCase = value; }
        }

        /// <summary>
        /// 缓存提供器
        /// </summary>
        [Variable("Cache")]
        public Caching.ICache CachingProvider
        {
            get { return this._cachingProvider; }
            set { this._cachingProvider = value; }
        }

        /// <summary>
        /// 执行提供器
        /// </summary>
        public ICallProxy CallProvider
        {
            get { return this._callProvider; }
            set { this._callProvider = value; }
        }

        /// <summary>
        /// 加载提供器
        /// </summary>
        public ILoader LoadProvider
        {
            get { return this._loadProvider; }
            set { this._loadProvider = value; }
        }

        /// <summary>
        /// 标签分析器
        /// </summary>
        [Variable("Parsers")]
        public Parsers.ITagParser[] TagParsers
        {
            get { return this._tagParsers; }
            set { this._tagParsers = value; }
        }



        /// <summary>
        /// 将配置转换成字典形式
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, string> ToDictionary()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["Charset"] = this.Charset;
            dic["TagPrefix"] = this.TagPrefix;
            dic["TagSuffix"] = this.TagSuffix;
            dic["TagFlag"] = this.TagFlag.ToString();
            dic["ThrowExceptions"] = this.ThrowExceptions.ToString();
            dic["StripWhiteSpace"] = this.StripWhiteSpace.ToString();
            dic["IgnoreCase"] = this.IgnoreCase.ToString();
            //#if NETSTANDARD
            //            IEnumerable<PropertyInfo> pis = this.GetType().GetRuntimeProperties();
            //#else
            //            IEnumerable<PropertyInfo> pis = this.GetType().GetProperties();
            //#endif
            //#if NOTDNX
            //            Type type = typeof(VariableAttribute);
            //#endif
            //            foreach (PropertyInfo pi in pis)
            //            {
            //#if NOTDNX
            //                if (Attribute.IsDefined(pi, type))
            //                {
            //                    dic[pi.Name] = (pi.GetValue(this, null) ?? string.Empty).ToString();
            //                }
            //#else
            //                dic[pi.Name] = (pi.GetValue(this, null) ?? string.Empty).ToString();
            //#endif
            //            }
            return dic;
        }

        //        /// <summary>
        //        /// 将字典转换成配置形式
        //        /// </summary>
        //        /// <returns></returns>
        //        public static ConfigBase ForDictionary<T>(Dictionary<string, string> dic)
        //        {
        //            Type type =typeof(T);
        //            var conf = Activator.CreateInstance(type);
        //#if NETSTANDARD
        //            IEnumerable<PropertyInfo> pis = type.GetRuntimeProperties();
        //#else
        //            IEnumerable<PropertyInfo> pis = type.GetProperties();
        //#endif
        //            foreach (PropertyInfo pi in pis)
        //            {
        //                string value;
        //                if(dic.TryGetValue(pi.Name,out value))
        //                {
        //                    if (pi.PropertyType == typeof(string))
        //                    {
        //                        pi.SetValue(type, value, null);
        //                        continue;
        //                    }
        //                    //if (pi.PropertyType == ) 
        //                }
        //                //dic[pi.Name] = (pi.GetValue(this, null) ?? string.Empty).ToString();
        //            }

        //            //    switch (t.FullName)
        //            //    {
        //            //        case "System.String":
        //            //        case "System.Byte":
        //            //        case "System.Boolean":
        //            //        case "System.DateTime":
        //            //        case "System.Decimal":
        //            //        case "System.Double":
        //            //        case "System.Guid":
        //            //        case "System.Int16":
        //            //        case "System.Int32":
        //            //        case "System.Int64":
        //            //        case "System.SByte":
        //            //        case "System.Single":
        //            //        case "System.UInt16":
        //            //        case "System.UInt32":
        //            //        case "System.UInt64":
        //            //        case "System.Object":
        //            //            return true;
        //            //        default:
        //            //            return false;
        //            //    }
        //            //}
        //            return new ConfigBase();
        //        }
    }
}