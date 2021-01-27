/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 文本标签
    /// </summary>
    [Serializable]
    public class TextTag : SpecialTag
    {
        /// <summary>
        /// 标签文本
        /// </summary>
        public string Text
        {
            get
            {
                if (this.FirstToken != null)
                {
                    return this.FirstToken.ToString();
                }
                return null;
            }
        } 

        /// <summary>
        /// 获取对象的字符串引用
        /// </summary>
        public override string ToString()
        {
            return this.Text;
        }

    }
}