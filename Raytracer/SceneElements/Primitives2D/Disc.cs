using Raytracer.Materials;
using Raytracer.vectorsAndOthersforNow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.SceneElements.Primitives2D
{
    /// <summary>
    /// this class doesnt work as of now, yet it is our intent to fix it later
    /// </summary>
    internal class Disc : PlanarPrimitive
    {
        private double radius;
        public Disc(Point3 center, double radius, IMaterial material) : base(center, new Vec3(radius, 0, 0), new Vec3(0, radius, 0), material)
        {
            this.radius = radius;
        }

        public override void SetBoundingBox()
        {
            bbox = new BVH.AABB(Q - new Vec3(radius, radius, 0), Q + new Vec3(radius, radius, 0));
        }

        public override bool IsInterior(double a, double b, HitRecord record)
        {
            if (!(Math.Sqrt(a*a+b*b)<radius))
            {
                return false;
            }

            record.U = a / 2 + 0.5;
            record.V = b / 2 + 0.5;
            return true;
        }
    }
}
