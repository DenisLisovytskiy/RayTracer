//#define UpTo3
#define From4
using static System.Net.Mime.MediaTypeNames;
using Raytracer.Outputs;
using System.Xml.Linq;
using Raytracer.TextInterfacing;
using Raytracer.vectorsAndOthersforNow;
using System.Drawing;
using System.Runtime.CompilerServices;


namespace Raytracer
{
    internal class Program
    {
        public static MyColor RayColor(Ray _ray)
        {
            Vec3 unitDirection = Vec3.UnitVector(_ray.Direction);
            double a = 0.5 * (unitDirection.Y + 1.0);

            // Convert Vec3 result explicitly into Color
            Vec3 result = (1.0 - a) * new MyColor(1.0, 1.0, 1.0) + a * new MyColor(0.5, 0.7, 1.0);

            return new MyColor(result.X, result.Y, result.Z); // Explicitly create a Color
        }

        static void Main(string[] args)
        {
#if UpTo3
            var (width, height) = InputForms.GetDimensions();
            ImageCustom image = new ImageCustom(width, height);

            string name = InputForms.GetName();
            using (PPMWriter writer = new(name))
            {
                writer.GenerateImage(image);
            }
            if (InputForms.WantJpeg())
            {
                JPGWriter jpegWriter = new(name);
                jpegWriter.ConvertToJPG();
            }
#endif
#if From4
            string name = InputForms.GetName();
            StreamWriter streamWriter = new StreamWriter(name + ".ppm"); // c# neccesary to save the file


            double aspectRatio = 16.0 / 9.0;
            int imageWidth = 400;
            int imageHeight = (int)(imageWidth / aspectRatio);
            imageHeight = (imageHeight < 1) ? 1 : imageHeight; // Ensure at least 1 pixel height

            // Camera Parameters
            double focalLength = 1.0;
            double viewportHeight = 2.0;
            double viewportWidth = viewportHeight * ((double)imageWidth / imageHeight);
            Point3 cameraCenter = new Point3(0, 0, 0);

            // Viewport Edge Vectors
            Vec3 viewportU = new Vec3(viewportWidth, 0, 0);
            Vec3 viewportV = new Vec3(0, -viewportHeight, 0);

            // Pixel Delta Vectors
            Vec3 pixelDeltaU = viewportU / imageWidth;
            Vec3 pixelDeltaV = viewportV / imageHeight;

            // Upper Left Pixel Calculation
            
            Point3 viewportUpperLeft = (cameraCenter
                                         - (new Vec3(0, 0, focalLength))
                                         - viewportU / 2
                                         - viewportV / 2);


            Point3 pixel00Loc = (viewportUpperLeft + 0.5 * (pixelDeltaU + pixelDeltaV));


            streamWriter.WriteLine("P3");
            streamWriter.WriteLine($"{imageWidth} {imageHeight}");
            streamWriter.WriteLine("255");

            for (int j = 0; j < imageHeight; j++)
            {
                for (int i = 0; i < imageWidth; i++)
                {
                    var pixelCenter = pixel00Loc + (i * pixelDeltaU) + (j * pixelDeltaV);
                    var rayDirection = pixelCenter - cameraCenter;
                    Ray _ray = new Ray(cameraCenter, rayDirection);


                    // Normalize pixel coordinates to [0,1] range
                    MyColor pixelColor = RayColor(_ray);

                    // Output the color in [0,255] format
                    pixelColor.WriteColor(streamWriter, pixelColor);

                }
            }
            ProgressReporting.DoneMessage();

            streamWriter.Flush();
            streamWriter.Close();
            if (InputForms.WantJpeg())
            {
                JPGWriter jpegWriter = new(name);
                jpegWriter.ConvertToJPG();
            }
#endif

        }
    }
}
