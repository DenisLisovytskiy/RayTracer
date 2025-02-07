using System;
using System.Collections.Generic;
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

        private int imageHeight; // Rendered image height
        private Point3 cameraCenter; // Camera center
        private Point3 pixel00Location; // Location of pixel 0, 0
        private Vec3 pixelDeltaU; // Offset to pixel to the right
        private Vec3 pixelDeltaV; // Offset to pixel below



        public void Render(IHittable world)
        {

            int userWidth = InputForms.GetWidth();
            if(userWidth >=1)
            {
                this.imageWidth = userWidth;
            }
            
            Initialize();
            
            string name = InputForms.GetName();
            StreamWriter streamWriter = new StreamWriter(name + ".ppm"); // c# neccesary to save the file

            streamWriter.WriteLine("P3");
            streamWriter.WriteLine($"{imageWidth} {imageHeight}");
            streamWriter.WriteLine("255");

            //loops
            for (int j = 0; j < imageHeight; j++)
            {
                for (int i = 0; i < imageWidth; i++)
                {
                    Vec3 pixelCenter = pixel00Location + (i * pixelDeltaU) + (j * pixelDeltaV);
                    Vec3 rayDirection = pixelCenter - cameraCenter;
                    Ray _ray = new(cameraCenter, rayDirection);


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

        public static ColorV2 RayColor(Ray ray, IHittable world)
        {
            HitRecord record;
            if (world.Hit(ray, new Interval(0, double.PositiveInfinity), out record))
            {
                return 0.5 * (record.Normal + new ColorV2(1, 1, 1));
            }

            Vec3 unitDirection = Vec3.UnitVector(ray.Direction);
            double a = 0.5 * (unitDirection.Y + 1.0);

            return (1.0 - a) * new ColorV2(1.0, 1.0, 1.0) + a * new ColorV2(0.5, 0.7, 1.0);
        }
    }
}
