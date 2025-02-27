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
using System.Runtime.InteropServices;
using Raytracer.SceneElements.Primitives2D;

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
                background = new ColorV2(0.70, 0.80, 1.00),

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
                background = new ColorV2(0.70, 0.80, 1.00),

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
                background = new ColorV2(0.70, 0.80, 1.00),

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
                background = new ColorV2(0.70, 0.80, 1.00),

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
                background = new ColorV2(0.70, 0.80, 1.00),

                vfov = 20, // field of view, basicallly zooming in and out 

                lookFrom = new Point3(13, 2, 3),
                lookAt = new Point3(0, 0, 0),
                vup = new Vec3(0, 1, 0),

                defocusAngle = 0,
                //focusDistance = 10.0
            };
            camera.Render(world);

        }

        static void Quads()
        {
            Stopwatch _stopwatch = Stopwatch.StartNew();
            //World            
            HittableList world = new HittableList();

            //materials 
            var left = new Lambertian(new ColorV2(1.0, 0.0, 0.0));
            var back = new Lambertian(new ColorV2(0.0, 1.0, 0.0));
            var right = new Lambertian(new ColorV2(0.0, 0.0, 1.0));
            var up = new Lambertian(new ColorV2(1.0, 1.0, 0.0));
            var down = new Lambertian(new ColorV2(1.0, 0.0, 1.0));

            //quads
            world.Add(new Quad(new Point3(-3, -2, 5), new Vec3(0, 0, -4), new Vec3(0, 4, 0), left));
            world.Add(new Quad(new Point3(-2, -2, 0), new Vec3(4, 0, 0), new Vec3(0, 4, 0), back));
            world.Add(new Quad(new Point3(3, -2, 1), new Vec3(0, 0, 4), new Vec3(0, 4, 0), right));
            world.Add(new Quad(new Point3(-2, 3, 1), new Vec3(4, 0, 0), new Vec3(0, 0, 4), up));
            world.Add(new Quad(new Point3(-2, -3, 5), new Vec3(4, 0, 0), new Vec3(0, 0, -4), down));

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
                background = new ColorV2(0.70, 0.80, 1.00),

                vfov = 80, // field of view, basicallly zooming in and out 

                lookFrom = new Point3(0, 0, 9),
                lookAt = new Point3(0, 0, 0),
                vup = new Vec3(0, 1, 0),

                defocusAngle = 0,
            };
            camera.Render(world);
        }

        static void Triangle()
        {
            Stopwatch _stopwatch = Stopwatch.StartNew();
            //World            
            HittableList world = new HittableList();

            //materials (nice green)
            var material = new Lambertian(new ColorV2(0.535, 0.949, 0.211));

            //triangle
            world.Add(new Triangle(new Point3(-2, -2, 0), new Vec3(4, 0, 0), new Vec3(0, 4, 0), material));

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
                background = new ColorV2(0.70, 0.80, 1.00),

                vfov = 80, // field of view, basicallly zooming in and out 

                lookFrom = new Point3(0, 0, 9),
                lookAt = new Point3(0, 0, 0),
                vup = new Vec3(0, 1, 0),

                defocusAngle = 0,
            };
            camera.Render(world);
        }

        static void SimpleLight()
        {
            Stopwatch _stopwatch = Stopwatch.StartNew();
            //World            
            HittableList world = new HittableList();

            //objects
            var pertext = new NoiseTexture(4);
            world.Add(new Sphere(new Point3(0, -1000, 0), 1000, new Lambertian(pertext)));
            world.Add(new Sphere(new Point3(0, 2, 0), 2, new Lambertian(pertext)));

            //light source
            var light = new DiffuseLight(new ColorV2(4, 4, 4));
            world.Add(new Sphere(new Point3(0, 7, 0), 2, light));
            world.Add(new Quad(new Point3(3, 1, -2), new Vec3(2, 0, 0), new Vec3(0, 2, 0), light));

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
                background = new ColorV2(0, 0, 0),

                vfov = 20, // field of view, basicallly zooming in and out 

                lookFrom = new Point3(26, 3, 6),
                lookAt = new Point3(0, 2, 0),
                vup = new Vec3(0, 1, 0),

                defocusAngle = 0,
            };
            camera.Render(world);
        }

        static void CornellBox()
        {
            Stopwatch _stopwatch = Stopwatch.StartNew();
            //World            
            HittableList world = new HittableList();

            //materials
            var red = new Lambertian(new ColorV2(0.65, 0.05, 0.05));
            var white = new Lambertian(new ColorV2(0.73, 0.73, 0.73));
            var green = new Lambertian(new ColorV2(0.12, 0.45, 0.15));
            var light = new DiffuseLight(new ColorV2(15, 15, 15));

            //objects
            world.Add(new Quad(new Point3(555, 0, 0), new Vec3(0, 555, 0), new Vec3(0, 0, 555), green));
            world.Add(new Quad(new Point3(0, 0, 0), new Vec3(0, 555, 0), new Vec3(0, 0, 555), red));
            world.Add(new Quad(new Point3(343, 554, 332), new Vec3(-130, 0, 0), new Vec3(0, 0, -105), light));
            world.Add(new Quad(new Point3(0, 0, 0), new Vec3(555, 0, 0), new Vec3(0, 0, 555), white));
            world.Add(new Quad(new Point3(555, 555, 555), new Vec3(-555, 0, 0), new Vec3(0, 0, -555), white));
            world.Add(new Quad(new Point3(0, 0, 555), new Vec3(555, 0, 0), new Vec3(0, 555, 0), white));

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
                background = new ColorV2(0, 0, 0),

                vfov = 40, // field of view, basicallly zooming in and out 

                lookFrom = new Point3(278, 278, -800),
                lookAt = new Point3(278, 278, 0),
                vup = new Vec3(0, 1, 0),

                defocusAngle = 0,
            };
            camera.Render(world);
        }

        static void Main(string[] args)
        {
            switch (9)
            {
                case 1: CheckeredSceneBook1(); break;
                case 2: CheckeredSceneBook1_withEarth(); break;
                case 3: CheckeredSpheres(); break;
                case 4: Earth(); break;
                case 5: PerlinSpheres(); break;
                case 6: Quads(); break;
                case 7: Triangle(); break;
                case 8: SimpleLight(); break;
                case 9: CornellBox(); break;
            }
        }
    }
}
