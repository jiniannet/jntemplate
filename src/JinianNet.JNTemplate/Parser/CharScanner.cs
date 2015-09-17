/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;

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
        private const Char EOF = '\0';
        private Int32 _index;
        private Int32 _start;
        private String _document;
        /// <summary>
        /// CharScanner
        /// </summary>
        /// <param name="text">扫描内容</param>
        public CharScanner(String text)
        {
            this._document = (text ?? String.Empty);
        }
        /// <summary>
        /// 当前索引
        /// </summary>
        public Int32 Index
        {
            get { return this._index; }
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
            if (this._index + i > this._document.Length)
            {
                return false;
            }
            this._index += i;
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
            if (this._index < i)
            {
                return false;
            }
            this._index -= i;
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
            if (this._index + i >= this._document.Length)
            {
                return EOF;
            }
            return this._document[this._index + i];
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
            return this._index >= this._document.Length;
        }
        /// <summary>
        /// 是否匹配指定对象
        /// </summary>
        /// <param name="list">匹配对象</param>
        /// <param name="n">从当前索引后第N位开始</param>
        /// <returns></returns>
        public Boolean IsMatch(Char[] list, Int32 n)
        {
            n = this._index + n;
            if (this._document.Length >= n + list.Length)
            {
                for (Int32 i = 0; i < list.Length; i++)
                {
                    if (this._document[n + i] != list[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取当前start到index的字符串(会处理转义符)
        /// </summary>
        /// <returns></returns>
        public String GetEscapeString()
        {
            String value = GetEscapeString(this._start, this._index);
            this._start = this._index;
            return value;
        }

        /// <summary>
        /// 截取start到index的字符串
        /// </summary>
        /// <returns></returns>
        public String GetString()
        {
            String value = GetString(this._start,this._index);
            this._start = this._index;
            return value;
        }
        /// <summary>
        /// 截取x到y的转义字符串
        /// </summary>
        /// <param name="x">开始索引</param>
        /// <param name="y">结束索引</param>
        /// <returns></returns>
        public String GetEscapeString(Int32 x, Int32 y)
        {
            List<Char> cs = new List<Char>();
            for (Int32 i = x; i < y; i++)
            {
                if (this._document[i] == '\\')
                {
                    switch (this._document[i + 1])
                    {
                        case '0':
                            cs.Add('\0');
                            i++;
                            break;
                        case '"':
                            cs.Add('\"');
                            i++;
                            break;
                        case '\\':
                            cs.Add('\\');
                            i++;
                            break;
                        case 'a':
                            cs.Add('\a');
                            i++;
                            break;
                        case 'b':
                            cs.Add('\b');
                            i++;
                            break;
                        case 'f':
                            cs.Add('\f');
                            i++;
                            break;
                        case 'n':
                            cs.Add('\n');
                            i++;
                            break;
                        case 'r':
                            cs.Add('\r');
                            i++;
                            break;
                        case 't':
                            cs.Add('\t');
                            i++;
                            break;
                        case 'v':
                            cs.Add('\v');
                            i++;
                            break;
                        default:
                            cs.Add(this._document[i]);
                            break;
                    }
                }
                else
                {
                    cs.Add(this._document[i]);
                }
            }
            return new String(cs.ToArray());
        }
        /// <summary>
        /// 截取x到y的字符串
        /// </summary>
        /// <param name="x">开始索引</param>
        /// <param name="y">结束索引</param>
        /// <returns></returns>
        public String GetString(Int32 x, Int32 y)
        {
            return this._document.Substring(x, y - x);
        }
    }
}