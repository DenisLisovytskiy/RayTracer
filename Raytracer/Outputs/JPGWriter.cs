using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

public class JPGWriter
{
    public static void ConvertToJPG(string name)
    {
        string path = name + ".ppm";
        using (Image<Rgba32> image = Image.Load<Rgba32>(path))
        {
            image.Save(name + ".jpg");
        }
    }
}
