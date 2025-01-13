using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drake.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Get string value between [first] a and [next] b.
        /// </summary>
        public static string _Between(this string value, string a, string b)
        {
            int posA = value.IndexOf(a);
            int posB = value.IndexOf(b, posA + a.Length);
            if (posA == -1)
            {
                return "";
            }
            if (posB == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        /// <summary>
        /// Returns a string, trimming off anything after a string.
        /// If the string isn't found, it will return the original string.
        /// </summary>
        public static string _TrimSuffix(this string value, string a)
        {
            value += a; // Add suffix just so we always get a hit.
            int posA = value.IndexOf(a);
            if (posA == -1)
            {
                return value;
            }
 
            return value.Substring(0, posA);
        }

        /// <summary>
        /// Get string value between [first] a and [last] b.
        /// </summary>
        public static string _BetweenFirstLast(this string value, string a, string b)
        {
            int posA = value.IndexOf(a);
            int posB = value.LastIndexOf(b);
            if (posA == -1)
            {
                return "";
            }
            if (posB == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        /// <summary>
        /// Remove all strings between [first] a and [first after] b.
        /// </summary>
        public static string _RemoveBetween(this string value, string a, string b)
        {
            int posA = value.IndexOf(a);
            int posB = value.IndexOf(b, posA + a.Length);
            if (posA == -1)
            {
                return value;
            }
            if (posB == -1)
            {
                return value;
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA > posB)
            {
                return "";
            }
            return value.Remove(adjustedPosA, posB - adjustedPosA);
        }

        /// <summary>
        /// Get string value between [first] a and [first after] b.
        /// </summary>
        public static string _ReplaceAtPosition(this string value, int startPosition, int length, string replacementString)
        {
            var aStringBuilder = new StringBuilder(value);
            aStringBuilder.Remove(startPosition, length);
            aStringBuilder.Insert(startPosition, replacementString);

            return aStringBuilder.ToString();
        }

        /// <summary>
        /// Capitalizes the first letter of a string. This is not camelcase.
        /// </summary>
        /// <returns>The modified string.</returns>
        public static string _CapitalizeFirstCharacter(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return value;

            return value.First().ToString().ToUpper() + String.Join("", value.Skip(1));
        }

        /// <summary>
        /// Removes all characters of a character value from a string.
        /// </summary>
        /// <param name="value">A string object</param>
        /// <param name="stripChar">Character to remove from string</param>
        /// <returns>The modified string.</returns>
        public static string _Strip(this string value, char stripChar)
        {
            if (String.IsNullOrEmpty(value))
                return value;

            return new String(value.ToCharArray().Where(x => x != stripChar).ToArray());
        }

        /// <summary>
        /// Returns true if a string is an int.
        /// </summary>
        /// <param name="str">The string to check</param>
        /// <returns>A bool value indicating whether or not this string is an int.</returns>
        public static bool IsInt(this string str)
        {
            int dummyInt;
            return int.TryParse(str, out dummyInt);
        }
        /// <summary>
        /// Returns true if a string is a long.
        /// </summary>
        /// <param name="str">The string to check</param>
        /// <returns>A bool value indicating whether or not this string is a long.</returns>
        public static bool IsLong(this string str)
        {
            long dummyLong;
            return long.TryParse(str, out dummyLong);
        }
        /// <summary>
        /// Returns true if a string is a long.
        /// </summary>
        /// <param name="str">The string to check</param>
        /// <returns>A bool value indicating whether or not this string is a long.</returns>
        public static bool IsFloat(this string str)
        {
            long dummyFloat;
            return long.TryParse(str, out dummyFloat);
        }
        /// <summary>
        /// Returns true if a string is a decimal.
        /// </summary>
        /// <param name="str">The string to check</param>
        /// <returns>A bool value indicating whether or not this string is a decimal.</returns>
        public static bool IsDecimal(this string str)
        {
            decimal dummyDecimal;
            return decimal.TryParse(str, out dummyDecimal);
        }


        public static Dictionary<string, string> ToDictionary(this string s, char valueDelim, char pairDelim)
        {
            var segments = s.Split(new char[] { pairDelim }, StringSplitOptions.RemoveEmptyEntries);
            var entries = segments.Select(item => item.Split(new char[] { valueDelim }, StringSplitOptions.RemoveEmptyEntries));
            var kvps = entries.Select(kvp => new KeyValuePair<string, string>(kvp[0].Trim(), kvp.Length > 1 ? kvp[1] : string.Empty));
            return kvps.ToDictionary(k => k.Key, v => v.Value, StringComparer.OrdinalIgnoreCase);
        }
    }
}
