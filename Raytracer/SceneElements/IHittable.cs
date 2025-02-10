using System;
using Raytracer.Materials;
using Raytracer.vectorsAndOthersforNow;
using Raytracer.BVH;

namespace Raytracer.SceneElements
{
    public struct HitRecord
    {
        public Point3 P { get; set; }
        public Point3 Normal { get; private set; }
        public IMaterial material;
        public double T { get; set; }
        public bool FrontFace { get; private set; }

        public HitRecord(Point3 p, Point3 normal, double t, bool frontFace)
        {
            P = p;
            Normal = normal;
            T = t;
            FrontFace = frontFace;
        }

        public void SetFaceNormal(in Ray ray, in Vec3 outwardNormal)
        {
            FrontFace = Vec3.Dot(ray.Direction, outwardNormal) < 0;
            Normal = FrontFace ? outwardNormal : -outwardNormal;
        }
    }

    public interface IHittable
    {
        bool Hit(Ray ray, Interval rayT, ref HitRecord record);

        AABB BoundingBox();
    }
}
