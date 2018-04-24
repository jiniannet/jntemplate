/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Parser;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JinianNet.JNTemplate
{
    public class EngineInfo
    {
        #region 字段
        private Dictionary<String, String> _environmentVariable;
        private VariableScope _data; 
        private List<ITagParser> _parsers;
        private ILoader _loder;
        #endregion
        /// <summary>
        /// 引擎信息
        /// </summary>
        public EngineInfo()
        {
            _environmentVariable = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
            _parsers = new List<ITagParser>();
        }

        #region 实列
        /// <summary>
        /// 环境变量
        /// </summary>
        public Dictionary<String, String> EnvironmentVariable
        {
            get { return _environmentVariable; }
        }
        /// <summary>
        /// 全局初始数据
        /// </summary>
        public VariableScope Data
        {
            get { return _data; }
            set { _data = value; }
        }
 
        /// <summary>
        /// 标签分析器
        /// </summary>
        public List<ITagParser> Parsers
        {
            get { return _parsers; }
        }

        /// <summary>
        /// 加载器
        /// </summary>
        public ILoader Loder
        {
            get { return _loder; }
            set { _loder = value; }
        }
        #endregion
    }
}
