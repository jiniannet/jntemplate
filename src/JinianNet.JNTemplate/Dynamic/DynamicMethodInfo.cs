
/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/

using System;
using System.Reflection;
namespace JinianNet.JNTemplate.Dynamic
{
    /// <summary>
    /// 动态方法信息
    /// </summary>
    public class DynamicMethodInfo
    {
        private ExcuteMethodDelegate _delegate;
        private ParameterInfo[] _parameters;
        private String _name;
        private String _fullName;

        /// <summary>
        /// 执行方法委托
        /// </summary>
        public ExcuteMethodDelegate Delegate
        {
            get { return _delegate; }
            set { _delegate = value; }
        }
        /// <summary>
        /// 形参
        /// </summary>
        public ParameterInfo[] Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }
        /// <summary>
        /// 方法名称
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// 方法完整名称
        /// </summary>
        public String FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }
    }
}
