﻿/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// CommentTag
    /// </summary>
    [Serializable]
    public class CommentTag : SpecialTag
    {

        /// <inheritdoc />
        public override bool Out => false;

        /// <inheritdoc />
        public override string ToString()
        {
            return this.FirstToken.ToString();
        }

    }
}