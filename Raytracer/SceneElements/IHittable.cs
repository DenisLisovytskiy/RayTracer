    using System;
    using Raytracer.Materials;
    using Raytracer.vectorsAndOthersforNow;
    using Raytracer.BVH;
    using System.Numerics;

    namespace Raytracer.SceneElements
    {
        public interface IHittable
        {
            bool Hit(Ray ray, Interval rayT, ref HitRecord record);

            AABB BoundingBox();
        }
        public struct HitRecord
        {
            public Point3 P { get; set; }
            public Point3 Normal { get; internal set; }
            public IMaterial? material;
            public double T;
            public double U;
            public double V;
            public bool FrontFace { get; private set; }

            public HitRecord(Point3 p, Point3 normal, double t, bool frontFace)
            {
                P = p;
                Normal = normal;
                T = t;
                FrontFace = frontFace;
            }

            public void SetFaceNormal(in Ray ray, in Vec3 outwardNormal)
            {
                FrontFace = Vec3.Dot(ray.Direction, outwardNormal) < 0;
                Normal = FrontFace ? outwardNormal : -outwardNormal;
            }
        }

        class Translate : IHittable
        {
            private readonly Vec3 offset;
            private readonly IHittable obj;
            private readonly AABB bbox;
            public Translate(IHittable _obj, Vec3 _offset)
            {
                obj = _obj;
                offset = _offset;
                bbox = obj.BoundingBox() + offset; 
            }


            public bool Hit(Ray ray, Interval rayT, ref HitRecord record)
            {
                Ray offsetRay = new Ray(ray.Origin - offset, ray.Direction, ray.Time);

                if (!obj.Hit(offsetRay, rayT, ref record))
                    return false;

                // Move the intersection point forwards by the offset
                record.P += offset;

                return true;
            }

            public AABB BoundingBox()
            {
                return bbox;
            }
        }

        public class RotateY : IHittable
        {
            private readonly IHittable _object;
            private readonly double _sinTheta;
            private readonly double _cosTheta;
            private readonly AABB _bbox;

            public RotateY(IHittable obj, double angle)
            {
                _object = obj;
                double radians = Math.PI / 180 * angle;
                _sinTheta = Math.Sin(radians);
                _cosTheta = Math.Cos(radians);

                // Get bounding box of the original object
                _bbox = obj.BoundingBox();
        
                Point3 min = new Vec3(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
                Point3 max = new Vec3(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);

                // Rotate bounding box corners
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            double x = i * _bbox.x.Max + (1 - i) * _bbox.x.Min;
                            double y = j * _bbox.y.Max + (1 - j) * _bbox.y.Min;
                            double z = k * _bbox.z.Max + (1 - k) * _bbox.z.Min;

                            double newX = _cosTheta * x + _sinTheta * z;
                            double newZ = -_sinTheta * x + _cosTheta * z;

                            Vec3 tester = new Vec3(newX, y, newZ);

                            for (int c = 0; c < 3; c++)
                            {
                                min[c] = Math.Min(min[c], tester[c]);
                                max[c] = Math.Max(max[c], tester[c]);
                            }
                        }
                    }
                }

                _bbox = new AABB(min, max);
            }

            public bool Hit(Ray ray, Interval rayT, ref HitRecord rec)
            {
                // Transform the ray from world space to object space.
                Vec3 origin = new Vec3(
                    (_cosTheta * ray.Origin.X) - (_sinTheta * ray.Origin.Z),
                    ray.Origin.Y,
                    (_sinTheta * ray.Origin.X) + (_cosTheta * ray.Origin.Z)
                );

                Vec3 direction = new Vec3(
                    (_cosTheta * ray.Direction.X) - (_sinTheta * ray.Direction.Z),
                    ray.Direction.Y,
                    (_sinTheta * ray.Direction.X) + (_cosTheta * ray.Direction.Z)
                );

                Ray rotatedRay = new Ray(origin, direction, ray.Time);

                // Determine whether an intersection exists in object space.
                if (!_object.Hit(rotatedRay, rayT, ref rec))
                    return false;

                // Transform the intersection point from object space back to world space.
                rec.P = new Vec3(
                    (_cosTheta * rec.P.X) + (_sinTheta * rec.P.Z),
                    rec.P.Y,
                    (-_sinTheta * rec.P.X) + (_cosTheta * rec.P.Z)
                );

                rec.Normal = new Vec3(
                    (_cosTheta * rec.Normal.X) + (_sinTheta * rec.Normal.Z),
                    rec.Normal.Y,
                    (-_sinTheta * rec.Normal.X) + (_cosTheta * rec.Normal.Z)
                );

                return true;
            }

            public AABB BoundingBox()
            {
                return _bbox;
            }
        }

    }
