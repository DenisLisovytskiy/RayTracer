using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raytracer.vectorsAndOthersforNow;

namespace Raytracer.SceneElements
{
    public class HitRecord
    {
        public Point3 P { get; set; }
        public Point3 Normal { get; set; }
        public double T { get; set; }
        public bool frontFace { get; set; }

        public void setFaceNormal(Ray ray, Vec3 outwardNormal)
        {
            frontFace = Vec3.Dot(ray.Direction, outwardNormal) < 0;
            Normal = frontFace? outwardNormal : - outwardNormal;
        }

        public HitRecord()
        {
            P = new Point3(0, 0, 0);
            Normal = new Point3(0, 0, 0);
            T = 0;
        }
    }

    public interface IHittable
    {
        bool Hit(Ray ray, Interval rayT, out HitRecord record);
    }
}
