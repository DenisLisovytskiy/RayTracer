using Raytracer.A_External;
using Raytracer.vectorsAndOthersforNow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.SceneElements
{
    public abstract class Texture
    {
        public abstract ColorV2 Value(double u, double v, Point3 p);
    }

    public class SolidColor : Texture
    {
        private readonly ColorV2 albedo;

        public SolidColor(ColorV2 albedo)
        {
            this.albedo = albedo;
        }

        public SolidColor(double red, double green, double blue) : this(new ColorV2(red, green, blue))
        {
        }

        public override ColorV2 Value(double u, double v, Point3 p)
        {
            return albedo;
        }
    }

    public class CheckerTexture : Texture
    {
        private double invScale;
        private Texture even;
        private Texture odd;

        public CheckerTexture(double scale, Texture even, Texture odd)
        {
            invScale = 1.0 / scale;
            this.even = even;
            this.odd = odd;
        }

        public CheckerTexture(double scale, ColorV2 c1, ColorV2 c2)
            : this(scale, new SolidColor(c1), new SolidColor(c2)) { }

        public override ColorV2 Value(double u, double v, Point3 p)
        {
            int xInteger = (int)Math.Floor(invScale * p.X);
            int yInteger = (int)Math.Floor(invScale * p.Y);
            int zInteger = (int)Math.Floor(invScale * p.Z);

            bool isEven = (xInteger + yInteger + zInteger) % 2 == 0;
            return isEven ? even.Value(u, v, p) : odd.Value(u, v, p);
        }
    }

    public class ImageTexture : Texture
    {
        private readonly RTWImage image;

        public ImageTexture(string filename)
        {
            image = new RTWImage(filename);
        }

        public override ColorV2 Value(double u, double v, Point3 p)
        {
            // If we have no texture data, then return solid cyan as a debugging aid.
            if (image.Height <= 0)
                return new  ColorV2(0,1,1);

            // Clamp input texture coordinates to [0,1] x [1,0]
            u = new Interval(0,1).Clamp(u);
            v = 1.0 - new Interval(0, 1).Clamp(v);

            var i = (int)(u * image.Width);
            var j = (int)(u * image.Height);
            var pixel = image.PixelData(i, j);

            double colorScale = 1.0 / 255.0; 

            return new ColorV2(
                (pixel[0] * colorScale),
                (pixel[1] * colorScale),
                (pixel[2] * colorScale)
            );
        }
    }

}
