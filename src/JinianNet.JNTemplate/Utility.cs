/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System; 

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Contains utilities that the jntemplate uses.
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Converts the specified string representation of a logical value to its Boolean  equivalent.
        /// </summary>
        /// <param name="input">A string containing the value to convert.</param>
        /// <returns>true if value is equivalent to True String; false if value is equivalent</returns>
        public static bool StringToBoolean(string input)
        {
            if ("true".Equals(input, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Converts the <see cref="string"/> to its <see cref="bool"/> equivalent.
        /// </summary>
        /// <param name="input">A string containing the value to convert.</param>
        /// <returns>false if value is nul or "";otherwise, true.</returns>
        public static bool ToBoolean(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Converts the <see cref="int"/> to its <see cref="bool"/> equivalent.
        /// </summary>
        /// <param name="input">A number containing the value to convert.</param>
        /// <returns>false if value is 0;otherwise, true.</returns>
        public static bool ToBoolean(int input)
        {
            if (input == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Converts the <see cref="double"/> to its <see cref="bool"/> equivalent.
        /// </summary>
        /// <param name="input">A double containing the value to convert.</param>
        /// <returns>false if value is 0;otherwise, true.</returns>
        public static bool ToBoolean(double input)
        {
            if (input == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Converts the <see cref="decimal"/> to its <see cref="bool"/> equivalent.
        /// </summary>
        /// <param name="input">A decimal containing the value to convert.</param>
        /// <returns>false if value is 0;otherwise, true.</returns>
        public static bool ToBoolean(decimal input)
        {
            if (input == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Converts the <see cref="Int16"/> to its <see cref="bool"/> equivalent.
        /// </summary>
        /// <param name="input">A Int16 containing the value to convert.</param>
        /// <returns>false if value is 0;otherwise, true.</returns>
        public static bool ToBoolean(Int16 input)
        {
            if (input == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Converts the <see cref="Int64"/> to its <see cref="bool"/> equivalent.
        /// </summary>
        /// <param name="input">A Int64 containing the value to convert.</param>
        /// <returns>false if value is 0;otherwise, true.</returns>
        public static bool ToBoolean(Int64 input)
        {
            if (input == 0)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Converts the <see cref="float"/> to its <see cref="bool"/> equivalent.
        /// </summary>
        /// <param name="input">A float containing the value to convert.</param>
        /// <returns>false if value is 0;otherwise, true.</returns>
        public static bool ToBoolean(float input)
        {
            if (input == 0)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Converts the <see cref="object"/> to its <see cref="bool"/> equivalent.
        /// </summary>
        /// <param name="input">A object containing the value to convert.</param>
        /// <returns>false if value is null;otherwise, true.</returns>
        public static bool ToBoolean(object input)
        {
            if (input == null)
            {
                return false;
            }
            var t = input.GetType();
            switch (t.FullName)
            {
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                    return ToBoolean(long.Parse(input.ToString()));
                case "System.Single":
                case "System.Decimal":
                case "System.Double":
                    return ToBoolean(decimal.Parse(input.ToString()));
                case "System.String":
                    return ToBoolean(input.ToString());
                case "System.Boolean":
                    return bool.Parse(input.ToString());
                default:
                    return true;
            }
        }
        /// <summary>
        /// Indicates whether the specified Unicode character is categorized as a Unicode letter.
        /// </summary>
        /// <param name="value">The Unicode character to evaluate.</param>
        /// <returns></returns>
        public static bool IsLetter(char value)
        {
            return char.IsLower(value) || char.IsUpper(value);
        }

        /// <summary>
        /// Is it a letter, a combination of underscores and numbers
        /// </summary>
        /// <param name="value">The Unicode character to evaluate.</param>
        /// <returns></returns>
        public static bool AllowWord(char value)
        {
            return char.IsLower(value) || char.IsUpper(value) || char.IsNumber(value) || value == '_';
        }

        /// <summary>
        /// Determines whether two specified System.String objects have the same value. A parameter specifies the culture, case, and sort rules used in the comparison.
        /// </summary>
        /// <param name="x">The first string to compare, or null.</param>
        /// <param name="y">The second string to compare, or null.</param>
        /// <returns>true if the value of the a parameter is equal to the value of the b parameter; otherwise, false.</returns>
        public static bool IsEqual(string x, string y)
        {
            if (x == null || y == null)
                return x == y;
            return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns md5 code for this string.
        /// </summary>
        /// <param name="input">The input to compute the md5 code for.</param>
        /// <returns>a hex string</returns>
        public static string Md5(string input)
        {
            using (var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] data = md5.ComputeHash(bytes);
                string byte2String = null;

                for (int i = 0; i < data.Length; i++)
                {
                    byte2String += data[i].ToString("x2");
                }
                return byte2String;
            }
        }
    }
}