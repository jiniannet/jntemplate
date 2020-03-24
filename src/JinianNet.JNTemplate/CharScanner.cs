/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 字符扫描器
    /// </summary>
    public class CharScanner
    {
        /// <summary>
        /// 结束字符
        /// </summary>
        private const char EOF = '\0';
        private int index;
        private int start;
        private string document;
        /// <summary>
        /// CharScanner
        /// </summary>
        /// <param name="text">扫描内容</param>
        public CharScanner(string text)
        {
            this.document = (text ?? string.Empty);
        }
        /// <summary>
        /// 当前索引
        /// </summary>
        public int Index
        {
            get { return this.index; }
        }
        /// <summary>
        /// 前进1个字符
        /// </summary>
        /// <returns></returns>
        public bool Next()
        {
            return Next(1);
        }
        /// <summary>
        /// 前进指定介字符
        /// </summary>
        /// <param name="i">数目</param>
        /// <returns></returns>
        public bool Next(int i)
        {
            if (this.index + i > this.document.Length)
            {
                return false;
            }
            this.index += i;
            return true;
        }
        /// <summary>
        /// 后退一个字符
        /// </summary>
        /// <returns></returns>
        public bool Back()
        {
            return Back(1);
        }
        /// <summary>
        /// 后退指定字符
        /// </summary>
        /// <param name="i">数目</param>
        /// <returns></returns>
        public bool Back(int i)
        {
            if (this.index < i)
            {
                return false;
            }
            this.index -= i;
            return true;
        }
        /// <summary>
        /// 读取当前字符
        /// </summary>
        /// <returns></returns>
        public char Read()
        {
            return Read(0);
        }
        /// <summary>
        /// 读取当前索引位开始后第i个字符
        /// </summary>
        /// <param name="i">数目</param>
        /// <returns></returns>
        public char Read(int i)
        {
            if (this.index + i >= this.document.Length)
            {
                return EOF;
            }
            return this.document[this.index + i];
        }
        /// <summary>
        /// 当前是否匹配指定对象
        /// </summary>
        /// <param name="list">匹配对象</param>
        /// <returns></returns>
        public bool IsMatch(char[] list)
        {
            return IsMatch(list, 0);
        }
        /// <summary>
        /// 是否扫描结束
        /// </summary>
        /// <returns></returns>
        public bool IsEnd()
        {
            return this.index >= this.document.Length;
        }
        /// <summary>
        /// 是否匹配指定对象
        /// </summary>
        /// <param name="list">匹配对象</param>
        /// <param name="n">从当前索引后第N位开始</param>
        /// <returns></returns>
        public bool IsMatch(char[] list, int n)
        {
            n = this.index + n;
            if (this.document.Length >= n + list.Length)
            {
                for (int i = 0; i < list.Length; i++)
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
        /// 获取当前start到index的字符串(会处理转义符)
        /// </summary>
        /// <returns></returns>
        public string GetEscapeString()
        {
            string value = GetEscapeString(this.start, this.index);
            this.start = this.index;
            return value;
        }

        /// <summary>
        /// 截取start到index的字符串
        /// </summary>
        /// <returns></returns>
        public string GetString()
        {
            string value = GetString(this.start,this.index);
            this.start = this.index;
            return value;
        }
        /// <summary>
        /// 截取x到y的转义字符串
        /// </summary>
        /// <param name="x">开始索引</param>
        /// <param name="y">结束索引</param>
        /// <returns></returns>
        public string GetEscapeString(int x, int y)
        {
            List<char> cs = new List<char>();
            for (int i = x; i < y; i++)
            {
                if (this.document[i] == '\\')
                {
                    switch (this.document[i + 1])
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
                            cs.Add(this.document[i]);
                            break;
                    }
                }
                else
                {
                    cs.Add(this.document[i]);
                }
            }
            return new string(cs.ToArray());
        }
        /// <summary>
        /// 截取x到y的字符串
        /// </summary>
        /// <param name="x">开始索引</param>
        /// <param name="y">结束索引</param>
        /// <returns></returns>
        public string GetString(int x, int y)
        {
            return this.document.Substring(x, y - x);
        }
    }
}