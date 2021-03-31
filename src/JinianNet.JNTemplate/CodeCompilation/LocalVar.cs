/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.CodeCompilation
{
    /// <summary>
    /// The local variable.
    /// </summary>
    public class LocalVar
    {
        private ILGenerator il;
        private Dictionary<string, int> dict;
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalVar"/> class
        /// </summary>
        /// <param name="generator">The <see cref="ILGenerator"/>.</param>
        public LocalVar(ILGenerator generator)
        {
            il = generator;
            dict = new Dictionary<string, int>();
        }
        /// <summary>
        /// Declares a local variable of the specified type.
        /// </summary>
        /// <param name="type"> A <see cref="Type"/> object that represents the type of the local variable.</param>
        public void Declare(Type type)
        {
            Declare(null,type);
        }
        /// <summary>
        /// Declares a local variable of the specified type.
        /// </summary>
        /// <param name="varName">The name of the variable.</param>
        /// <param name="type"> A <see cref="Type"/> object that represents the type of the local variable.</param>
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
        /// Gets the zero-based index of the local variable within the method body.
        /// </summary>
        /// <param name="varName">The name of the variable.</param>
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
