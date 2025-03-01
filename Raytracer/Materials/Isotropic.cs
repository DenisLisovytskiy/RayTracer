using Raytracer.SceneElements;
using Raytracer.vectorsAndOthersforNow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.Materials
{
    struct Isotropic : IMaterial
    {
        private Texture texture;

        public Isotropic(ColorV2 albedo)
        {
            texture = new SolidColor(albedo);
        }

        public Isotropic(Texture texture)
        { 
            this.texture = texture;
        }

        public bool Scatter(Ray rayIn, HitRecord record, out Point3 attenuation, out Ray scattered)
        {
            scattered = new Ray(record.P, Vec3.RandomUnitVector(), rayIn.Time);
            attenuation = texture.Value(record.U, record.V, record.P);
            return true;
        }
    }
}
