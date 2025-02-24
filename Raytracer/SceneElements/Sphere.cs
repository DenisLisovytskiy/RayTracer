using Raytracer.SceneElements;
using Raytracer.vectorsAndOthersforNow;
using System;
using Raytracer.Materials;
using Raytracer.BVH;

namespace Raytracer.SceneElements
{
    public class Sphere : IHittable
    {
        public Point3 Center { get; }
        public double Radius { get; }
        private IMaterial material;
        private AABB bbox;

        public AABB BoundingBox()
        {
            return bbox;
        }
        public Sphere(Point3 center, double radius, IMaterial material)
        {
            Center = center;
            Radius = Math.Max(0, radius);
            this.material = material;
            var rvec = new Vec3(radius, radius, radius);
            bbox = new AABB(center - rvec, center + rvec);
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
            GetSphereUV(outwardNormal, out record.U, out record.V);
            record.material = material;

            return true;
        }
        private static void GetSphereUV(Point3 p, out double u, out double v)
        {
            // p: a given point on the sphere of radius one, centered at the origin.
            // u: returned value [0,1] of angle around the Y axis from X=-1.
            // v: returned value [0,1] of angle from Y=-1 to Y=+1.

            // < 1 0 0 > yields < 0.50 0.50 >    <-1  0  0> yields <0.00 0.50>
            // <0 1 0> yields <0.50 1.00>    < 0 -1  0> yields <0.50 0.00>
            // <0 0 1> yields <0.25 0.50>     < 0  0 -1> yields <0.75 0.50>

            // Calculate theta and phi
            double theta = Math.Acos(-p.Y); // Angle from the Y-axis
            double phi = Math.Atan2(-p.Z, p.X) + Math.PI; // Angle around the Y-axis

            // Calculate UV coordinates
            u = phi / (2 * Math.PI);
            v = theta / Math.PI; // Scale theta to [0,1]
        }

    }
}
