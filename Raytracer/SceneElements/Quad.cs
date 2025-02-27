using Raytracer.BVH;
using Raytracer.Materials;
using Raytracer.vectorsAndOthersforNow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.SceneElements
{
    internal class Quad : IHittable
    {
        /*
        Q -> the starting corner.
        u -> a vector representing the first side. Q+u gives one of the corners adjacent to Q
        v -> a vector representing the second side. Q+v gives the other corner adjacent to Q.
        */
        private Point3 Q;
        private Vec3 u, v, w;
        private IMaterial material;
        AABB bbox;
        private Vec3 normal;
        double D;
        //D is a const in the equation Ax+By+Cz=D

        public Quad(Point3 Q, Vec3 u, Vec3 v, IMaterial material)
        {
            this.Q = Q;
            this.u = u;
            this.v = v;
            this.material = material;

            var n = Vec3.Cross(u, v);
            normal = Vec3.UnitVector(n);
            D = Vec3.Dot(normal, Q);
            w = n / Vec3.Dot(n, n);

            SetBoundingBox();
        }

        public virtual void SetBoundingBox()
        {
            var bboxDiagonal1 = new AABB(Q, Q + u + v);
            var bboxDiagonal2 = new AABB(Q + u, Q + v);
            bbox = new AABB(bboxDiagonal1, bboxDiagonal2);
        }

        //checks if hit point is in the quad so actually a hit
        public virtual bool IsInterior(double a, double b, HitRecord record)
        {
            Interval unitInterval = new Interval(0, 1);

            if (!unitInterval.Contains(a) || !unitInterval.Contains(b))
            {
                return false;
            }

            record.U = a;
            record.V = b;
            return true;
        }

        public AABB BoundingBox()
        {
            return bbox;
        }

        //to solve for hits with a plane we use t= (D-nP)/n*d and if n*d is 0 we record a miss
        public bool Hit(Ray ray, Interval rayT, ref HitRecord record)
        {
            var denominator = Vec3.Dot(normal, ray.Direction);

            //value close enough for us to count it as 0
            if (Math.Abs(denominator) < 1e-8)
            {
                return false;
            }

            var t = (D - Vec3.Dot(normal, ray.Origin)) / denominator;

            //t is outside the ray interval counts as a miss
            if (!rayT.Contains(t))
            {
                return false;
            }

            var intersection = ray.At(t);
            Vec3 planarHitPointVector = intersection - Q;
            var alpha = Vec3.Dot(w, Vec3.Cross(planarHitPointVector, v));
            var beta = Vec3.Dot(w, Vec3.Cross(u, planarHitPointVector));

            if(!IsInterior(alpha, beta, record))
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
