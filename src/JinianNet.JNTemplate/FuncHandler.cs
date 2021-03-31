/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Encapsulates a method that returns a value.
    /// </summary>
    /// <param name="args">The parameters.</param>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    [Obsolete("This delegate is obsolete; use Func<T..> ")]
    public delegate object FuncHandler(params object[] args);
}