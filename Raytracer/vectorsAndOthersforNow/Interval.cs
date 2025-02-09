using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.vectorsAndOthersforNow
{
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

        public double Clamp(double x)
        {
            if (x < Min) return Min;
            if (x > Max) return Max;
            return x;
        }

        public static readonly Interval Empty = new Interval(double.PositiveInfinity, double.NegativeInfinity);
        public static readonly Interval Universe = new Interval(double.NegativeInfinity, double.PositiveInfinity);
    }
}
