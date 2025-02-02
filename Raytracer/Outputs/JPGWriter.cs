using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Raytracer.Outputs
{
    /// <summary>
    /// This class requires the download of the System.Drawing.Common package.
    /// You can do so, for example, in VS by using the NuGet Package Manager and running:
    /// Install-Package System.Drawing.Common
    /// This class contains both generating a JPG and converting PPM to JPG.
    /// </summary>
    internal class JPGWriter
    {
        private string name;

        public JPGWriter(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Prepares a bitmap of colors to save as JPG.
        /// </summary>
        /// <param name="image"></param>
        public void GenerateJPG(ImageCustom image)
        {
            // Bitmaps need to be properly disposed of!
            using (Bitmap bitmap = new Bitmap(image.width, image.height))
            {
                //generates a basic square to save
                for (int j = 0; j < image.height; j++)
                {
                    for (int i = 0; i < image.width; i++)
                    {
                        // Normalize pixel coordinates to [0,1] range
                        vectorsAndOthersforNow.Color pixelColor = new vectorsAndOthersforNow.Color((double)i / (image.width - 1), (double)j / (image.height - 1), 0);

                        // Convert from [0,1] to [0,255]
                        int r = (int)(255.999 * Math.Clamp(pixelColor.X, 0, 1));
                        int g = (int)(255.999 * Math.Clamp(pixelColor.Y, 0, 1));
                        int b = (int)(255.999 * Math.Clamp(pixelColor.Z, 0, 1));

                        // Set pixel in Bitmap
                        bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(r, g, b));
                    }
                }

                // Save as JPEG
                SaveJPG(bitmap);
            }
        }

        public void ConvertToJPG()
        {
            string ppmPath = name + ".ppm";
            using (StreamReader reader = new StreamReader(ppmPath))
            {
                string ppmHeader = reader.ReadLine();
                if (ppmHeader != "P3")
                {
                    throw new Exception("Invalid format");
                }

                // Read image dimensions form ppm
                string[] sizingData = reader.ReadLine().Split(' ');
                int width = int.Parse(sizingData[0]);
                int height = int.Parse(sizingData[1]);

                reader.ReadLine(); // Skip the max color value line

                Bitmap bitmap = new Bitmap(width, height);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        string[] pixelData = reader.ReadLine().Split(' ');
                        int r = int.Parse(pixelData[0]);
                        int g = int.Parse(pixelData[1]);
                        int b = int.Parse(pixelData[2]);
                        bitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                    }
                }

                SaveJPG(bitmap);
            }
        }

        private void SaveJPG(Bitmap bitmap)
        {
            // Find the correct encoder for the format we're using
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            if (jpgEncoder == null)
            {
                throw new Exception("Failed to get correct encoder!");
            }

            // Set the parameters for the encoder
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L); // 90 is high quality but compressed
            bitmap.Save(name + ".jpg", jpgEncoder, encoderParameters);
        }

        private ImageCodecInfo? GetEncoder(ImageFormat format)
        {
            // This gets all available image encoders
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                // If the requested (JPEG) format has been found, we use it
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

    }
}
