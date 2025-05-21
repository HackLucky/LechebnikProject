using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace LechebnikProject.Helpers
{
    public static class ImageHelper
    {
        public static BitmapImage Base64ToBitmapImage(string base64String)
        {
            byte[] binaryData = Convert.FromBase64String(base64String);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(binaryData);
            bitmapImage.EndInit();
            bitmapImage.Freeze(); // Для потокобезопасности в WPF
            return bitmapImage;
        }
    }
}