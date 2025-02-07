global using Point3 = Raytracer.vectorsAndOthersforNow.Vec3;
global using ColorV2 = Raytracer.vectorsAndOthersforNow.Vec3;

using System;
using System.IO;

namespace Raytracer.vectorsAndOthersforNow
{
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
            int rByte = (int)(255.999 * Math.Clamp(pixelColor.X, 0, 1));
            int gByte = (int)(255.999 * Math.Clamp(pixelColor.Y, 0, 1));
            int bByte = (int)(255.999 * Math.Clamp(pixelColor.Z, 0, 1));

            output.WriteLine($"{rByte} {gByte} {bByte}");
        }
    }
}
