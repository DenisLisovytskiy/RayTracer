using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raytracer.Outputs;

namespace Raytracer.TextInterfacing
{
    internal class InputForms
    {
        public static int GetWidth()
        {
            Console.WriteLine(@"Please input the width of the image you'd like to render: (ratio is 16/9)");
            string? input = Console.ReadLine();
            if (input != null)
                input.TrimEnd();
            else
                throw new Exception("null in the input");
            int number;
            if (int.TryParse(input, out number))
            {
                return number;
            }
            else
            {
                Console.WriteLine("Invalid input, will use defaults");
                return 0;
            }
        }


        public static string GetName()
        {
            Console.WriteLine("Generating image...");
            Console.WriteLine("Please specify output file name(just name, no extension):");
            string? name = Console.ReadLine();
            if (name == null || name == "")
            {
                Console.WriteLine("Invalid input, will use \"default\"");
                name = "default";
            }
            return name;
        }

        public static bool WantJpeg()
        {
            while (true)
            {
                Console.WriteLine("Would you like to also get a .jpg file?\n" +
                    "(1- yes, 2-no)\n" +
                    "(.ppm file is always generated)");
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.KeyChar == '1')
                    return true;
                else if (key.KeyChar == '2')
                    return false;

                Console.WriteLine("Your input was invalid please try again!");
            }
        }
    }
}
