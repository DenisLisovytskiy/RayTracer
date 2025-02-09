using Raytracer.SceneElements;
using Raytracer.vectorsAndOthersforNow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.Materials
{
    struct Lambertian : IMaterial
    {
        private ColorV2 albedo;

        public Lambertian(ColorV2 albedo)
        {
            this.albedo = albedo;
        }
        public bool Scatter(Ray rayIn, HitRecord record, out ColorV2 attenuation, out Ray scattered)
        {
            var scatterDirection = record.Normal + Vec3.RandomUnitVector();
            //catch really small scatter directions to avoid problems later on
            if (scatterDirection.NearZero())
            {
                scatterDirection = record.Normal;
            }
            scattered = new Ray(record.P, scatterDirection);
            attenuation = albedo;
            return true;
        }
    }

    struct Metal : IMaterial
    {
        private ColorV2 albedo;
        private double fuzz;

        public Metal(ColorV2 albedo, double fuzz)
        {
            this.albedo = albedo;
            this.fuzz = (fuzz <1) ? fuzz : 1;
        }

        public bool Scatter(Ray rayIn, HitRecord record, out Point3 attenuation, out Ray scattered)
        {
            Vec3 reflected = Vec3.Reflect(rayIn.Direction, record.Normal);
            reflected = Vec3.UnitVector(reflected) + (fuzz * Vec3.RandomUnitVector());
            scattered = new Ray(record.P, reflected);
            attenuation = albedo;
            return (Vec3.Dot(scattered.Direction, record.Normal)>0);
        }
    }
}
