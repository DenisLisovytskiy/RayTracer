global using Point3 = Raytracer.vectorsAndOthersforNow.Vec3;
global using ColorV2 = Raytracer.vectorsAndOthersforNow.Vec3;

using System;
using System.IO;
using Raytracer.Utilities;

namespace Raytracer.vectorsAndOthersforNow
{

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

        public static Vec3 Random()
        {
            return new Vec3(
                UtilityFunctions.RandomDouble(),
                UtilityFunctions.RandomDouble(),
                UtilityFunctions.RandomDouble()
            );
        }

        public static Vec3 Random(double min, double max)
        {
            return new Vec3(
                UtilityFunctions.RandomDouble(min, max),
                UtilityFunctions.RandomDouble(min, max),
                UtilityFunctions.RandomDouble(min, max)
            );
        }
        public static Vec3 operator +(Vec3 u, Vec3 v) => u.Add(v);
        public static Vec3 operator -(Vec3 u, Vec3 v) => u.Subtract(v);
        public static Vec3 operator *(Vec3 v, double t) => v.Multiply(t);
        public static Vec3 operator *(double t, Vec3 v) => v.Multiply(t);
        public static Vec3 operator /(Vec3 v, double t) => v.Divide(t);
        public static Vec3 operator -(Vec3 v) => v.Negate();
        public static Vec3 operator *(Vec3 u, Vec3 v)
        {
            return new Vec3(u.X * v.X, u.Y * v.Y, u.Z * v.Z);
        }

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

        public static Vec3 RandomInUnitDisk()
        {
            while (true)
            {
                var p = new Vec3(UtilityFunctions.RandomDouble(-1, 1), UtilityFunctions.RandomDouble(-1, 1), 0);
                if (p.LengthSquared() < 1)
                {
                    return p;
                }
            }
        }

        public static Vec3 RandomUnitVector()
        {
            while (true)
            {
                Vec3 p = Random(-1, 1);
                double lensq = p.LengthSquared();
                if (lensq > 1e-160 && lensq <= 1)
                    return UnitVector(p);
            }
        }

        public static Vec3 RandomOnHemisphere(Vec3 normal)
        {
            Vec3 onUnitSphere = RandomUnitVector();
            return Dot(onUnitSphere, normal) > 0.0 ? onUnitSphere : -onUnitSphere;
        }

        public static double LinearToGamma(double linearComponent)
        {
            return linearComponent > 0 ? Math.Sqrt(linearComponent) : 0;
        }

        public bool NearZero()
        {
            var s = 1e-8;
            return (Math.Abs(X) < s) && (Math.Abs(Y) < s) && (Math.Abs(Z) < s);
        }

        public static Vec3 Reflect(Vec3 v, Vec3 n)
        {
            return v - 2 * Vec3.Dot(v, n) * n;
        }

        public static Vec3 Refract(Vec3 u, Vec3 n, double etaIOverEtaT)
        {
            double cosTheta = Math.Min(Vec3.Dot(-u, n), 1.0);
            Vec3 rayOutPerpendicular = etaIOverEtaT * (u + cosTheta * n);
            Vec3 rayOutParallel = -Math.Sqrt(Math.Abs(1.0 - rayOutPerpendicular.LengthSquared())) * n;
            return rayOutPerpendicular + rayOutParallel;
        }

        // for eventual debugging
        public override string ToString() => $"{X} {Y} {Z}";

        public void WriteColor(TextWriter output, ColorV2 pixelColor)
        {
            double r = ColorUtils.Intensity.Clamp(pixelColor.X);
            double g = ColorUtils.Intensity.Clamp(pixelColor.Y);
            double b = ColorUtils.Intensity.Clamp(pixelColor.Z);

            // Apply a linear to gamma transform for gamma 2
            r = LinearToGamma(r);
            g = LinearToGamma(g);
            b = LinearToGamma(b);

            int rByte = (int)(256 * r);
            int gByte = (int)(256 * g);
            int bByte = (int)(256 * b);

            output.WriteLine($"{rByte} {gByte} {bByte}");
        }

        public double this[int index]
        {
            get => index switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                _ => throw new IndexOutOfRangeException("Vec3 available indices are 0, 1, 2")
            };
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    default: throw new IndexOutOfRangeException("Vec3 available indices are 0, 1, 2");
                }
            }
        }

    }
}
