using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.vectorsAndOthersforNow
{
    public class Ray
    {
        // only get, should not be changed after being created (for now?)
        public Point3 Origin { get; }
        public Vec3 Direction { get; }

        // default
        public Ray() : this(new Point3(0, 0, 0), new Vec3(0, 0, 0)) { }

        public Ray(Point3 origin, Vec3 direction)
        {
            Origin = origin;
            Direction = direction;
        }

        // Returns the point at a given parameter t along the ray
        public Point3 At(double t)
        {
            Vec3 result = Origin + t * Direction;
            return new Point3(result.X, result.Y, result.Z);
        }
    }
}
