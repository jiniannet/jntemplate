/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

namespace JinianNet.JNTemplate.Dynamic
{
    /// <summary>
    /// 默认执行Provider
    /// </summary>
    public class DefaultActuatorProvider : IActuatorProvider
    {
        IActuator actuator;
        /// <summary>
        /// 构造函数
        /// </summary>
        public DefaultActuatorProvider(IActuator actuator)
        {
            this.actuator = actuator;
        }

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public DefaultActuatorProvider()
            : this(
#if NET20 || NET40
                  new ReflectionActuator()
#else
                  new ILActuator()
#endif
                  )
        {

        }


        /// <summary>
        /// 创建执行器
        /// </summary>
        /// <returns></returns>
        public IActuator CreateActuator()
        {
            return actuator;
        }
    }
}
