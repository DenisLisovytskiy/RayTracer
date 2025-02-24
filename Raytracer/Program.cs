using static System.Net.Mime.MediaTypeNames;
using Raytracer.Outputs;
using System.Xml.Linq;
using Raytracer.TextInterfacing;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Reflection;
using Raytracer.vectorsAndOthersforNow;
using Raytracer.SceneElements;
using System.Diagnostics;
using Raytracer.Utilities;
using Raytracer.Materials;
using Raytracer.BVH;

namespace Raytracer
{
    internal class Program
    {

        static void CheckeredSceneBook1()
        {
            Stopwatch _stopwatch = Stopwatch.StartNew();
            //World            
            HittableList world = new HittableList();

            var checker = new CheckerTexture(0.32, new ColorV2(1.0, 0.713, 0.756), new ColorV2(1.0, 0.078, 0.576));
            world.Add(new Sphere(new Point3(0, -1000, 0), 1000, new Lambertian(checker)));

            // before textures
            //var groundMaterial = new Lambertian(new ColorV2(0.5, 0.5, 0.5));
            //world.Add(new Sphere(new Point3(0, -1000, 0), 1000, groundMaterial));

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
                maxDepth = 10, // used to determine how far recursion can go in RayColor

                vfov = 20, // field of view, basicallly zooming in and out 

                lookFrom = new Point3(13, 2, 3),
                lookAt = new Point3(0, 0, 0),
                vup = new Vec3(0, 1, 0),

                defocusAngle = 0.6,
                focusDistance = 10.0
            };

            camera.Render(world);
        }

        static void CheckeredSceneBook1_withEarth()
        {
            Stopwatch _stopwatch = Stopwatch.StartNew();
            //World            
            HittableList world = new HittableList();

            var checker = new CheckerTexture(0.32, new ColorV2(1.0, 0.713, 0.756), new ColorV2(1.0, 0.078, 0.576));
            world.Add(new Sphere(new Point3(0, -1000, 0), 1000, new Lambertian(checker)));

            // before textures
            //var groundMaterial = new Lambertian(new ColorV2(0.5, 0.5, 0.5));
            //world.Add(new Sphere(new Point3(0, -1000, 0), 1000, groundMaterial));

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

            var earthTexture = new ImageTexture("earthmap.jpg");
            var earthSurface = new Lambertian(earthTexture);
            //var material2 = new Lambertian(new ColorV2(0.4, 0.2, 0.1));
            world.Add(new Sphere(new Point3(-4, 1, 0), 1.0, earthSurface));

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
                maxDepth = 10, // used to determine how far recursion can go in RayColor

                vfov = 20, // field of view, basicallly zooming in and out 

                lookFrom = new Point3(13, 2, 3),
                lookAt = new Point3(0, 0, 0),
                vup = new Vec3(0, 1, 0),

                defocusAngle = 0.6,
                focusDistance = 10.0
            };

            camera.Render(world);
        }

        static void CheckeredSpheres()
        {
            Stopwatch _stopwatch = Stopwatch.StartNew();
            //World            
            HittableList world = new HittableList();

            var checker = new CheckerTexture(0.32, new ColorV2(1.0, 0.713, 0.756), new ColorV2(1.0, 0.078, 0.576));
            world.Add(new Sphere(new Point3(0, -10, 0), 10, new Lambertian(checker)));
            world.Add(new Sphere(new Point3(0, 10, 0), 10, new Lambertian(checker)));



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
                maxDepth = 50, // used to determine how far recursion can go in RayColor

                vfov = 20, // field of view, basicallly zooming in and out 

                lookFrom = new Point3(13, 2, 3),
                lookAt = new Point3(0, 0, 0),
                vup = new Vec3(0, 1, 0),

                defocusAngle = 0,
                //focusDistance = 10.0
            };

            camera.Render(world);
        }

        static void Earth()
        {
            Stopwatch _stopwatch = Stopwatch.StartNew();
            //World            
            HittableList world = new HittableList();
            var earthTexture = new ImageTexture("earthmap.jpg");
            var earthSurface = new Lambertian(earthTexture);
            var globe = new Sphere(new Point3(0, 0, 0), 2, earthSurface);

            world.Add(globe);

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
                maxDepth = 50, // used to determine how far recursion can go in RayColor

                vfov = 20, // field of view, basicallly zooming in and out 

                lookFrom = new Point3(0, 0, 12),
                lookAt = new Point3(0, 0, 0),
                vup = new Vec3(0, 1, 0),

                defocusAngle = 0,
                //focusDistance = 10.0
            };
            camera.Render(world);
        }
        static void PerlinSpheres()
        {
            Stopwatch _stopwatch = Stopwatch.StartNew();
            //World            
            HittableList world = new HittableList();

            var perlinTexture = new NoiseTexture(4);
            world.Add(new Sphere(new Point3(0, -1000, 0), 1000, new Lambertian(perlinTexture)));
            world.Add(new Sphere(new Point3(0, 2, 0), 2, new Lambertian(perlinTexture)));


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
                maxDepth = 50, // used to determine how far recursion can go in RayColor

                vfov = 20, // field of view, basicallly zooming in and out 

                lookFrom = new Point3(13, 2, 3),
                lookAt = new Point3(0, 0, 0),
                vup = new Vec3(0, 1, 0),

                defocusAngle = 0,
                //focusDistance = 10.0
            };
            camera.Render(world);

        }
        static void Main(string[] args)
        {
            switch (5)
            {
                case 1: CheckeredSceneBook1(); break;
                case 2: CheckeredSceneBook1_withEarth(); break;
                case 3: CheckeredSpheres(); break;
                case 4: Earth(); break;
                case 5: PerlinSpheres(); break;
            }
        }
    }
}
