/*****************************************************
   Copyright (c) 2013-2014 jiniannet (http://www.jiniannet.com)

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
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate.Parser
{
    /// <summary>
    /// 字符扫描器
    /// </summary>
    public class CharScanner
    {
        /// <summary>
        /// 结束字符
        /// </summary>
        const Char EOF = '\0';
        private Int32 index;
        private Int32 start;
        private String document;
        /// <summary>
        /// CharScanner
        /// </summary>
        /// <param name="text">搜描内容</param>
        public CharScanner(String text)
        {
            this.document = text==null ? String.Empty : text;
            this.index = 0;
            this.start = 0;
        }
        /// <summary>
        /// 当前索引
        /// </summary>
        public Int32 Index
        {
            get { return this.index; }
        }
        /// <summary>
        /// 前进1个字符
        /// </summary>
        /// <returns></returns>
        public Boolean Next()
        {
            return Next(1);
        }
        /// <summary>
        /// 前进指定介字符
        /// </summary>
        /// <param name="i">数目</param>
        /// <returns></returns>
        public Boolean Next(Int32 i)
        {
            if (this.index + i > this.document.Length)
                return false;
            this.index += i;
            return true;
        }
        /// <summary>
        /// 后退一个字符
        /// </summary>
        /// <returns></returns>
        public Boolean Back()
        {
            return Back(1);
        }
        /// <summary>
        /// 后退指定字符
        /// </summary>
        /// <param name="i">数目</param>
        /// <returns></returns>
        public Boolean Back(Int32 i)
        {
            if (this.index < i)
                return false;
            this.index -= i;
            return true;
        }
        /// <summary>
        /// 读取当前字符
        /// </summary>
        /// <returns></returns>
        public Char Read()
        {
            return Read(0);
        }
        /// <summary>
        /// 读取当前索引位开始后第i个字符
        /// </summary>
        /// <param name="i">数目</param>
        /// <returns></returns>
        public Char Read(Int32 i)
        {
            if (this.index + i >= this.document.Length)
                return EOF;
            return this.document[this.index + i];
        }
        /// <summary>
        /// 当前是否匹配指定对象
        /// </summary>
        /// <param name="list">匹配对象</param>
        /// <returns></returns>
        public Boolean IsMatch(Char[] list)
        {
            return IsMatch(list, 0);
        }
        /// <summary>
        /// 是否扫描结束
        /// </summary>
        /// <returns></returns>
        public Boolean IsEnd()
        {
            return this.index >= this.document.Length;
        }
        /// <summary>
        /// 是否匹配指定对象
        /// </summary>
        /// <param name="list">匹配对象</param>
        /// <param name="n">从当前索引后第N位开始</param>
        /// <returns></returns>
        public Boolean IsMatch(Char[] list, Int32 n)
        {
            n = this.index + n;
            if (this.document.Length >= n + list.Length)
            {
                for (Int32 i = 0; i < list.Length; i++)
                {
                    if (this.document[n + i] != list[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 截取start到index的字符串
        /// </summary>
        /// <returns></returns>
        public String GetString()
        {
            String value = GetString(this.start);
            this.start = this.index;
            return value;
        }
        /// <summary>
        /// 截取s到index的字符串
        /// </summary>
        /// <param name="x">开始索引</param>
        /// <returns></returns>
        public String GetString(Int32 x)
        {
            return GetString(x, this.index);
        }
        /// <summary>
        /// 截取x到y的字符串
        /// </summary>
        /// <param name="x">开始索引</param>
        /// <param name="y">结束索引</param>
        /// <returns></returns>
        public String GetString(Int32 x, Int32 y)
        {
            return this.document.Substring(x, y - x);
        }
    }
}