using Raytracer.vectorsAndOthersforNow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.BVH
{
    //Axis-Aligned Bounding Box
    public struct AABB
    {
        public Interval x;
        public Interval y;
        public Interval z;
        public AABB() { }
        public static readonly AABB Empty = new AABB(Interval.Empty, Interval.Empty, Interval.Empty);
        public static readonly AABB Universe = new AABB(Interval.Universe, Interval.Universe, Interval.Universe);

        public AABB(Interval x, Interval y, Interval z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public AABB(Point3 a, Point3 b)
        // Treat the two points a and b as extrema for the bounding box, so we don't require a
        // particular minimum/maximum coordinate order.
        {
            x = (a.X <= b.X) ? new Interval(a.X, b.X) : new Interval(b.X, a.X);
            y = (a.Y <= b.Y) ? new Interval(a.Y, b.Y) : new Interval(b.Y, a.Y);
            z = (a.Z <= b.Z) ? new Interval(a.Z, b.Z) : new Interval(b.Z, a.Z);
        }

        public AABB(AABB box0, AABB box1)
        {
            x = new Interval(box0.x, box1.x);
            y = new Interval(box0.y, box1.y);
            z = new Interval(box0.z, box1.z);
        }

        public Interval AxisInterval(int n)
        {
            if (n == 1) return y;
            if (n == 2) return z;
            return x;
        }

        public bool Hit(Ray ray, Interval rayT)
        {
            Point3 rayOrigin = ray.Origin;
            Vec3 rayDirection = ray.Direction;

            for (int axis = 0; axis < 3; axis++)
            {
                Interval ax = AxisInterval(axis);
                double adinv = 1.0 / rayDirection[axis];

                var t0 = (ax.Min - rayOrigin[axis]) * adinv;
                var t1 = (ax.Max - rayOrigin[axis]) * adinv;

                if (t0 < t1)
                {
                    if (t0 > rayT.Min) rayT.Min = t0;
                    if (t1 < rayT.Max) rayT.Max = t1;
                }
                else
                {
                    if (t1 > rayT.Min) rayT.Min = t1;
                    if (t0 < rayT.Max) rayT.Max = t0;
                }

                if (rayT.Max <= rayT.Min)
                    return false;
            }
            return true;
        }
        public int LongestAxis()
        {
            // Returns the index of the longest axis of the bounding box.

            if (x.Size() > y.Size())
                return x.Size() > z.Size() ? 0 : 2;
            else
                return y.Size() > z.Size() ? 1 : 2;
        }
    }
}
