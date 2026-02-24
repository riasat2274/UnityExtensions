using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RAK
{
    public static class FormatingExtender 
    {
        public static double IdleRounder(this double value)
        {
            int exponent = (int)Math.Floor(Math.Log(Math.Abs(value), 1000));
            double newValue = value / Math.Pow(1000, exponent);


            float fl = (float)newValue;
            if (fl < 5)
            {
                return Math.Pow(1000, exponent) * Mathf.Round(fl * 100) / 100;
            }
            else if (fl < 20)
            {
                return Math.Pow(1000, exponent) * Mathf.Round(fl);
            }
            else if (fl < 150)
            {
                return Math.Pow(1000, exponent) * Mathf.Round(fl / 5) * 5;
            }
            else if (fl < 360)
            {
                return Math.Pow(1000, exponent) * Mathf.Round(fl / 10) * 10;
            }
            else if (fl < 600)
            {
                return Math.Pow(1000, exponent) * Mathf.Round(fl / 20) * 20;
            }
            else
            {
                return Math.Pow(1000, exponent) * Mathf.Round(fl / 50) * 50;
            }
        }
        public static string IdleFormatter(this double value)
        {
            if (value == 0) return "0";

            int exponent = (int)Math.Floor(Math.Log(Math.Abs(value), 1000));
            double newValue = value / Math.Pow(1000, exponent);
            string f = "0.##";
            if (newValue > 100)
            {
                newValue = Math.Round(newValue);
                f = "F0";
            }

            switch (exponent)
            {
                case 0:
                    return newValue.ToString("F0");
                case 1:
                    return newValue.ToString(f) + "K";
                case 2:
                    return newValue.ToString(f) + "M";
                case 3:
                    return newValue.ToString(f) + "B";
                case 4:
                    return newValue.ToString(f) + "T";
                case 5:
                    return newValue.ToString(f) + "Qa";
                case 6:
                    return newValue.ToString(f) + "Qi";
                case 7:
                    return newValue.ToString(f) + "Sx";
                case 8:
                    return newValue.ToString(f) + "Sp";
                case 9:
                    return newValue.ToString(f) + "Oc";
                default:
                    // If the exponent is greater than 9, just return the original value
                    return value.ToString(f);
            }
        }
        public static string ConvertToPositionString(this int value)
        {
            switch (value)
            {
                case 1:
                    return $"{value}st";
                case 2:
                    return $"{value}nd";
                case 3:
                    return $"{value}rd";
                default:
                    return $"{value}th";
            }
        }
        public static string ConvertToPositionStringRichSize(this int value, int richSize)
        {
            switch (value)
            {
                case 1:
                    return $"{value}<size={richSize}>st</size>";
                case 2:
                    return $"{value}<size={richSize}>nd</size>";
                case 3:
                    return $"{value}<size={richSize}>rd</size>";
                default:
                    return $"{value}<size={richSize}>th</size>";
            }
        }

    }
}
