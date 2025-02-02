using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.TextInterfacing
{
    public class ProgressReporting
    {
        public static void DoneMessage()    
        {
            Console.WriteLine("ImageCustom generated successfully!");
        }

        //no longer used
        public static void ProgressMessage(int linesRemaining)
        {
            Console.Clear();
            Console.WriteLine($"Scanlines remaining: {linesRemaining}\n");
        }
    }
}
