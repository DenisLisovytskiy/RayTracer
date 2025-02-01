using Raytracer.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.vectorsAndOthersforNow
{
    public class Color : Vec3
    {
        public Color(double r, double g, double b) : base(r, g, b) { }

        public void WriteColor(TextWriter output, Color pixelColor)
        {
            // Extract RGB values
            double r = pixelColor.X;
            double g = pixelColor.Y;
            double b = pixelColor.Z;

            // Convert from [0,1] to [0,255] and ensure valid range
            int rByte = (int)(255.999 * Math.Clamp(r, 0, 1));
            int gByte = (int)(255.999 * Math.Clamp(g, 0, 1));
            int bByte = (int)(255.999 * Math.Clamp(b, 0, 1));

            output.WriteLine($"{rByte} {gByte} {bByte}");
        }

    }
}
