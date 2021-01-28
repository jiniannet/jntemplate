/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Caching;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Parsers;
using JinianNet.JNTemplate.Resources;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Context
    /// </summary>
    [Serializable]
    public class Context
    {
        private string currentPath;
        private Encoding charset;
        private bool throwErrors;
        private bool stripWhiteSpace;
        private List<string> resourceDirectories;
        /// <summary>
        /// 上下文
        /// </summary>
        public Context()
        {
            this.resourceDirectories = new List<string>();
            this.currentPath = null;
            this.throwErrors = Utility.StringToBoolean(Runtime.GetEnvironmentVariable(nameof(Configuration.IConfig.ThrowExceptions)));
            this.stripWhiteSpace = Utility.StringToBoolean(Runtime.GetEnvironmentVariable(nameof(Configuration.IConfig.StripWhiteSpace))); 
            this.charset = Runtime.Encoding;
        }

        /// <summary>
        /// 处理标签前后空格
        /// </summary>
        public bool StripWhiteSpace
        {
            get { return stripWhiteSpace; }
            set { this.stripWhiteSpace = value; }
        }

        /// <summary>
        /// 当前资源路径
        /// </summary>
        public string CurrentPath
        {
            get { return this.currentPath; }
            set { this.currentPath = value; }
        }

        /// <summary>
        /// 当前资源编码
        /// </summary>
        public Encoding Charset
        {
            get { return this.charset; }
            set { this.charset = value; }
        }

        /// <summary>
        /// 是否抛出异常(默认为true)
        /// </summary>
        public bool ThrowExceptions
        {
            get { return this.throwErrors; }
            set { this.throwErrors = value; }
        }

        /// <summary>
        /// 模板资源搜索目录
        /// </summary>
        /// <value></value>
        public List<string> ResourceDirectories
        {
            get { return this.resourceDirectories; }
        }
    }
}
