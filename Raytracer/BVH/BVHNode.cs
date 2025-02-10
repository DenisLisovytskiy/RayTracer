using Raytracer.SceneElements;
using Raytracer.Utilities;
using Raytracer.vectorsAndOthersforNow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.BVH
{
    public class BVHNode : IHittable
    {

        private IHittable left;
        private IHittable right;
        private AABB bbox;

        public BVHNode(HittableList list) : this(list.Objects, 0, list.Objects.Count) { }

        public BVHNode(List<IHittable> objects, int start, int end)
        {
            // Build the bounding box of the span of source objects.
            bbox = AABB.Empty;
            for (int object_index = start; object_index < end; object_index++)
            { 
                bbox = new AABB(bbox, objects[object_index].BoundingBox());
            }

            int axis = bbox.LongestAxis();
            Comparison<IHittable> comparator = axis switch
            {
                0 => BoxXCompare,
                1 => BoxYCompare,
                2 => BoxZCompare,
                _ => throw new ArgumentException("Invalid axis index", nameof(axis))
            };

            int objectSpan = end - start;

            if (objectSpan == 1)
            {
                left = right = objects[start];
            }
            else if (objectSpan == 2)
            {
                //swap the ONLY two elements in the objects list if they are in the wrong order
                if (comparator(objects[start], objects[start + 1]) > 0)
                {
                    (objects[start], objects[start + 1]) = (objects[start + 1], objects[start]);
                }

                left = objects[start];
                right = objects[start + 1];
            }
            else
            {
                objects.Sort(start, objectSpan, Comparer<IHittable>.Create(comparator));

                int mid = start + objectSpan / 2;
                left = new BVHNode(objects, start, mid);
                right = new BVHNode(objects, mid, end);
            }
        }

        public bool Hit(Ray ray, Interval rayT, ref HitRecord record)
        {
            record = new HitRecord();

            if (!bbox.Hit(ray, rayT))
            {
                return false;
            }

            bool hitLeft = left.Hit(ray, rayT, ref record);
            HitRecord recRight = new HitRecord();
            bool hitRight = right.Hit(ray, new Interval(rayT.Min, hitLeft ? record.T : rayT.Max), ref recRight);

            if (hitRight)
                record = recRight;

            return hitLeft || hitRight;
        }

        public AABB BoundingBox() => bbox;

        //Return -1 if a < b
        //Return 1 if a > b
        //Return 0 if a = b
        private static int BoxCompare(IHittable a, IHittable b, int axisIndex)
        {
            Interval aAxisInterval = a.BoundingBox().AxisInterval(axisIndex);
            Interval bAxisInterval = b.BoundingBox().AxisInterval(axisIndex);
            return aAxisInterval.Min.CompareTo(bAxisInterval.Min);
        }

        public static int BoxXCompare(IHittable a, IHittable b) => BoxCompare(a, b, 0);
        public static int BoxYCompare(IHittable a, IHittable b) => BoxCompare(a, b, 1);
        public static int BoxZCompare(IHittable a, IHittable b) => BoxCompare(a, b, 2);
    }
}
