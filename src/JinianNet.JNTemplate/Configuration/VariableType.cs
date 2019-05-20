/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

namespace JinianNet.JNTemplate.Configuration
{
    /// <summary>
    /// 变量类型
    /// </summary>
    public enum VariableType
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 环境变量（会保存在引擎环境变量中，随时读取）
        /// </summary>
        Environment,
        /// <summary>
        /// 系统变量（系统特殊属性，需要用户在代码中自行进行处理）
        /// </summary>
        System
    }
}
