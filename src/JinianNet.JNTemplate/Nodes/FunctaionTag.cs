/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 函数（方法）标签
    /// </summary>
    [Serializable]
    public class FunctaionTag : ChildrenTag
    {
        private string name;
        /// <summary>
        /// 函数
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        } 
    }
}