/********************************************************************************
 Copyright(c) jiniannet(http://www.jiniannet.com). All rights reserved.

Licensed under the MIT license.See licence.txt file in the project root for full license information.

********************************************************************************/

namespace JinianNet.JNTemplate.Dynamic
{
    /// <summary>
    /// 执行Provider
    /// </summary>
    public interface IActuatorProvider
    {
        /// <summary>
        /// 创建执行器
        /// </summary>
        /// <returns></returns>
        IActuator CreateActuator();
    }
}
