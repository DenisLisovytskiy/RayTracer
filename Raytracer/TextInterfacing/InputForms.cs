using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raytracer.Outputs;

namespace Raytracer.Interfacing
{
    internal class InputForms
    {
        public static Tuple<int, int> GetDimensions()
        {
            int w, h;
            Console.WriteLine("Please input the dimensions of the image you'd like to render: [width] [height]");
            var input = Console.ReadLine();
            string[] parts = input.Split(' ');

            if (2 == parts.Length)
            {
                w = int.Parse(parts[0]);
                h = int.Parse(parts[1]);
            }
            else
            {
                w = 1080;
                h = 1920;
            }
            return Tuple.Create(w, h);
        }

        public static string GetName()
        {
            Console.WriteLine("Generating image...");
            Console.WriteLine("Please specify output file name:");
            string? name = Console.ReadLine();
            if (name == null)
            {
                name = "default";
            }
            return name;
        }
    }
}
