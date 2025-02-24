using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;

namespace Raytracer.A_External;
public class RTWImage : IDisposable
{
    private int imageWidth;
    private int imageHeight;
    private float[] fdata; // Floating-point pixel data
    private byte[] bdata;  // 8-bit per channel pixel data
    private const int BytesPerPixel = 3;

    public RTWImage(string imageFilename)
    {
        LoadImage(imageFilename);
    }

    private void LoadImage(string filename)
    {
        string[] searchPaths = {
            filename,
            Path.Combine(Environment.GetEnvironmentVariable("RTW_IMAGES") ?? "", filename),
            "images/" + filename,
            "../images/" + filename,
            "../../images/" + filename,
            "../../../images/" + filename,
            "../../../../images/" + filename,
            "../../../../../images/" + filename,
            "../../../../../../images/" + filename
        };

        foreach (var path in searchPaths)
        {
            if (File.Exists(path))
            {
                try
                {
                    using var image = Image.Load<Rgba32>(path);
                    imageWidth = image.Width;
                    imageHeight = image.Height;
                    ConvertToFloatAndByte(image);
                    return;
                }
                catch (Exception)
                {
                    Console.WriteLine($"ERROR: Could not load image file '{path}'.");
                }
            }
        }

        Console.WriteLine($"ERROR: Could not locate image file '{filename}' in expected directories.");
        imageWidth = 0;
        imageHeight = 0;
        fdata = null;
        bdata = null;
    }

    private void ConvertToFloatAndByte(Image<Rgba32> image)
    {
        int totalPixels = imageWidth * imageHeight;
        fdata = new float[totalPixels * BytesPerPixel];
        bdata = new byte[totalPixels * BytesPerPixel];

        int index = 0;
        for (int y = 0; y < imageHeight; y++)
        {
            for (int x = 0; x < imageWidth; x++)
            {
                var pixel = image[x, y];

                // Store floating-point pixel data (C++ equivalent of stbi_loadf from the RayTracer: the next week )
                fdata[index] = pixel.R / 255.0f;
                fdata[index + 1] = pixel.G / 255.0f;
                fdata[index + 2] = pixel.B / 255.0f;

                // Convert to byte format (same as C++ float_to_byte from the RayTracer: the next week) 
                bdata[index] = FloatToByte(fdata[index]);
                bdata[index + 1] = FloatToByte(fdata[index + 1]);
                bdata[index + 2] = FloatToByte(fdata[index + 2]);

                index += BytesPerPixel;
            }
        }
    }

    private static byte FloatToByte(float value)
    {
        if (value <= 0.0f) return 0;
        if (value >= 1.0f) return 255;
        return (byte)(value * 256.0f);
    }

    public int Width => imageWidth;
    public int Height => imageHeight;

    public byte[] GetPixelData() => bdata;
    public float[] GetFloatPixelData() => fdata;

    public byte[] PixelData(int x, int y)
    {
        byte[] Magenta = { 255, 0, 255 };
        if (bdata == null) return Magenta;

        x = Clamp(x, 0, imageWidth - 1);
        y = Clamp(y, 0, imageHeight - 1);

        int index = (y * imageWidth + x) * BytesPerPixel;
        return new byte[] { bdata[index], bdata[index + 1], bdata[index + 2] };
    }

    private int Clamp(int value, int min, int max)
    {
        // Return the value clamped to the range [low, high).
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    public void Dispose()
    {
        fdata = null;
        bdata = null;
    }
}
