using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace Match_3_Test
{
    /// <summary>
    /// Managing loading and gives access for game graphics.
    /// </summary>
    public class ResourceManager
    {
        public static List<Brush> elementGraphics;
        public static List<Brush> backgroundGraphics;
        public static Brush bonusBomb;
        public static Brush bonusVertical;
        public static Brush bonusHorizontal;
        public static Brush destructor;

        public static void LoadGraphics()
        {
            elementGraphics = new List<Brush>();
            backgroundGraphics = new List<Brush>();
            foreach (string file in Directory.GetFiles("res\\img\\elements\\"))
            {
                elementGraphics.Add(new ImageBrush(LoadImageSource(file)));
            }
            foreach (string file in Directory.GetFiles("res\\img\\backgrounds\\"))
            {
                backgroundGraphics.Add(new ImageBrush(LoadImageSource(file)));
            }
            bonusBomb = new ImageBrush(LoadImageSource("res\\img\\bonuses\\bomb.png"));
            bonusVertical = new ImageBrush(LoadImageSource("res\\img\\bonuses\\vertical.png"));
            bonusHorizontal = new ImageBrush(LoadImageSource("res\\img\\bonuses\\horizontal.png"));
            destructor = new ImageBrush(LoadImageSource("res\\img\\bonuses\\destructor.png"));
        }

        private static ImageSource LoadImageSource(string relativePath)
        {
            string path = System.IO.Path.GetFullPath(relativePath);
            string ext = System.IO.Path.GetExtension(path);
            if (ext == ".jpg" || ext == ".png")
            {
                ImageSource image;
                try
                {
                    image = new BitmapImage(new Uri(path));
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
