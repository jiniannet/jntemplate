/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate.Parser
{
    public class CharScanner
    {
        const Char EOF = ' ';
        private Int32 index;
        private Int32 start;
        private String document;
        public CharScanner(String text)
        {
            this.document = text==null ? String.Empty : text;
            this.index = 0;
            this.start = 0;
        }
        public Int32 Index
        {
            get { return this.index; }
        }
        public Boolean Next()
        {
            return Next(1);
        }
        public Boolean Next(Int32 i)
        {
            if (this.index + i > this.document.Length)
                return false;
            this.index += i;
            return true;
        }
        public Boolean Back()
        {
            return Back(1);
        }
        public Boolean Back(Int32 i)
        {
            if (this.index < i)
                return false;
            this.index -= i;
            return true;
        }
        public Char Read()
        {
            return Read(0);
        }
        public Char Read(Int32 i)
        {
            if (this.index + i >= this.document.Length)
                return EOF;
            return this.document[this.index + i];
        }
        public Boolean IsMatch(Char[] list)
        {
            return IsMatch(list, 0);
        }

        public Boolean IsEnd()
        {
            return this.index >= this.document.Length;
        }

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

        public String GetString()
        {
            String value = GetString(this.start);
            this.start = this.index;
            return value;
        }
        public String GetString(Int32 x)
        {
            return GetString(x, this.index);
        }
        public String GetString(Int32 x, Int32 y)
        {
            return this.document.Substring(x, y - x);
        }
    }
}