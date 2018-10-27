/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using System;

namespace JinianNet.JNTemplate.Dynamic
{
    /// <summary>
    /// 获取属性（包括有参属性）或字段委托
    /// </summary>
    /// <param name="model">对象</param>
    /// <param name="propertyName">属性名称</param>
    /// <returns>返回结果</returns>
    public delegate object CallPropertyOrFieldDelegate(object model, string propertyName);
}