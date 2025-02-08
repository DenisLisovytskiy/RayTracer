// only one can be created at a time, used to check the difference in speed

#define structs 
//#define classes 

global using Point3 = Raytracer.vectorsAndOthersforNow.Vec3;
global using ColorV2 = Raytracer.vectorsAndOthersforNow.Vec3;

using System;
using System.IO;

namespace Raytracer.vectorsAndOthersforNow
{
#if structs
    public static class ColorUtils
    {
        public static readonly Interval Intensity = new Interval(0.000, 0.999);
    }
    public struct Vec3
    {
        public double X, Y, Z;

        public Vec3(double e0, double e1, double e2)
        {
            X = e0;
            Y = e1;
            Z = e2;
        }

        public Vec3 Negate() => new Vec3(-X, -Y, -Z);

        public double GetIndex(int i) => i == 0 ? X : (i == 1 ? Y : Z);
        public void SetIndex(int i, double value)
        {
            if (i == 0) X = value;
            else if (i == 1) Y = value;
            else Z = value;
        }

        public Vec3 Add(Vec3 v) => new Vec3(X + v.X, Y + v.Y, Z + v.Z);

        public Vec3 Subtract(Vec3 v) => new Vec3(X - v.X, Y - v.Y, Z - v.Z);

        public Vec3 Multiply(double t) => new Vec3(X * t, Y * t, Z * t);

        // Scalar Division 
        public Vec3 Divide(double t)
        {
            double invT = 1 / t;
            return new Vec3(X * invT, Y * invT, Z * invT);
        }

        public double Length() => Math.Sqrt(LengthSquared());

        // Squared Length (avoids expensive sqrt call)
        public double LengthSquared() => X * X + Y * Y + Z * Z;

        public static Vec3 operator +(Vec3 u, Vec3 v) => u.Add(v);
        public static Vec3 operator -(Vec3 u, Vec3 v) => u.Subtract(v);
        public static Vec3 operator *(Vec3 v, double t) => v.Multiply(t);
        public static Vec3 operator *(double t, Vec3 v) => v.Multiply(t);
        public static Vec3 operator /(Vec3 v, double t) => v.Divide(t);
        public static Vec3 operator -(Vec3 v) => v.Negate();

        // Dot Product
        public static double Dot(Vec3 u, Vec3 v) => u.X * v.X + u.Y * v.Y + u.Z * v.Z;

        // Cross Product
        public static Vec3 Cross(Vec3 u, Vec3 v)
        {
            return new Vec3(
                u.Y * v.Z - u.Z * v.Y,
                u.Z * v.X - u.X * v.Z,
                u.X * v.Y - u.Y * v.X
            );
        }

        // Unit Vector
        public static Vec3 UnitVector(Vec3 v) => v / v.Length();

        // for eventual debugging
        public override string ToString() => $"{X} {Y} {Z}";


        public void WriteColor(TextWriter output, ColorV2 pixelColor)
        {
            double r = ColorUtils.Intensity.Clamp(pixelColor.X);
            double g = ColorUtils.Intensity.Clamp(pixelColor.Y);
            double b = ColorUtils.Intensity.Clamp(pixelColor.Z);

            int rByte = (int)(256 * r);
            int gByte = (int)(256 * g);
            int bByte = (int)(256 * b);

            output.WriteLine($"{rByte} {gByte} {bByte}");
        }


    }
#endif
#if classes
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


     //for the color application of Vec3
    public void WriteColor(TextWriter output, ColorV2 pixelColor)
        {
            double r = ColorUtils.Intensity.Clamp(pixelColor.X);
            double g = ColorUtils.Intensity.Clamp(pixelColor.Y);
            double b = ColorUtils.Intensity.Clamp(pixelColor.Z);

            int rByte = (int)(256 * r);
            int gByte = (int)(256 * g);
            int bByte = (int)(256 * b);

            output.WriteLine($"{rByte} {gByte} {bByte}");
        }
 }
#endif
}
