#define structs 
using System;

namespace Raytracer.vectorsAndOthersforNow
{
    public struct Ray
    {
        public Point3 Origin { get; }
        public Vec3 Direction { get; }

        public double Time { get; }
        public Ray() : this(new Point3(0, 0, 0), new Vec3(0, 0, 0)) { }
        
        public Ray(Point3 origin, Vec3 direction)
        {
            Origin = origin;
            Direction = direction;
            Time = 0;
        }

        public Ray(Point3 origin, Vec3 direction, double time)
        {
            Origin = origin;
            Direction = direction;
            Time = time;
        }

        // Returns the point at a given parameter t along the ray
        // created only one object instead of two(like in the book),
        // it is because this method will be called countless times 

        public Point3 At(double t)
        {
            return new Point3(
                Origin.X + t * Direction.X,
                Origin.Y + t * Direction.Y,
                Origin.Z + t * Direction.Z
            );
        }
    }
}
