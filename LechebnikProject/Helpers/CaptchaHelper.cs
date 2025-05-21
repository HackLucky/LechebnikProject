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
            int width = 100;
            int height = 30;

            using (var bitmap = new Bitmap(width, height))
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                using (var font = new Font("Arial", 20, FontStyle.Bold, GraphicsUnit.Pixel))
                using (var brush = new SolidBrush(Color.Black))
                {
                    g.DrawString(code, font, brush, new PointF(10, 5));
                }
                var rand = new Random();
                for (int i = 0; i < 100; i++)
                {
                    int x = rand.Next(width);
                    int y = rand.Next(height);
                    bitmap.SetPixel(x, y, Color.Gray);
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