using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raytracer.vectorsAndOthersforNow;


namespace Raytracer.A_External
{
    using System;
    using System.Net.Http.Headers;

    public class Perlin
    {
        private const int PointCount = 256;
        private Vec3[] randVectors = new Vec3[PointCount];
        private int[] permX = new int[PointCount];
        private int[] permY = new int[PointCount];
        private int[] permZ = new int[PointCount];

        public Perlin()
        {
            Random rand = new Random();
            for (int i = 0; i < PointCount; i++)
            {
                randVectors[i] = Vec3.UnitVector(Vec3.Random(-1, 1));


            }
            PerlinGeneratePerm(permX, rand);
            PerlinGeneratePerm(permY, rand);
            PerlinGeneratePerm(permZ, rand);
        }

        public double Noise(Point3 p)
        {
            var u = p.X - Math.Floor(p.X);
            var v = p.Y - Math.Floor(p.Y);
            var w = p.Z - Math.Floor(p.Z);

            var i = (int)Math.Floor(p.X);
            var j = (int)Math.Floor(p.Y);
            var k = (int)Math.Floor(p.Z);

            Vec3[,,] c = new Vec3[2, 2, 2];

            for (int di = 0; di < 2; di++)
            {
                for (int dj = 0; dj < 2; dj++)
                {
                    for (int dk = 0; dk < 2; dk++)
                    {
                        c[di, dj, dk] = randVectors[
                         permX[(i + di) & 255] ^
                         permY[(j + dj) & 255] ^
                         permZ[(k + dk) & 255] 
                        ];

                    }
                }
            }

            return TrilinearInterp(c, u, v, w);
        }

        private static double TrilinearInterp(Vec3[,,] c, double u, double v, double w)
        {
            double uu = u * u * (3 - 2 * u);
            double vv = v * v * (3 - 2 * v);
            double ww = w * w * (3 - 2 * w);
            double accum = 0.0;

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Vec3 weightV = new Vec3(u - i, v - j, w - k);
                        accum += (i * uu + (1 - i) * (1 - uu))
                               * (j * vv + (1 - j) * (1 - vv))
                               * (k * ww + (1 - k) * (1 - ww))
                               * Vec3.Dot(c[i, j, k], weightV);
                    }
                }
            }

            return accum;
        }

        public double Turbulence(Point3 p, int depth)
        {
            double accum = 0.0;
            Point3 tempP = p;
            double weight = 1.0;

            for (int i = 0; i < depth; i++)
            {
                accum += weight * Noise(tempP);
                weight *= 0.5;
                tempP *= 2; 
            }

            return Math.Abs(accum);
        }

        private static void PerlinGeneratePerm(int[] p, Random rand)
        {
            for (int i = 0; i < PointCount; i++)
            {
                p[i] = i;
            }
            Permute(p, PointCount, rand);
        }

        private static void Permute(int[] p, int n, Random rand)
        {
            for (int i = n - 1; i > 0; i--)
            {
                int target = rand.Next(0, i + 1);
                int tmp = p[i];
                p[i] = p[target];
                p[target] = tmp;
            }
        }

        private static double RandomDouble(Random rand)
        {
            return rand.NextDouble();
        }
    }
}
