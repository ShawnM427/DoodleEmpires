using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WinFormsTools
{
    /// <summary>
    /// A class for some basic math stuffs
    /// </summary>
    public static class Math2
    {
        /// <summary>
        /// Performs linear interpolation between two values
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <param name="percent">The percent, between 0 and 1, to lerp</param>
        /// <returns>A linear interpolation of min-max</returns>
        public static float Lerp(float min, float max, float percent)
        {

            float range = max - min;
            return min + (range * percent);
        }
        
        /// <summary>
        /// Performs linear interpolation between two values
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <param name="percent">The percent, between 0 and 1, to lerp</param>
        /// <returns>A linear interpolation of min-max</returns>
        public static double Lerp(double min, double max, double percent)
        {

            double range = max - min;
            return min + (range * percent);
        }
        
        /// <summary>
        /// Performs linear interpolation between two values
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <param name="percent">The percent, between 0 and 1, to lerp</param>
        /// <returns>A linear interpolation of min-max</returns>
        public static int Lerp(int min, int max, double percent)
        {

            int range = max - min;
            return (int)Math.Round(min + (range * percent));
        }

        /// <summary>
        /// Performs linear interpolation between 2 colors
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <param name="percent">The percent, between 0 and 1, to lerp</param>
        /// <returns>A linear interpolation of min-max</returns>
        public static Color Lerp(Color min, Color max, float percent)
        {
            float rangeR = max.R - min.R;
            float rangeG = max.G - min.G;
            float rangeB = max.B - min.B;
            float rangeA = max.A - min.A;
            return Color.FromArgb(min.A + (int)(rangeA * percent), min.R + (int)(rangeR * percent),
                min.G + (int)(rangeG * percent), min.B + (int)(rangeB * percent));
        }
    }
}
