/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Compile
{
    /// <summary>
    /// Compile Context
    /// </summary>
    public class CompileContext : Context
    {
        private int seed = 0;
        /// <summary>
        /// ctox
        /// </summary>
        public CompileContext()
            : base()
        {

        }
        /// <summary>
        /// template name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Type builder
        /// </summary>
        public TypeBuilder TypeBuilder { get; set; }
        /// <summary>
        /// IL Generator
        /// </summary>
        public ILGenerator Generator { get; set; }
        /// <summary>
        /// data
        /// </summary>
        public VariableScope Data { get; set; }

        /// <summary>
        /// set type
        /// </summary>
        /// <param name="name">key</param>
        /// <param name="type">type</param>
        public void Set(string name, Type type)
        {
            Data.SetElement(name, new VariableElement(type, null));
        }

        /// <summary>
        /// seed 
        /// </summary>
        internal int Seed
        {
            get
            {
                if (seed == int.MaxValue)
                {
                    seed = 0;
                }
                return seed++;
            }
        }
    }
}
