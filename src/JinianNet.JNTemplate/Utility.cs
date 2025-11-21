/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;


#if !NF35 && !NF20
using System.Threading.Tasks;
#endif
namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Contains utilities that the jntemplate uses.
    /// </summary>
    public static class Utility
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
            return !input.Equals(0);
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
            return !input.Equals(0);
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
                    return (bool)input;
                default:
                    return true;
            }
        }


        /// <summary>
        /// Returns the hash code for this string.
        /// </summary>
        /// <param name="value"> A 64-bit signed integer hash code.</param>
        /// <returns></returns>
        public static long ToHashCode(Type value)
        {
            if (value==null)
                return 0L;
            return 0x7fffffffL +  value.GetHashCode();
        }

        /// <summary>
        /// Returns the hash code for this string.
        /// </summary>
        /// <param name="value"> A 64-bit signed integer hash code.</param>
        /// <returns></returns>
        public static long ToHashCode(string value)
        {
            if (value==null)
                return 0L;
            return 0x7fffffffL +  value.GetHashCode();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string ContentToTemplateName(string content)
        {
            return $"ContentTemp{ToHashCode(content)}";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string PathToTemplateName(string path)
        {
            return $"FileTemp{ToHashCode(path)}";
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
            return IsLetter(value) || char.IsNumber(value) || value == '_';
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
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(Dictionary<object, object> dict)
        {
            var tDict = new Dictionary<TKey, TValue>();

            foreach (var kv in dict)
            {
                var key = Change<TKey>(kv.Key);
                var value = Change<TValue>(kv.Value);
                tDict[key] = value;
            }
            return tDict;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Change<T>(object value)
        {
            if (value is T tValue)
                return tValue;
            var type = typeof(T);
            if (type ==typeof(string))
                return (T)((object)value.ToString());
            return (T)value;
        }

        /// <summary>
        /// Generates default values based on the specified type
        /// </summary>
        /// <typeparam name="T">the Type of the object.</typeparam>
        /// <returns>The efault values.</returns>
        public static T GenerateDefaultValue<T>()
        {
            return default(T);
        }
#if !NF35 && !NF20
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static T ExcuteTask<T>(Task<T> task)
        {
            if (task == null)
            {
                return default(T);
            }
#if NF40
            task.Wait();
            return task.Result;
#else
            return task.GetAwaiter().GetResult();
#endif
        }


#if NF40
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static string ExcuteReturnTaskAsync<T>(Task<T> task)
        {
            task.Wait();
            return task.Result?.ToString();
        }
#else
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static async Task<string> ExcuteReturnTaskAsync<T>(Task<T> task)
        {
            var result = await task;
            return result?.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static async Task<string> ExcuteTaskAsync(Task task)
        {
            await task;
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Task<string> ExcuteStringAsync(string value)
        {
            return Task.FromResult<string>(value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Task WriteAsync(System.IO.TextWriter writer, string value)
        {
            return writer.WriteAsync(value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Task WriteAsync(System.IO.TextWriter writer, object value)
        {
            return writer.WriteAsync(value?.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Task WriteAsync(System.IO.TextWriter writer, ValueType value)
        {
            return writer.WriteAsync(value?.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writer"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public static async Task WriteTaskAsync<T>(System.IO.TextWriter writer, Task<T> task)
        {
            var result = await task;
            string value = result?.ToString();
            if (value != null)
            {
                await writer.WriteAsync(value?.ToString());
            }
        }
#endif
#endif
    }
}