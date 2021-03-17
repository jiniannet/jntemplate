/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Compile
{
    /// <summary>
    /// 
    /// </summary>
    public class LocalVar
    {
        private ILGenerator il;
        private Dictionary<string, int> dict;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="generator"></param>
        public LocalVar(ILGenerator generator)
        {
            il = generator;
            dict = new Dictionary<string, int>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public void Declare(Type type)
        {
            Declare(null,type);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int Declare(string varName, Type type)
        {
            var local = il.DeclareLocal(type);
            if (varName != null)
            {
                dict[varName] = local.LocalIndex;
            }
            return local.LocalIndex;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool CanDeclare(string value)
        {
            return dict.ContainsKey(value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public int this[string varName]
        {
            get
            {
                return dict[varName];
            }
        }
    }
}
