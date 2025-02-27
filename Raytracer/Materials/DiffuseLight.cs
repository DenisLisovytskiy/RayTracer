using Raytracer.SceneElements;
using Raytracer.vectorsAndOthersforNow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.Materials
{
    struct DiffuseLight : IMaterial
    {
        private Texture texture;
        
        public DiffuseLight(Texture tex)
        {
            texture = tex;
        }

        public DiffuseLight(ColorV2 emit)
        {
            texture = new SolidColor(emit);
        }

        public ColorV2 Emmited(double u, double v, Point3 p)
        {
            return texture.Value(u, v, p);
        }

        // This is not a great way to handle it but it works, needs to be changed later
        public bool Scatter(Ray rayIn, HitRecord record, out Point3 attenuation, out Ray scattered)
        {
            //these do not matter as they wont be used
            attenuation = new Vec3(0,0,0);
            //in order to not create a new object we pass rayin 
            scattered = rayIn;
            return false;
        }
    }
}
