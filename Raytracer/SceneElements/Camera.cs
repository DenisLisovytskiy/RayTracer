using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raytracer.Outputs;
using Raytracer.TextInterfacing;
using Raytracer.vectorsAndOthersforNow;


namespace Raytracer.SceneElements
{
    public class Camera
    {
        public double aspectRatio = 1.0;
        public int imageWidth = 100;
        public int samplesPerPixel = 10;
        public int maxDepth = 10;

        private double pixelSamplesScale;

        private int imageHeight; // Rendered image height
        private Point3 cameraCenter; // Camera center
        private Point3 pixel00Location; // Location of pixel 0, 0
        private Vec3 pixelDeltaU; // Offset to pixel to the right
        private Vec3 pixelDeltaV; // Offset to pixel below

        public Stopwatch? stopwatch;
        private static readonly Random rand = new Random();

        public void Render(IHittable world)
        {
            if (stopwatch != null)
                stopwatch.Stop();
            int userWidth = InputForms.GetWidth();
            if (stopwatch != null)
                stopwatch.Start();
            if (userWidth >=1)
            {
                this.imageWidth = userWidth;
            }
            
            Initialize();
            if (stopwatch != null)
                stopwatch.Stop();
            string name = InputForms.GetName();
            if (stopwatch != null)
                stopwatch.Start();
            StreamWriter streamWriter = new StreamWriter(name + ".ppm"); // c# neccesary to save the file

            streamWriter.WriteLine("P3");
            streamWriter.WriteLine($"{imageWidth} {imageHeight}");
            streamWriter.WriteLine("255");

            //loops
            for (int j = 0; j < imageHeight; j++)
            {
                for (int i = 0; i < imageWidth; i++)
                {
                    /*
                    Vec3 pixelCenter = pixel00Location + (i * pixelDeltaU) + (j * pixelDeltaV);
                    Vec3 rayDirection = pixelCenter - cameraCenter;
                    Ray _ray = new(cameraCenter, rayDirection);


                    // Normalize pixel coordinates to [0,1] range
                    ColorV2 pixelColor = RayColor(_ray, world);

                     */
                    ColorV2 pixelColor = new ColorV2(0, 0, 0);

                    // Multi-sampling per pixel
                    Ray r = new Ray();
                    for (int sample = 0; sample < samplesPerPixel; sample++)
                    {
                        r = GetRay(i, j);
                        pixelColor += RayColor(r,maxDepth, world);
                    }

                    // Output the color in [0,255] format
                    pixelColor.WriteColor(streamWriter, pixelColor * pixelSamplesScale);

                }
            }

            if (stopwatch != null)
            {
                stopwatch.Stop();
                ProgressReporting.DoneMessage($"{stopwatch.ElapsedMilliseconds}");
            }
            else
            {
                ProgressReporting.DoneMessage("No data");
            }

            streamWriter.Flush();
            streamWriter.Close();
            if (InputForms.WantJpeg())
            {
                //JPGWriter jpegWriter = new(name);
                try
                {
                    //jpegWriter.ConvertToJPG();
                    JPGWriter.ConvertToJPG(name);
                }
                catch(Exception e)
                {
                    ProgressReporting.ExceptionMessage(e);
                }
            }
        }

        private void Initialize()
        {
            imageHeight = (int)(imageWidth / aspectRatio);
            imageHeight = Math.Max(1, imageHeight); // Prevent zero or negative values
            pixelSamplesScale = 1.0 / samplesPerPixel;

            // Camera Parameters
            double focalLength = 1.0;
            double viewportHeight = 2.0;
            double viewportWidth = viewportHeight * ((double)imageWidth / imageHeight);
            cameraCenter = new Point3(0, 0, 0);

            // Viewport Edge Vectors
            Vec3 viewportU = new Vec3(viewportWidth, 0, 0);
            Vec3 viewportV = new Vec3(0, -viewportHeight, 0);

            // Pixel Delta Vectors
            pixelDeltaU = viewportU / imageWidth;
            pixelDeltaV = viewportV / imageHeight;

            // Upper Left Pixel Calculation
            Point3 viewportUpperLeft = (cameraCenter
                                         - (new Vec3(0, 0, focalLength))
                                         - viewportU / 2
                                         - viewportV / 2);
            pixel00Location = (viewportUpperLeft + 0.5 * (pixelDeltaU + pixelDeltaV));
        }

        private Ray GetRay(int i, int j)
        {
            // Construct a camera ray originating from the camera center and directed at a randomly sampled point around pixel (i, j)
            Vec3 offset = SampleSquare();
            Vec3 pixelSample = pixel00Location
                              + ((i + offset.X) * pixelDeltaU)
                              + ((j + offset.Y) * pixelDeltaV);
            Vec3 rayOrigin = cameraCenter;
            Vec3 rayDirection = pixelSample - rayOrigin;
            return new Ray(rayOrigin, rayDirection);
        }

        // Returns a vector to a random point in the [-0.5, -0.5] - [+0.5, +0.5] unit square.
        private Vec3 SampleSquare()
        {
            double x = (rand.NextDouble() - 0.5);
            double y = (rand.NextDouble() - 0.5);
            return new Vec3(x, y, 0);
        }



        //public static ColorV2 RayColor(Ray ray, IHittable world)
        //{
        //    HitRecord record;
        //    if (world.Hit(ray, new Interval(0, double.PositiveInfinity), out record))
        //    {
        //        return 0.5 * (record.Normal + new ColorV2(1, 1, 1));
        //    }

        //    Vec3 unitDirection = Vec3.UnitVector(ray.Direction);
        //    double a = 0.5 * (unitDirection.Y + 1.0);

        //    return (1.0 - a) * new ColorV2(1.0, 1.0, 1.0) + a * new ColorV2(0.5, 0.7, 1.0);
        //}

        public static ColorV2 RayColor(Ray ray, int depth, IHittable world)
        {
            // If we've exceeded the ray bounce limit, no more light is gathered.
            if (depth <= 0)
                return new ColorV2(0, 0, 0);

            HitRecord record = default; // Structs need explicit initialization

            if (world.Hit(ray, new Interval(0.001, double.PositiveInfinity), ref record))
            {
                //Vec3 direction = Vec3.RandomOnHemisphere(record.Normal); //before 9.4
                Vec3 direction = record.Normal + Vec3.RandomUnitVector();
                return 0.9 * RayColor(new Ray(record.P, direction),depth-1, world);
                
                // !!ATTENTION!!
                // Changing variable above (from 0 to 1 ) determines 
                // brightness

                //return 0.5 * (record.Normal + new ColorV2(1, 1, 1));
            }

            Vec3 unitDirection = Vec3.UnitVector(ray.Direction);
            double a = 0.5 * (unitDirection.Y + 1.0);

            return (1.0 - a) * new ColorV2(1.0, 1.0, 1.0) + a * new ColorV2(0.5, 0.7, 1.0);
        }


    }
}
