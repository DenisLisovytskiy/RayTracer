using Raytracer.SceneElements;
using Raytracer.vectorsAndOthersforNow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.Materials
{
    public interface IMaterial
    {
        public bool Scatter(Ray rayIn, HitRecord record, out ColorV2 attenuation, out Ray scattered);
    }
}
