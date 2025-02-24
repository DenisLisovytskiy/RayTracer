﻿using System;
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
        fdata = new float[imageWidth * imageHeight * BytesPerPixel];
        bdata = new byte[imageWidth * imageHeight * BytesPerPixel];

        int index = 0;
        for (int y = 0; y < imageHeight; y++)
        {
            for (int x = 0; x < imageWidth; x++)
            {
                var pixel = image[x, y];
                fdata[index] = pixel.R / 255.0f;
                fdata[index + 1] = pixel.G / 255.0f;
                fdata[index + 2] = pixel.B / 255.0f;

                bdata[index] = pixel.R;
                bdata[index + 1] = pixel.G;
                bdata[index + 2] = pixel.B;

                index += BytesPerPixel;
            }
        }
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
