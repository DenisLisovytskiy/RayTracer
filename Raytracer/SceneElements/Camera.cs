using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raytracer.Outputs;
using Raytracer.TextInterfacing;
using Raytracer.Utilities;
using Raytracer.vectorsAndOthersforNow;


namespace Raytracer.SceneElements
{
    public class Camera
    {
        public double aspectRatio = 1.0;
        public int imageWidth = 100;
        public int samplesPerPixel = 10;
        public int maxDepth = 10;

        public double vfov = 90;  // Vertical view angle (field of view)
        public Point3 lookFrom = new Point3(0, 0, 0);   // Point camera is looking from
        public Point3 lookAt = new Point3(0, 0, -1);  // Point camera is looking at
        public Vec3 vup = new Vec3(0, 1, 0);     // Camera-relative "up" direction

        public double defocusAngle = 0;// Variation angle of rays through each pixel
        public double focusDistance = 10;// Distance from camera lookfrom point to plane of perfect focus

        private double pixelSamplesScale;

        private int imageHeight; // Rendered image height
        private Point3 cameraCenter; // Camera center
        private Point3 pixel00Location; // Location of pixel 0, 0
        private Vec3 pixelDeltaU; // Offset to pixel to the right
        private Vec3 pixelDeltaV; // Offset to pixel below
        private Vec3 u, v, w;

        private Vec3 defocusDiskU;// Defocus disk horizontal radius
        private Vec3 defocusDiskV;// Defocus disk vertical radius

        public Stopwatch? stopwatch;
        private static readonly Random rand = new Random();

        public void Render(IHittable world)
        {
            if (stopwatch != null)
                stopwatch.Stop();
            int userWidth = InputForms.GetWidth();
            if (stopwatch != null)
                stopwatch.Start();
            if (userWidth >= 1)
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
                        pixelColor += RayColor(r, maxDepth, world);
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
                catch (Exception e)
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
            cameraCenter = lookFrom;

            var theta = UtilityFunctions.DegreesToRadians(vfov);
            var h = Math.Tan(theta / 2);
            var viewportHeight = 2 * h * focusDistance;
            double viewportWidth = viewportHeight * ((double)imageWidth / imageHeight);

            // Calculate the u,v,w unit basis vectors for the camera coordinate frame.
            w = Vec3.UnitVector(lookFrom - lookAt);
            u = Vec3.UnitVector(Vec3.Cross(vup, w));
            v = Vec3.Cross(w, u);

            // Calculate the vectors across the horizontal and down the vertical viewport edges.
            Vec3 viewportU = viewportWidth * u; // Vector across viewport horizontal edge
            Vec3 viewportV = viewportHeight * -v; // Vector down viewport vertical edge

            // Calculate the horizontal and vertical delta vectors from pixel to pixel
            pixelDeltaU = viewportU / imageWidth;
            pixelDeltaV = viewportV / imageHeight;

            // Upper Left Pixel Calculation
            Vec3 viewportUpperLeft = cameraCenter - (focusDistance * w) - viewportU / 2 - viewportV / 2;
            pixel00Location = (viewportUpperLeft + 0.5 * (pixelDeltaU + pixelDeltaV));

            // Calculate the camera defocus disk basis vectors.
            double defocus_radius = focusDistance * Math.Tan(UtilityFunctions.DegreesToRadians(defocusAngle / 2));
            defocusDiskU = u * defocus_radius;
            defocusDiskV = v * defocus_radius;
        }

        private Ray GetRay(int i, int j)
        {
            // Construct a camera ray originating from the camera center and directed at a randomly sampled point around pixel (i, j)
            Vec3 offset = SampleSquare();
            Vec3 pixelSample = pixel00Location
                              + ((i + offset.X) * pixelDeltaU)
                              + ((j + offset.Y) * pixelDeltaV);
            Vec3 rayOrigin = (defocusAngle <= 0) ? cameraCenter : DefocusDiskSample();
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

        private Point3 DefocusDiskSample()
        {
            // Returns a random point in the camera defocus disk.
            var p = Vec3.RandomInUnitDisk();
            return cameraCenter + (p.X * defocusDiskU) + (p.Y * defocusDiskV);
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
                Ray scattered;
                ColorV2 attenuation;
                if (record.material.Scatter(ray, record, out attenuation, out scattered))
                {
                    return attenuation * RayColor(scattered, depth - 1, world);
                }
                return new ColorV2(0, 0, 0);
                //Vec3 direction = Vec3.RandomOnHemisphere(record.Normal); //before 9.4
                //Vec3 direction = record.Normal + Vec3.RandomUnitVector();
                //return 0.9 * RayColor(new Ray(record.P, direction),depth-1, world);

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
