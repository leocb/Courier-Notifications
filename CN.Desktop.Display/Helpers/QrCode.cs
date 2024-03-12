using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

using QRCoder;

namespace CN.Desktop.Display.Helpers;
public static class QrCode
{
    /// <summary>
    /// Generates a BitmapImage QRCode from a string
    /// </summary>
    /// <param name="content">The QRCode content</param>
    /// <returns>A WPF Compatible Bitmap Image</returns>
    public static BitmapImage Generate(string content)
    {
        using MemoryStream stream = new();
        QRCodeData qrCodeData = new QRCodeGenerator().CreateQrCode(content.ToString(), QRCodeGenerator.ECCLevel.M);
        new QRCode(qrCodeData).GetGraphic(20).Save(stream, ImageFormat.Bmp);
        stream.Position = 0;

        BitmapImage image = new();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = stream;
        image.EndInit();

        return image;
    }

    public static BitmapImage ExampleImage => Generate("https://hostname.com/join?channel=40d98a81-f4a0-4ce6-9e21-19803c1fdf4f&auth=40d98a81-f4a0-4ce6-9e21-19803c1fdf4f");
}
