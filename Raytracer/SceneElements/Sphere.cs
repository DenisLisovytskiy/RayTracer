using Raytracer.SceneElements;
using Raytracer.vectorsAndOthersforNow;
using System;
using Raytracer.Materials;

namespace Raytracer.SceneElements
{
    public class Sphere : IHittable
    {
        public Point3 Center { get; }
        public double Radius { get; }
        private IMaterial material;

        public Sphere(Point3 center, double radius, IMaterial material)
        {
            Center = center;
            Radius = Math.Max(0, radius);
            this.material = material;
        }

        public bool Hit(Ray ray, Interval rayT, ref HitRecord record)
        {
            Vec3 oc = Center - ray.Origin;
            double a = ray.Direction.LengthSquared();
            double h = Vec3.Dot(ray.Direction, oc);
            double c = oc.LengthSquared() - Radius * Radius;

            double delta = h * h - a * c;
            if (delta < 0)
            {
                return false;
            }

            double sqrtd = Math.Sqrt(delta);
            // Find the nearest acceptable root
            double root = (h - sqrtd) / a;
            if (!rayT.Surrounds(root))
            {
                root = (h + sqrtd) / a;
                if (!rayT.Surrounds(root))
                {
                    return false;
                }
            }

            // Assign values directly to the struct instead of reinitializing
            record.T = root;
            record.P = ray.At(record.T);
            Vec3 outwardNormal = (record.P - Center) / Radius;
            record.SetFaceNormal(ray, outwardNormal);
            record.material = material;

            return true;
        }
    }
}
