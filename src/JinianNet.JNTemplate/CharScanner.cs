/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// The char scanner.
    /// </summary>
    public class CharScanner
    {
        private const char EOF = '\0';
        private int index;
        private int start;
        private string document;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharScanner"/> class
        /// </summary>
        /// <param name="text">The contents.</param>
        public CharScanner(string text)
        {
            this.document = (text ?? string.Empty);
        }
        /// <summary>
        /// The zero-based index in the <see cref="CharScanner"/> at which scanned.
        /// </summary>
        public int Index
        {
            get { return this.index; }
        }

        /// <summary>
        /// Forward
        /// </summary>
        /// <returns></returns>
        public bool Next()
        {
            return Next(1);
        }
        /// <summary>
        /// Forward specified character
        /// </summary>
        /// <param name="i">Forward number</param>
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
        /// Back
        /// </summary>
        /// <returns></returns>
        public bool Back()
        {
            return Back(1);
        }
        /// <summary>
        /// Back specified character.
        /// </summary>
        /// <param name="i">Back number.</param>
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
        /// Reads the characters from the current string.
        /// </summary>
        /// <returns>A char. </returns>
        public char Read()
        {
            return Read(0);
        }
        /// <summary>
        /// Reads the characters from the current string.
        /// </summary>
        /// <param name="i">The start index.</param>
        /// <returns>A char. </returns>
        public char Read(int i)
        {
            if (this.index + i >= this.document.Length)
            {
                return EOF;
            }
            return this.document[this.index + i];
        }
        /// <summary>
        /// Indicates whether finds a match in a specified input chars.
        /// </summary>
        /// <param name="list">The chars to search for a match.</param> 
        /// <returns>true if the chars finds a match; otherwise, false.</returns>
        public bool IsMatch(char[] list)
        {
            return IsMatch(list, 0);
        }
        /// <summary>
        /// Indicates whether is end.
        /// </summary>
        /// <returns></returns>
        public bool IsEnd()
        {
            return this.index >= this.document.Length;
        }
        /// <summary>
        /// Indicates whether finds a match in a specified input chars.
        /// </summary>
        /// <param name="list">The chars to search for a match.</param>
        /// <param name="n">The start index.</param>
        /// <returns>true if the chars finds a match; otherwise, false.</returns>
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
        /// Retrieves a substring from this instance.
        /// </summary>
        /// <returns>A string.</returns>
        public string GetEscapeString()
        {
            string value = GetEscapeString(this.start, this.index);
            this.start = this.index;
            return value;
        }

        /// <summary>
        /// Retrieves a substring from this instance.
        /// </summary>
        /// <returns>A string.</returns>
        public string GetString()
        {
            string value = GetString(this.start,this.index);
            this.start = this.index;
            return value;
        }
        /// <summary>
        /// Retrieves a substring from this instance. 
        /// </summary>
        /// <param name="x">The zero-based starting character position of a substring in this instance.</param>
        /// <param name="y">The zero-based ended character position of a substring in this instance.</param>
        /// <returns>A string.</returns>
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
        /// Retrieves a substring from this instance. The substring starts at a specified character position and continues to the end of the string.
        /// </summary>
        /// <param name="x">The zero-based starting character position of a substring in this instance.</param>
        /// <param name="y">The zero-based ended character position of a substring in this instance.</param>
        /// <returns>A string.</returns>
        public string GetString(int x, int y)
        {
            return this.document.Substring(x, y - x);
        }
    }
}