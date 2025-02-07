// only one can be created at a time, used to check the difference in speed

//#define structs 
#define classes 
using System;

namespace Raytracer.vectorsAndOthersforNow
{
#if structs
    public struct Ray
    {
        public Point3 Origin { get; }
        public Vec3 Direction { get; }

        public Ray() : this(new Point3(0, 0, 0), new Vec3(0, 0, 0)) { }

        public Ray(Point3 origin, Vec3 direction)
        {
            Origin = origin;
            Direction = direction;
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
#endif
#if classes
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
#endif
}
