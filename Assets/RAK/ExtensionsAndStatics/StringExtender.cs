using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace RAK
{
    public static class StringExtender
    {
        public static string COrange(this string str)
        {
            return str.Color(new Color(1,.5f,0));
        }
        public static string CRed(this string str)
        {
            return str.Color(UnityEngine.Color.red);
        }
        public static string CGreen(this string str)
        {
            return str.Color(UnityEngine.Color.green);
        }
        public static string CBlue(this string str)
        {
            return str.Color(UnityEngine.Color.blue);
        }
        public static string CCyan(this string str)
        {
            return str.Color(UnityEngine.Color.cyan);
        }

        public static string CMagenta(this string str)
        {
            return str.Color(UnityEngine.Color.magenta);
        }
        public static string CYellow(this string str)
        {
            return str.Color(UnityEngine.Color.yellow);
        }

        static UnityEngine.Color ColorGrey = new UnityEngine.Color(.5f, .5f, .5f);
        static UnityEngine.Color ColorLightYellow = new UnityEngine.Color(1,1,.5f);
        static UnityEngine.Color ColorLime = new UnityEngine.Color(.5f, 1, 0f);
        public static string CGrey(this string str)
        {
            return str.Color(ColorGrey);
        }
        public static string CLightYellow(this string str)
        {
            return str.Color(ColorLightYellow);
        }
        public static string CLime(this string str)
        {
            return str.Color(ColorLime);
        }


        public static string Color(this string str, Color color)
        {
            string s = ColorUtility.ToHtmlStringRGB(color);
            return $"<color=#{s}>{str}</color>";

        }
        public static void Debug(this string str)
        {
            UnityEngine.Debug.Log(str);
        }
        public static void Debug(this string str, string hexColorCode)
        {
            UnityEngine.Debug.Log($"<color=#{hexColorCode}>{str}</color>");
        }
        public static void Debug(this string str, Color color)
        {
            string s = ColorUtility.ToHtmlStringRGB(color);
            UnityEngine.Debug.Log($"<color=#{s}>{str}</color>");
        }
        public static string ConvertCamelCase(this string input)
        {
            // Add a space before each capital letter (except the first one)
            string result = Regex.Replace(input, "([A-Z])", " $1").Trim();

            // Convert the first letter to uppercase (for cases where the input starts with a lowercase letter)
            result = char.ToUpper(result[0]) + result.Substring(1);

            return result;
        }
    }
}