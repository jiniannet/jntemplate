/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Resources
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITemplateWatcherProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ITemplateWatcher"/> class
        /// </summary>
        /// <returns></returns>
        ITemplateWatcher Create();
    }
}
