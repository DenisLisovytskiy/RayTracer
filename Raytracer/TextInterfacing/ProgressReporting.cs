using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.TextInterfacing
{
    public class ProgressReporting
    {
        public static void DoneMessage(string elapsedTime)
        {
            Console.WriteLine("ImageCustom generated successfully!");
            Console.WriteLine($"Operation took: {elapsedTime}ms");
        }

        public static void ProgressMessage(int linesRemaining)
        {
            Console.Clear();
            Console.WriteLine($"Scanlines remaining: {linesRemaining}\n");
        }

        public static void ExceptionMessage(Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
