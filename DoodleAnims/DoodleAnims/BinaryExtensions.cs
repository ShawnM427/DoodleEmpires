using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace DoodleAnims
{
    /// <summary>
    /// Some basic extensions for binary data processing
    /// </summary>
    public static class BinaryExtensions
    {
        /// <summary>
        /// Reads an image from a data stream
        /// </summary>
        /// <param name="r">The binary reader to use</param>
        /// <returns>An image loaded from the stream</returns>
        public static Image ReadImage(this BinaryReader r)
        {
            int width = r.ReadInt32();
            int height = r.ReadInt32();

            Bitmap i = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    i.SetPixel(x, y, Color.FromArgb(r.ReadByte(), r.ReadByte(), r.ReadByte(), r.ReadByte()));
                }
            }

            return i;
        }

        /// <summary>
        /// Writes an image to a data stream
        /// </summary>
        /// <param name="w">The binary writer to use</param>
        /// <param name="i">The image to write</param>
        public static void WriteImage(this BinaryWriter w, Bitmap i)
        {
            w.Write(i.Width);
            w.Write(i.Height);

            Color c;
            for (int y = 0; y < i.Height; y++)
            {
                for (int x = 0; x < i.Width; x++)
                {
                    c = i.GetPixel(x, y);
                    w.Write(c.A);
                    w.Write(c.R);
                    w.Write(c.G);
                    w.Write(c.B);
                }
            }
        }
    }
}
