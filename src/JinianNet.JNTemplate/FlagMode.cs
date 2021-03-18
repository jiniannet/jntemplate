/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Tag flag.
    /// </summary>
    public enum FlagMode
    {
        /// <summary>
        /// default
        /// </summary>
        None = 0,

        /// <summary>
        /// Logogram Tag: $text
        /// </summary>
        Logogram,

        /// <summary>
        /// Full Tag:${text}
        /// </summary>
        Full,

        /// <summary>
        /// Comment tag:$* text *$
        /// </summary>
        Comment
    }
}