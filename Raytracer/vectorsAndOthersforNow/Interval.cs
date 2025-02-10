using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.vectorsAndOthersforNow
{
    public struct Interval
    {
        public double Min { get; set; }
        public double Max { get; set; }

        public Interval() : this(double.PositiveInfinity, double.NegativeInfinity) { }

        public Interval(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public Interval(Interval a, Interval b)
        {
            // Create the interval tightly enclosing the two input intervals.
            Min = a.Min <= b.Min ? a.Min : b.Min;
            Max = a.Max >= b.Max ? a.Max : b.Max;
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

        public Interval Expand(double delta)
        {
            double padding = delta / 2;
            return new Interval(Min - padding, Max + padding);
        }

        public static readonly Interval Empty = new Interval(double.PositiveInfinity, double.NegativeInfinity);
        public static readonly Interval Universe = new Interval(double.NegativeInfinity, double.PositiveInfinity);
    }
}
