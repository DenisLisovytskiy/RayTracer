using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.Utilities;
public static class UtilityFunctions
{
    private static Random random = new Random();

    public static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    public static double RandomDouble()
    {
        // Returns a random real in [0,1].
        return random.NextDouble();
    }

    public static double RandomDouble(double min, double max)
    {
        // Returns a random real in [min,max].
        return min + (max - min) * RandomDouble();
    }
}
