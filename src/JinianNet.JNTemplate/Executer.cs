/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

#if NETCOREAPP || NETSTANDARD
using System.Threading.Tasks;
#endif


namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 执行器
    /// </summary>
    public abstract class Executer : Executer<object>,IExecuter, IExecuter<object>
    {
 
    }
}
