using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace Match_3_Test
{
    public class ResourceManager
    {
        public static List<Brush> elementGraphics;
        public static List<Brush> backgroundGraphics;

        public static void LoadGraphics()
        {
            elementGraphics = new List<Brush>();
            backgroundGraphics = new List<Brush>();
            foreach (string file in Directory.GetFiles(System.IO.Path.GetFullPath("res\\img\\elements\\")))
            {
                elementGraphics.Add(new ImageBrush(LoadImageSource(file)));
            }
            foreach (string file in Directory.GetFiles(System.IO.Path.GetFullPath("res\\img\\backgrounds\\")))
            {
                backgroundGraphics.Add(new ImageBrush(LoadImageSource(file)));
            }
        }

        private static ImageSource LoadImageSource(string path)
        {
            string ext = System.IO.Path.GetExtension(path);
            if (ext == ".jpg" || ext == ".png")
            {
                ImageSource image;
                try
                {
                    image = new BitmapImage(new Uri(path));
                    Console.WriteLine("Loaded graphics: " + path);
                    return image;
                }
                catch
                {
                    throw new Exception("Failed to load graphics: " + path);
                }
            }
            else
                throw new FormatException("Inapropriate file format! Only .png and .jpg allowed.");

        }
    }
}
