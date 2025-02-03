using Raytracer.SceneElements;
using Raytracer.vectorsAndOthersforNow;
using System;
using System.Collections.Generic;

namespace Raytracer.SceneElements
{
    public class HittableList : IHittable
    {
        //simplification of: = new List<IHittable>();
        public List<IHittable> Objects { get; } = [];

        public HittableList() { }

        public HittableList(IHittable obj)
        {
            Add(obj);
        }

        public void Clear()
        {
            Objects.Clear();
        }

        public void Add(IHittable obj)
        {
            Objects.Add(obj);
        }

        public bool Hit(Ray ray, Interval rayT, out HitRecord record)
        {
            record = new HitRecord();
            HitRecord tmpRecord;
            bool hitAnything = false;
            double closestSoFar = rayT.Max;

            foreach (var obj in Objects)
            {
                if (obj.Hit(ray, new Interval(rayT.Min, closestSoFar), out tmpRecord))
                {
                    hitAnything = true;
                    closestSoFar = tmpRecord.T;
                    record = tmpRecord;
                }
            }

            return hitAnything;
        }
    }
}
