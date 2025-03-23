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
using System.Diagnostics.Metrics;

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

        static void EmptyCornellBox()
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

            //world.Add(BoxCreator.Box(new Point3(130, 0, 65), new Point3(295, 165, 230), white));
            //world.Add(BoxCreator.Box(new Point3(295, 0, 295), new Point3(430, 330, 460), white));

            // Create a box
            var box1 = BoxCreator.Box(new Point3(0, 0, 0), new Point3(165, 330, 165), white);
            var rotatedbox1 = new RotateY(box1, 15);
            var translatedbox1 = new Translate(rotatedbox1, new Vec3(265, 0, 295));
            world.Add(translatedbox1);

            var box2 = BoxCreator.Box(new Point3(0, 0, 0), new Point3(165, 165, 165), white);
            var rotatedbox2 = new RotateY(box2, -18);
            var translatedbox2 = new Translate(rotatedbox2, new Vec3(130, 0, 65));
            world.Add(translatedbox2);

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

        static void CornellSmoke()
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

            // Create a box
            var box1 = BoxCreator.Box(new Point3(0, 0, 0), new Point3(165, 330, 165), white);
            var rotatedbox1 = new RotateY(box1, 15);
            var translatedbox1 = new Translate(rotatedbox1, new Vec3(265, 0, 295));
            world.Add(new ConstantMedium(translatedbox1, 0.01, new ColorV2(1, 0, 1)));

            var box2 = BoxCreator.Box(new Point3(0, 0, 0), new Point3(165, 165, 165), white);
            var rotatedbox2 = new RotateY(box2, -18);
            var translatedbox2 = new Translate(rotatedbox2, new Vec3(130, 0, 65));
            world.Add(new ConstantMedium(translatedbox2, 0.01, new ColorV2(0, 0, 1)));

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

        static void B2FinalScene()
        {
            Stopwatch _stopwatch = Stopwatch.StartNew();

           

            //World            
            HittableList world = new HittableList();

           

            var light = new DiffuseLight(new ColorV2(7, 7, 7));
            world.Add(new Quad(new Point3(123, 554, 147), new Vec3(300, 0, 0), new Vec3(0, 0, 265), light));

            var center1 = new Point3(400, 400, 200);
            var sphere_material = new Lambertian(new ColorV2(0.7, 0.3, 0.1));
            world.Add(new Sphere(center1, 50, sphere_material));

            world.Add(new Sphere(new Point3(260, 150, 45), 50, new Dielectric(1.5)));
            world.Add(new Sphere(
                new Point3(0, 150, 145), 50, new Metal(new ColorV2(0.8, 0.8, 0.9), 1.0)
            ));

            var boundary = new Sphere(new Point3(360, 150, 145), 70, new Dielectric(1.5));
            world.Add(boundary);
            world.Add(new ConstantMedium(boundary, 0.2, new ColorV2(0.2, 0.4, 0.9)));
            boundary = new Sphere(new Point3(0, 0, 0), 5000, new Dielectric(1.5));
            world.Add(new ConstantMedium(boundary, .0001, new ColorV2(1, 1, 1)));

            var emat = new Lambertian(new ImageTexture("earthmap.jpg"));
            world.Add(new Sphere(new Point3(400, 200, 400), 100, emat));
            var pertext = new NoiseTexture(0.2);
            world.Add(new Sphere(new Point3(220, 280, 300), 80, new Lambertian(pertext)));

            //-
            BVHNode bvhWorld = new BVHNode(world);
            world.Clear();
            world.Add(bvhWorld);
            //-

            //boxes1
            HittableList boxes1 = new HittableList();
            var ground = new Lambertian(new ColorV2(0.48, 0.83, 0.53));

            int boxes_per_side = 20;
            for (int i = 0; i < boxes_per_side; i++)
            {
                for (int j = 0; j < boxes_per_side; j++)
                {
                    var w = 100.0;
                    var x0 = -1000.0 + i * w;
                    var z0 = -1000.0 + j * w;
                    var y0 = 0.0;
                    var x1 = x0 + w;
                    var y1 = UtilityFunctions.RandomDouble(1, 101);
                    var z1 = z0 + w;

                    boxes1.Add(BoxCreator.Box(new Point3(x0, y0, z0), new Point3(x1, y1, z1), ground));
                }
            }
            //adding boxes1
            world.Add(new BVHNode(boxes1));

            //boxes2
            HittableList boxes2 = new HittableList();
            var white = new Lambertian(new ColorV2(0.73, 0.73, 0.73));
            int ns = 1000;
            for (int j = 0; j < ns; j++)
            {
                boxes2.Add(new Sphere(Point3.Random(0, 165) , 10, white));
            }
            //adding boxes2
            world.Add(new Translate(
                new RotateY(
                    new BVHNode(boxes2), 15),
                    new Vec3(-100, 270, 395)
                )
            );

            Camera camera = new()
            {
                aspectRatio = 16.0 / 9.0,
                imageWidth = 400,
                stopwatch = _stopwatch,
                samplesPerPixel = 400, // increase by one -> one more operation for every pixel
                                       // (even more, beacause it is a complex computation)
                                       // basically "how strong you want your antilaiasing" and how many rays per pixel
                maxDepth = 100, // used to determine how far recursion can go in RayColor
                background = new ColorV2(0, 0, 0),

                vfov = 40, // field of view, basicallly zooming in and out 

                lookFrom = new Point3(478, 278, -600),
                lookAt = new Point3(278, 278, 0),
                vup = new Vec3(0, 1, 0),

                defocusAngle = 0,
            };
            camera.Render(world);
        }

        static void Main(string[] args)
        {
            switch (12)
            {
                case 1: CheckeredSceneBook1(); break;
                case 2: CheckeredSceneBook1_withEarth(); break;
                case 3: CheckeredSpheres(); break;
                case 4: Earth(); break;
                case 5: PerlinSpheres(); break;
                case 6: Quads(); break;
                case 7: Triangle(); break;
                case 8: SimpleLight(); break;
                case 9: EmptyCornellBox(); break;
                case 10: CornellBox(); break;
                case 11: CornellSmoke(); break;
                case 12: B2FinalScene(); break;
            }
        }
    }
}
