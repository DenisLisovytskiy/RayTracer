
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

namespace Raytracer
{ 
    internal class Program
    {

        double degreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        static void Main(string[] args)
        {
            Stopwatch _stopwatch = Stopwatch.StartNew();
            //World            
            HittableList world = new(new Sphere(new Point3(0, 0, -1), 0.5));
            world.Add(new Sphere(new Point3(0, -100.5, -1), 100));

            Camera camera = new()
            {
                aspectRatio = 16.0 / 9.0,
                imageWidth = 400,
                stopwatch = _stopwatch
            };

            camera.Render(world);
        }
    }
}
