using Raytracer.SceneElements;
using Raytracer.Utilities;
using Raytracer.vectorsAndOthersforNow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.Materials
{
    struct Dielectric : IMaterial
    {
        private double refractionindex;
        public Dielectric(double refractionIndex) 
        {
            this.refractionindex = refractionIndex;
        }

        private static double Reflectance(double cos, double refractionIndex)
        {
            //Schlicks approximation
            double r0 = (1 - refractionIndex) / (1 + refractionIndex);
            r0 *= r0;
            return r0 + (1 - r0) * Math.Pow((1 - cos), 5);
        }

        public bool Scatter(Ray rayIn, HitRecord record, out Point3 attenuation, out Ray scattered)
        {
            attenuation = new ColorV2(1.0, 1.0, 1.0);
            double ri = record.FrontFace ? (1.0/refractionindex) : refractionindex;
            Vec3 unitDirection = Vec3.UnitVector(rayIn.Direction);

            double cosTheta = Math.Min(Vec3.Dot(-unitDirection, record.Normal), 1.0);
            double sinTheta = Math.Sqrt(1 - cosTheta*cosTheta);

            bool cannotRefract = ri * sinTheta > 1.0;
            Vec3 direction;

            if (cannotRefract || Reflectance(cosTheta, ri) > UtilityFunctions.RandomDouble())
            {
                direction = Vec3.Reflect(unitDirection, record.Normal);
            }
            else
            {
                direction = Vec3.Refract(unitDirection, record.Normal, ri);
            }

            scattered = new Ray(record.P, direction);
            return true;
        }
    }
}
