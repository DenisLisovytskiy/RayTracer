using Raytracer.SceneElements;
using Raytracer.vectorsAndOthersforNow;
using System;

namespace Raytracer.SceneElements
{
    public class Sphere : IHittable
    {
        public Point3 Center { get; }
        public double Radius { get; }

        public Sphere(Point3 center, double radius)
        {
            Center = center;
            Radius = Math.Max(0, radius);
        }

        public bool Hit(Ray ray, Interval rayT, out HitRecord record)
        {
            record = new HitRecord();
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
            //find nearest acceptable root
            double root = (h - sqrtd) / a;
            if (!rayT.Surrounds(root))
            {
                root = (h + sqrtd) / a;
                if (!rayT.Surrounds(root))
                {
                    return false;
                }
            }

            record.T = root;
            record.P = ray.At(record.T);
            Vec3 outwardNormal = (record.P - Center) / Radius;
            record.setFaceNormal(ray, outwardNormal);

            return true;
        }
    }
}
