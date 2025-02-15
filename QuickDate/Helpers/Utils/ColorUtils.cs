using Android.Graphics;
using Android.Graphics.Drawables;
using System;
using System.Globalization;

namespace QuickDate.Helpers.Utils
{
    public static class ColorUtils
    {
        /// <summary>
        /// Convert an integer to a string of hexDecimal numbers.
        /// </summary>
        /// <param name="n">The int to convert to Hex representation</param>
        /// <param name="len">number of digits in the hex string. Pads with leading zeros.</param>
        /// <returns></returns>
        public static string IntToHexString(int n, int len)
        {
            char[] ch = new char[len--];
            for (int i = len; i >= 0; i--)
            {
                ch[len - i] = ByteToHexChar((byte)((uint)(n >> 4 * i) & 15));
            }
            return new string(ch);
        }

        /// <summary>
        /// Convert a byte to a hexDecimal char
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private static char ByteToHexChar(byte b)
        {
            if (b > 15)
                throw new Exception("IntToHexChar: input out of range for Hex value");
            return b < 10 ? (char)(b + 48) : (char)(b + 55);
        }

        /// <summary>
        /// Convert a hexDecimal string to an base 10 integer
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int HexStringToInt(string str)
        {
            str = str.Replace("#", "");

            int intValue = int.Parse(str, NumberStyles.HexNumber);
            Console.WriteLine(intValue);

            int value = 0;
            for (int i = 0; i < str.Length; i++)
            {
                value += HexCharToInt(str[i]) << ((str.Length - 1 - i) * 4);
            }
            return value;
        }

        /// <summary>
        /// Convert a hex char to it an integer.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        private static int HexCharToInt(char ch)
        {
            if (ch < 48 || ch > 57 && ch < 65 || ch > 70)
                throw new Exception("HexCharToInt: input out of range for Hex value");
            return ch < 58 ? ch - 48 : ch - 55;
        }

        /// <summary>
        /// mixed color Example : Green dark with green light
        /// </summary>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <param name="amount">0.5f</param>
        /// <returns></returns>
        public static int MixTwoColors(int color1, int color2, float amount)
        {
            byte alphaChannel = 24;
            byte redChannel = 16;
            byte greenChannel = 8;
            byte blueChannel = 0;

            float inverseAmount = 1.0f - amount;

            int a = (int)((color1 >> alphaChannel & 0xff) * amount + (color2 >> alphaChannel & 0xff) * inverseAmount) & 0xff;
            int r = (int)((color1 >> redChannel & 0xff) * amount + (color2 >> redChannel & 0xff) * inverseAmount) & 0xff;
            int g = (int)((color1 >> greenChannel & 0xff) * amount + (color2 >> greenChannel & 0xff) * inverseAmount) & 0xff;
            int b = (int)((color1 & 0xff) * amount + (color2 & 0xff) * inverseAmount) & 0xff;

            return a << alphaChannel | r << redChannel | g << greenChannel | b << blueChannel;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colors">new int[]{Color.LightGreen,Color.DarkGreen,}</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="showBorderWhite">Set 3 pixels width solid color border</param>
        /// <param name="showCornerRadius">Set CornerRadius 10</param>
        /// <returns></returns>
        public static (GradientDrawable, Bitmap) GetGradientDrawable(int[] colors, int width, int height, bool showBorderWhite = false, bool showCornerRadius = false)
        {
            try
            {
                // Initialize a new GradientDrawable
                var gd = new GradientDrawable();

                // Set the color array to draw gradient
                gd.SetColors(colors);

                // Set the GradientDrawable gradient type linear gradient
                gd.SetGradientType(GradientType.LinearGradient);

                // Set GradientDrawable shape is a rectangle
                gd.SetShape(ShapeType.Rectangle);

                // Set 3 pixels width solid color border
                if (showBorderWhite)
                    gd.SetStroke(3, Color.White);

                // Set GradientDrawable width and in pixels
                gd.SetSize(width, height); // Width 450 pixels and height 150 pixels

                // Set CornerRadius
                if (showCornerRadius)
                    gd.SetCornerRadius(10);

                var bitmap = BitmapUtil.ConvertToBitmap(gd, width, height);

                return (gd, bitmap);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return (null, null);
            }
        }
    }
}