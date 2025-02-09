
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

namespace Raytracer
{ 
    internal class Program
    {

        static void Main(string[] args)
        {
            Stopwatch _stopwatch = Stopwatch.StartNew();
            //World            
            HittableList world = new HittableList();

            var materialGround = new Lambertian(new ColorV2(0.8, 0.8, 0.0));
            var materialCenter = new Lambertian(new ColorV2(0.1, 0.2, 0.5));
            var materialLeft = new Dielectric(1.5);// refractive index of glass is ~1.5
            var materialBubble = new Dielectric(1.00 / 1.50); // sphere of air inside a glass sphere
            //var materialLeft = new Dielectric(1.00/1.33);// sphere of air in water
            var materialRight = new Metal(new ColorV2(0.8, 0.6, 0.2), 1.0);

            world.Add(new Sphere(new Point3(0.0, -100.5, -1.0), 100.0, materialGround));
            world.Add(new Sphere(new Point3(0.0, 0.0, -1.2), 0.5, materialCenter));
            world.Add(new Sphere(new Point3(-1.0, 0.0, -1.0), 0.5, materialLeft));
            world.Add(new Sphere(new Point3(-1.0, 0.0, -1.0), 0.4, materialBubble));
            world.Add(new Sphere(new Point3(1.0, 0.0, -1.0), 0.5, materialRight));


            Camera camera = new()
            {
                aspectRatio = 16.0 / 9.0,
                imageWidth = 400,
                stopwatch = _stopwatch,
                samplesPerPixel = 10, // increase by one -> one more operation for every pixel
                                      // (even more, beacause it is a complex computation)
                                      // basically "how strong you want your antilaiasing" 
                maxDepth = 10 // used to determine how far recursion can go in RayColor
            };

            camera.Render(world);
        }
    }
}
