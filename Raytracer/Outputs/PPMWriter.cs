using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Raytracer.TextInterfacing;
using Raytracer.vectorsAndOthersforNow;

namespace Raytracer.Outputs
{
    class ImageCustom
    {
        public int height { get; set; }
        public int width { get; set; }

        public ImageCustom(int w = 1920, int h = 1080)
        {
            this.width = w;
            this.height = h;
        }
    }
    class PPMWriter : IDisposable
    {
        private StreamWriter writer;
        private string name;

        public PPMWriter(string name)
        {
            this.name = name + ".ppm";
            writer = new StreamWriter(this.name);
        }

        public void PutHeader(ImageCustom image)
        {
            writer.WriteLine("P3");
            writer.WriteLine($"{image.width} {image.height}");
            writer.WriteLine("255");
        }

        public void PutPixels(ImageCustom image)
        {
            for (int i = 0; i < image.height; i++)
            {
                for (int j = 0; j < image.width; j++)
                {
                    // Normalize pixel coordinates to [0,1] range
                   Color pixelColor = new Color((double)j / (image.width - 1), (double)i / (image.height - 1), 0);

                    // Output the color in [0,255] format
                    pixelColor.WriteColor(writer, pixelColor);

                }
            }
            ProgressReporting.DoneMessage();
        }

        public void GenerateImage(ImageCustom image)
        {
            PutHeader(image);
            PutPixels(image);
            //flushing here is critical,
            //otherwise various bugs appear
            writer.Flush();
        }

        public void Dispose()
        {
            writer.Close();
        }

    }
}