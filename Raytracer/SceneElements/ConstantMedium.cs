using Raytracer.BVH;
using Raytracer.vectorsAndOthersforNow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Raytracer.Materials;
using Raytracer.Utilities;

namespace Raytracer.SceneElements
{
    internal class ConstantMedium : IHittable
    {
        private IHittable boundary;
        double negInvDensity;
        IMaterial phaseFunction;
        public ConstantMedium(IHittable boundary, double density, Texture texture)
        {
            this.boundary = boundary;
            this.negInvDensity = -1 / density;
            this.phaseFunction = new Isotropic(texture);
        }

        public ConstantMedium(IHittable boundary, double density, ColorV2 albedo)
        {
            this.boundary = boundary;
            this.negInvDensity = -1 / density;
            this.phaseFunction = new Isotropic(albedo);
        }

        public AABB BoundingBox()
        {
            return boundary.BoundingBox();
        }

        public bool Hit(Ray ray, Interval rayT, ref HitRecord record)
        {
            var record1 = new HitRecord();
            var record2 = new HitRecord();

            if (!boundary.Hit(ray, Interval.Universe, ref record1))
            {
                return false;
            }

            if (!boundary.Hit(ray, new Interval(record1.T + 0.0001, Double.PositiveInfinity), ref record2))
            {
                return false;
            }

            if (record1.T < rayT.Min)
            {
                record1.T = rayT.Min;
            }

            if (record2.T > rayT.Max)
            {
                record2.T = rayT.Max;
            }

            if (record1.T >= record2.T)
            {
                return false;
            }

            if (record1.T < 0)
            {
                record1.T = 0;
            }

            var rayLength = ray.Direction.Length();
            var distanceInsideBoundary = (record2.T - record1.T) * rayLength;
            var hitDistance = negInvDensity * Math.Log(UtilityFunctions.RandomDouble());

            if (distanceInsideBoundary < hitDistance)
            {
                return false;
            }

            record.T = record1.T + hitDistance / rayLength;
            record.P = ray.At(record.T);

            record.Normal = new Vec3(1, 0, 0);
            record.FrontFace = true;
            record.material = phaseFunction;

            return true;
        }
    }
}
