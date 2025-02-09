using Raytracer.SceneElements;
using Raytracer.vectorsAndOthersforNow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.Materials
{
    struct Metal : IMaterial
    {
        private ColorV2 albedo;
        private double fuzz;

        public Metal(ColorV2 albedo, double fuzz)
        {
            this.albedo = albedo;
            this.fuzz = (fuzz < 1) ? fuzz : 1;
        }

        public bool Scatter(Ray rayIn, HitRecord record, out Point3 attenuation, out Ray scattered)
        {
            Vec3 reflected = Vec3.Reflect(rayIn.Direction, record.Normal);
            reflected = Vec3.UnitVector(reflected) + (fuzz * Vec3.RandomUnitVector());
            scattered = new Ray(record.P, reflected);
            attenuation = albedo;
            return (Vec3.Dot(scattered.Direction, record.Normal) > 0);
        }
    }
}
