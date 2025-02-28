using Raytracer.Materials;
using Raytracer.SceneElements.Primitives2D;
using Raytracer.vectorsAndOthersforNow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.SceneElements
{
    public class BoxCreator
    {
        public static HittableList Box(Point3 a, Point3 b, IMaterial material) 
        {
            var sides = new HittableList();

            var min = new Point3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
            var max = new Point3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

            var dx = new Vec3(max.X - min.X, 0, 0);
            var dy = new Vec3(0, max.Y - min.Y, 0);
            var dz = new Vec3(0, 0, max.Z - min.Z);

            sides.Add(new Quad(new Point3(min.X, min.Y, max.Z), dx, dy, material)); // front
            sides.Add(new Quad(new Point3(max.X, min.Y, max.Z), -dz, dy, material)); // right
            sides.Add(new Quad(new Point3(max.X, min.Y, min.Z), -dx, dy, material)); // back
            sides.Add(new Quad(new Point3(min.X, min.Y, min.Z), dz, dy, material)); // left 
            sides.Add(new Quad(new Point3(min.X, max.Y, max.Z), dx, -dz, material)); // top 
            sides.Add(new Quad(new Point3(min.X, min.Y, min.Z), dx, dz, material)); // bottom

            return sides;
        }
    }
}
