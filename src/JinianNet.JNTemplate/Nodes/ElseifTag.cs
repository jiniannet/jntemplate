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
    /// ELSE if 标签
    /// </summary>
    [Serializable]
    public class ElseifTag : ComplexTag
    {

        private ITag condition;
        /// <summary>
        /// 条件
        /// </summary>
        public virtual ITag Condition
        {
            get { return this.condition; }
            set { this.condition = value; }
        }
    }
}