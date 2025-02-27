using Raytracer.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.SceneElements.Primitives2D
{
    internal class Triangle : PlanarPrimitive
    {
        public Triangle(Point3 Q, Point3 u, Point3 v, IMaterial material) : base(Q, u, v, material)
        { }

        public override bool IsInterior(double a, double b, HitRecord record)
        {
            if(!(a>0 && b >0 && a+b<1))
            {
                return false;
            }

            record.U = a;
            record.V = b;
            return true;
        }
    }
}
