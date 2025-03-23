using Raytracer.BVH;
using Raytracer.Materials;
using Raytracer.vectorsAndOthersforNow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.SceneElements.Primitives2D
{
    internal abstract class PlanarPrimitive : IHittable
    {
        /*
        Q -> the starting corner.
        u -> a vector representing the first side. Q+u gives one of the corners adjacent to Q
        v -> a vector representing the second side. Q+v gives the other corner adjacent to Q.
        */
        protected Point3 Q;
        protected Point3 u, v, w;
        protected IMaterial material;
        protected AABB bbox;
        protected Point3 normal;
        protected double D;
        //D is a const in the equation Ax+By+Cz=D

        public PlanarPrimitive(Point3 Q, Point3 u, Point3 v, IMaterial material)
        {
            this.Q = Q;
            this.u = u;
            this.v = v;
            this.material = material;

            var n = Point3.Cross(u, v);
            normal = Point3.UnitVector(n);
            D = Point3.Dot(normal, Q);
            w = n / Point3.Dot(n, n);

            SetBoundingBox();
        }

        public virtual void SetBoundingBox()
        {
            var bboxDiagonal1 = new AABB(Q, Q + u + v);
            var bboxDiagonal2 = new AABB(Q + u, Q + v);
            bbox = new AABB(bboxDiagonal1, bboxDiagonal2);
        }

        //checks if hit point is in the quad so actually a hit
        public abstract bool IsInterior(double a, double b, HitRecord record);

        public AABB BoundingBox()
        {
            return bbox;
        }

        //to solve for hits with a plane we use t= (D-nP)/n*d and if n*d is 0 we record a miss
        public bool Hit(Ray ray, Interval rayT, ref HitRecord record)
        {
            var denominator = Point3.Dot(normal, ray.Direction);

            //value close enough for us to count it as 0
            if (Math.Abs(denominator) < 1e-8)
            {
                return false;
            }

            var t = (D - Point3.Dot(normal, ray.Origin)) / denominator;

            //t is outside the ray interval counts as a miss
            if (!rayT.Contains(t))
            {
                return false;
            }

            var intersection = ray.At(t);
            Point3 planarHitPointVector = intersection - Q;
            var alpha = Point3.Dot(w, Point3.Cross(planarHitPointVector, v));
            var beta = Point3.Dot(w, Point3.Cross(u, planarHitPointVector));

            if (!IsInterior(alpha, beta, record))
            {
                return false;
            }

            //we got a hit!
            record.T = t;
            record.P = intersection;
            record.material = material;
            record.SetFaceNormal(ray, normal);

            return true;
        }
    }
}
