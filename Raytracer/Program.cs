//#define UpTo3
#define From4
using static System.Net.Mime.MediaTypeNames;
using Raytracer.Outputs;
using System.Xml.Linq;
using Raytracer.TextInterfacing;
using Raytracer.vectorsAndOthersforNow;
using System.Drawing;
using System.Runtime.CompilerServices;
using Raytracer.SceneElements;
using System.Reflection;

//math constants to be moved later

namespace Raytracer
{
    internal class Program
    {
        //no longer in use
        ////using vector calulus calculate whether a ray hits a sphere
        ////by hits we accept 1 or 2 hits (going through the sphere or being tangent to it)
        //static double HitSphere(Point3 center, double radius, Ray ray)
        //{
        //    Vec3 oc = center - ray.Origin;
        //    var a = ray.Direction.LengthSquared();
        //    var h = Vec3.Dot(ray.Direction, oc);
        //    var c = oc.LengthSquared() - radius* radius;
        //    var delta = h * h -  a * c;
        //    //if delta >= 0 means we have a solution of the equation so we get a hit
        //    if (delta < 0)
        //    {
        //        return -1.0;
        //    }
        //    else
        //    {
        //        return (h - Math.Sqrt(delta)) / a;
        //    }
        //}

        double degreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        public static ColorV2 RayColor(Ray _ray, IHittable world)
        {
            //used with the hit sphere function and with changed function to:
            //public static ColorV2 RayColor(Ray _ray)
            //var t = HitSphere(new Point3(0, 0, -1), 0.5, _ray);
            //if(t > 0.0)
            //{
            //    Vec3 N = Vec3.UnitVector(_ray.At(t) - new Vec3(0,0,-1));

            //    return 0.5 * (new ColorV2(N.X + 1, N.Y + 1, N.Z + 1));
            //}


            //new implementation with IHittable objects

            HitRecord record;
            if(world.Hit(_ray,new Interval(0, double.PositiveInfinity), out record))
            {
                return 0.5 * (record.Normal + new ColorV2(1, 1, 1));
            }

            Vec3 unitDirection = Vec3.UnitVector(_ray.Direction);
            double a = 0.5 * (unitDirection.Y + 1.0);

            // Convert Vec3 result explicitly into Color
            Vec3 result = (1.0 - a) * new ColorV2(1.0, 1.0, 1.0) + a * new ColorV2(0.5, 0.7, 1.0);

            return new ColorV2(result.X, result.Y, result.Z); // Explicitly create a Color
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

            //World
            HittableList world = new(new Sphere(new Point3(0, 0, -1), 0.5));
            world.Add(new Sphere(new Point3(0, -100.5, -1), 100));

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
                    ColorV2 pixelColor = RayColor(_ray, world);

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
