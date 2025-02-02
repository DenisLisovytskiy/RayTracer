using static System.Net.Mime.MediaTypeNames;
using Raytracer.Interfacing;
using Raytracer.Outputs;
using System.Xml.Linq;
using Raytracer.TextInterfacing;

namespace Raytracer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var (width, height) = InputForms.GetDimensions();
            ImageCustom image = new ImageCustom(width, height);

            string name = InputForms.GetName();
            using (PPMWriter writer = new(name))
            {
                writer.GenerateImage(image);
            }
            if (InputForms.WantJpeg())
            {
                JPGWriter jpegWriter = new(name);
                jpegWriter.ConvertToJPG();
            }
        }
    }
}
