using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectToShader
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourceFile = null;
            string destinationFile = null;

            if (args.Length < 2)
            {
                Console.WriteLine("Argument count must be greater than 2!");
                ShowInfo();
                goto Close;
            }

            sourceFile = args[0];
            destinationFile = args[1];

            if (!File.Exists(sourceFile))
            {
                Console.WriteLine("Cannot find source file!");
                goto Close;
            }
            
            //try
            //{
            EffectParameterCollection parameters = EffectReader.ReadEffect(new BinaryReader(File.OpenRead(sourceFile)));
            //}
            //catch(Exception e)
            //{
            //    Console.WriteLine("Cannot parse source file! May be wrong MGFX version.");
            //    goto Close;
            //}

          Close:
            Console.Write("\nPress any key to continue... ");

          Console.ReadKey();
        }

        static void ShowInfo()
        {
            Console.WriteLine("Converts a mgfx shader into a class file that can acess it's properties");
            Console.WriteLine("Usage: EffectToShader <sourceFile> <destFile>");
            Console.WriteLine("Paramters:");
            Console.WriteLine("\t <sourceFile> The source file for the shader");
            Console.WriteLine("\t <destFile> The destination file for the generated code");
        }
    }
}
