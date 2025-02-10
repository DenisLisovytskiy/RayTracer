using Raytracer.SceneElements;
using Raytracer.vectorsAndOthersforNow;
using System;
using System.Collections.Generic;

namespace Raytracer.SceneElements
{
    public class HittableList : IHittable
    {
        // Using C# 9.0+ shorthand initialization for an empty list
        public List<IHittable> Objects { get; } = new();

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

        public bool Hit(Ray ray, Interval rayT, ref HitRecord record)
        {
            bool hitAnything = false;
            double closestSoFar = rayT.Max;

            foreach (var obj in Objects)
            {
                HitRecord tmpRecord = default; // Structs need explicit initialization

                if (obj.Hit(ray, new Interval(rayT.Min, closestSoFar), ref tmpRecord))
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


