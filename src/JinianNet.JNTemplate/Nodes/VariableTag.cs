/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 变量标签(ParseResult)
    /// </summary>
    [Serializable]
    public class VariableTag : ChildrenTag
    {
        private string name;
        /// <summary>
        /// 变量名
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        } 
    }
}
