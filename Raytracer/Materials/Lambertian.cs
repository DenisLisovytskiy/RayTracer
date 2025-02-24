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
        private Texture texture;

        public Lambertian(ColorV2 albedo)
        {
            texture = new SolidColor(albedo);
        }
        public Lambertian(Texture tex)
        {
            this.texture = tex;
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
            attenuation = texture.Value(record.U, record.V, record.P);
            return true;
        }
    }
}
