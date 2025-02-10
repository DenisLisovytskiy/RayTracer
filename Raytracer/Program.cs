using static System.Net.Mime.MediaTypeNames;
using Raytracer.Outputs;
using System.Xml.Linq;
using Raytracer.TextInterfacing;
using Raytracer.vectorsAndOthersforNow;
using System.Drawing;
using System.Runtime.CompilerServices;
using Raytracer.SceneElements;
using System.Reflection;
using System.Diagnostics;
using Raytracer.Utilities;
using Raytracer.Materials;
using Raytracer.BVH;

namespace Raytracer
{
    internal class Program
    {

        static void Main(string[] args)
        {
            

            Stopwatch _stopwatch = Stopwatch.StartNew();
            //World            
            HittableList world = new HittableList();

            var groundMaterial = new Lambertian(new ColorV2(0.5, 0.5, 0.5));
            world.Add(new Sphere(new Point3(0, -1000, 0), 1000, groundMaterial));

            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    double chooseMat = UtilityFunctions.RandomDouble();
                    Point3 center = new Point3(a + 0.9 * UtilityFunctions.RandomDouble(), 0.2, b + 0.9 * UtilityFunctions.RandomDouble());

                    if ((center - new Point3(4, 0.2, 0)).Length() > 0.9)
                    {
                        IMaterial sphereMaterial;

                        if (chooseMat < 0.8)
                        {
                            // diffuse
                            ColorV2 albedo = ColorV2.Random() * ColorV2.Random();
                            sphereMaterial = new Lambertian(albedo);
                            world.Add(new Sphere(center, 0.2, sphereMaterial));
                        }
                        else if (chooseMat < 0.95)
                        {
                            // metal
                            ColorV2 albedo = ColorV2.Random(0.5, 1.0);
                            double fuzz = UtilityFunctions.RandomDouble(0, 0.5);
                            sphereMaterial = new Metal(albedo, fuzz);
                            world.Add(new Sphere(center, 0.2, sphereMaterial));
                        }
                        else
                        {
                            // glass
                            sphereMaterial = new Dielectric(1.5);
                            world.Add(new Sphere(center, 0.2, sphereMaterial));
                        }
                    }
                }
            }

            var material1 = new Dielectric(1.5);
            world.Add(new Sphere(new Point3(0, 1, 0), 1.0, material1));

            var material2 = new Lambertian(new ColorV2(0.4, 0.2, 0.1));
            world.Add(new Sphere(new Point3(-4, 1, 0), 1.0, material2));

            var material3 = new Metal(new ColorV2(0.7, 0.6, 0.5), 0.0);
            world.Add(new Sphere(new Point3(4, 1, 0), 1.0, material3));

            BVHNode bvhWorld = new BVHNode(world);
            world.Clear();
            world.Add(bvhWorld);

            Camera camera = new()
            {
                aspectRatio = 16.0 / 9.0,
                imageWidth = 400,
                stopwatch = _stopwatch,
                samplesPerPixel = 100, // increase by one -> one more operation for every pixel
                                      // (even more, beacause it is a complex computation)
                                      // basically "how strong you want your antilaiasing" 
                maxDepth = 15, // used to determine how far recursion can go in RayColor

                vfov = 20, // field of view, basicallly zooming in and out 

                lookFrom = new Point3(13, 2, 3),
                lookAt = new Point3(0, 0, 0),
                vup = new Vec3(0, 1, 0),

                defocusAngle = 0.6,
                focusDistance = 10.0
            };

            camera.Render(world);
        }
    }
}
