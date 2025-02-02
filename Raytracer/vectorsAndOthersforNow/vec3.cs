global using Point3 = Raytracer.vectorsAndOthersforNow.Vec3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Raytracer.vectorsAndOthersforNow
{

    public class Vec3
    {
        public double[] E { get; } = new double[3];

        public Vec3() : this(0, 0, 0) { }
        public Vec3(double e0, double e1, double e2)
        {
            E[0] = e0;
            E[1] = e1;
            E[2] = e2;
        }

        public double X => E[0];
        public double Y => E[1];
        public double Z => E[2];

        public Vec3 Negate() => new Vec3(-E[0], -E[1], -E[2]);

        public double GetIndex(int i) => E[i];
        public void SetIndex(int i, double value) => E[i] = value;

        public Vec3 Add(Vec3 v)
        {
            return new Vec3(E[0] + v.E[0], E[1] + v.E[1], E[2] + v.E[2]);
        }

        public Vec3 Subtract(Vec3 v)
        {
            return new Vec3(E[0] - v.E[0], E[1] - v.E[1], E[2] - v.E[2]);
        }

        public Vec3 Multiply(double t)
        {
            return new Vec3(E[0] * t, E[1] * t, E[2] * t);
        }

        public Vec3 Divide(double t)
        {
            return Multiply(1 / t);
        }

        public double Length() => Math.Sqrt(LengthSquared());
        public double LengthSquared() => E[0] * E[0] + E[1] * E[1] + E[2] * E[2];

        public static Vec3 operator +(Vec3 u, Vec3 v) => u.Add(v);
        public static Vec3 operator -(Vec3 u, Vec3 v) => u.Subtract(v);
        public static Vec3 operator *(Vec3 v, double t) => v.Multiply(t);
        public static Vec3 operator *(double t, Vec3 v) => v.Multiply(t);
        public static Vec3 operator /(Vec3 v, double t) => v.Divide(t);
        public static Vec3 operator -(Vec3 v) => v.Negate();

        // Dot Product
        public static double Dot(Vec3 u, Vec3 v)
        {
            return u.E[0] * v.E[0] + u.E[1] * v.E[1] + u.E[2] * v.E[2];
        }

        // Cross Product
        public static Vec3 Cross(Vec3 u, Vec3 v)
        {
            return new Vec3(
                u.E[1] * v.E[2] - u.E[2] * v.E[1],
                u.E[2] * v.E[0] - u.E[0] * v.E[2],
                u.E[0] * v.E[1] - u.E[1] * v.E[0]
            );
        }

        // Unit Vector
        public static Vec3 UnitVector(Vec3 v) => v / v.Length();

        public override string ToString() => $"{E[0]} {E[1]} {E[2]}";
    }

    // Alias for Vec3 to represent a point
    //public class Point3 : Vec3
    //{
    //    public Point3(double x, double y, double z) : base(x, y, z) { }
    //}


}
