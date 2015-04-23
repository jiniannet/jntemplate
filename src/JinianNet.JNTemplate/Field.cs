/*****************************************************
   Copyright (c) 2013-2015 jiniannet (http://www.jiniannet.com)

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

   Redistributions of source code must retain the above copyright notice
 *****************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 系统常用字段
    /// </summary>
    public class Field
    {
        /// <summary>
        /// 当前程序版本
        /// </summary>
        public const String Version = "1.2";
        internal const String KEY_FOREACH = "foreach";
        internal const String KEY_IF = "if";
        internal const String KEY_ELSEIF = "elseif";
        internal const String KEY_ELSE = "else";
        internal const String KEY_SET = "set";
        internal const String KEY_LOAD = "load";
        internal const String KEY_INCLUDE = "include";
        internal const String KEY_END = "end";
        internal const String KEY_FOR = "for";
        internal const String KEY_IN = "in";
    }


}
