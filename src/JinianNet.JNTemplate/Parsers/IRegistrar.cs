﻿/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
#define ALLOWCOMMENT
using JinianNet.JNTemplate.Hosting;
using System;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// The tag registrar
    /// </summary>
    public interface IRegistrar
    {
        /// <summary>
        /// Regiser the tag.
        /// </summary>
        /// <param name="host">The <see cref="IHost"/>.</param>
        void Regiser(IHost host);
    }
}
