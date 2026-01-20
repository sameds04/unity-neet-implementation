using System.IO;
using UnityEngine;

public class ImageUtils
{
    public static int size = (int) Mathf.Sqrt(GlobalVariables.inputs);
    public static int initial_size = size;

    public static Color[] GetImageDataFromCamera(Camera cam, bool render_image)
    {
        RenderTexture render = GetRenderTextureFromCamera(cam);
        Texture2D texture = ConvertToTexture2D(render);

        if (render_image)
        {
            Manager.input_view_image.texture = texture;
        }

        return Texture2DToBytes(texture);
    }

    public static RenderTexture GetRenderTextureFromCamera(Camera cam)
    {
        RenderTexture texture = new RenderTexture(initial_size, initial_size, 0);
        cam.targetTexture = texture;
        cam.Render();
        return texture;
    }

    public static Color[] Texture2DToBytes(Texture2D texture)
    {
        return texture.GetPixels();
    }

    public static Texture2D ConvertToTexture2D(RenderTexture texture)
    {
        Texture2D texture2d = new Texture2D(initial_size, initial_size, TextureFormat.RGB24, false);
        RenderTexture.active = texture;
        texture2d.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        texture2d.Apply();
        RenderTexture.active = null;
        return texture2d;
    }

    public static float[] RGBAToGrayscale(Color[] pixels)
    {
        float[] gray_pixels = new float[pixels.GetLength(0)];
        int count = 0;
        foreach (Color pixel in pixels)
        {
            float value = (0.299f * pixel.r) + (0.587f * pixel.g) + (0.114f * pixel.b);
            gray_pixels.SetValue(value, count); // Can't use pixel.grayscale because it uses intensity
            count += 1;
        }
        return gray_pixels;
    }

    public static float GetMax(float r, float g, float b)
    {
        if (r > g && r > b)
        {
            return r;
        }
        else if (g > r && g > b)
        {
            return g;
        }
        else
        {
            return b;
        }
    }

    private void SaveImage(Color[] colors)
    {
        Texture2D texture = new Texture2D(size, size);
        texture.SetPixels(colors);
        texture.Apply();
        File.WriteAllBytes("image_" + Random.Range(1000, 9999) + ".png", texture.EncodeToPNG());
    }
}
