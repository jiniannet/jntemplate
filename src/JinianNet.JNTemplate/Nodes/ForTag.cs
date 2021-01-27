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
    /// FOR标签
    /// </summary>
    [Serializable]
    public class ForTag : ComplexTag
    {
        private ITag initial;
        private ITag condition;
        private ITag dothing;

        /// <summary>
        /// 初始标签 
        /// </summary>
        public ITag Initial
        {
            get { return this.initial; }
            set { this.initial = value; }
        }

        /// <summary>
        /// 逻辑标签
        /// </summary>
        public ITag Condition
        {
            get { return this.condition; }
            set { this.condition = value; }
        }

        /// <summary>
        /// Do 
        /// </summary>
        public ITag Do
        {
            get { return this.dothing; }
            set { this.dothing = value; }
        }

    }
}