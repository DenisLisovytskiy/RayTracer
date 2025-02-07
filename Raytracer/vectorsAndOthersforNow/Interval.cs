using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.vectorsAndOthersforNow
{ 
    // I guess that it is better to leave Interval as a class, becaause
    // - it is not often created, unlike we got a ray per every pixel;
    // - it is passed around a lot, which is more convenient to do with classes

    public class Interval 
    {
        public double Min { get; }
        public double Max { get; }

        public Interval() : this(double.PositiveInfinity, double.NegativeInfinity) { }

        public Interval(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public double Size()
        {
            return Max - Min;
        }

        public bool Contains(double x)
        {
            return Min <= x && x <= Max;
        }

        public bool Surrounds(double x)
        {
            return Min < x && x < Max;
        }

        public static readonly Interval Empty = new Interval(double.PositiveInfinity, double.NegativeInfinity);
        public static readonly Interval Universe = new Interval(double.NegativeInfinity, double.PositiveInfinity);
    }
}
