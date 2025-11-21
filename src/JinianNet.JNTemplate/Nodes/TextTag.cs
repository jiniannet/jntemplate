/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// TextTag
    /// </summary>
    [Serializable]
    public class TextTag : SpecialTag
    {
        private string text;
        /// <summary>
        ///  Gets the text of the tag.
        /// </summary>
        public string Text
        {
            get
            {
                if (text != null)
                {
                    return text;
                }
                if (this.FirstToken != null)
                {
                    return text = OriginalText;
                }
                return text = "";
            }
            set
            {
                text = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string OriginalText => this.FirstToken.ToString();

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Text;
        }

    }
}