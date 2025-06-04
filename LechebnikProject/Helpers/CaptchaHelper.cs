using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace LechebnikProject.Helpers
{
    public static class CaptchaHelper
    {
        public static (string Code, string ImageBase64) GenerateCaptcha()
        {
            string code = new Random().Next(1000, 9999).ToString();
            int width = 1300;
            int height = 50;

            using (var bitmap = new Bitmap(width, height))
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.DarkGray);
                using (var font = new Font("Arial", 35, FontStyle.Bold, GraphicsUnit.Pixel))
                using (var brush = new SolidBrush(Color.Green))
                {
                    g.DrawString(code, font, brush, new PointF(10, 5));
                }
                var rand = new Random();
                for (int i = 0; i < 10000; i++)
                {
                    int x = rand.Next(width);
                    int y = rand.Next(height);
                    bitmap.SetPixel(x, y, Color.Black);
                }
                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    string imageBase64 = Convert.ToBase64String(ms.ToArray());
                    return (code, imageBase64);
                }
            }
        }
    }
}