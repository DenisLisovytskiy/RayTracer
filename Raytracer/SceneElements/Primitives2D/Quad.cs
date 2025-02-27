using Raytracer.BVH;
using Raytracer.Materials;
using Raytracer.vectorsAndOthersforNow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.SceneElements.Primitives2D
{
    internal class Quad : PlanarPrimitive
    {
        public Quad(Point3 Q, Point3 u, Point3 v, IMaterial material) : base(Q, u, v, material)
        { }

        public override bool IsInterior(double a, double b, HitRecord record)
        {
            Interval unitInterval = new Interval(0, 1);

            if (!unitInterval.Contains(a) || !unitInterval.Contains(b))
            {
                return false;
            }

            record.U = a;
            record.V = b;
            return true;
        }
    }
}
