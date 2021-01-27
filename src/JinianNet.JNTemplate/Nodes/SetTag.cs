/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 赋值标签
    /// </summary>
    [Serializable]
    public class SetTag : ComplexTag
    {
        private string name;
        private ITag value;

        /// <summary>
        /// 变量名
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// 值
        /// </summary>
        public ITag Value
        {
            get { return this.value; }
            set { this.value = value; }
        } 
    }
}