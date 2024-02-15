using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

using QRCoder;

namespace CN.Desktop.Display.Helpers;
public static class QrCode
{
    /// <summary>
    /// Generates a BitmapImage QRCode from a object's ToString() function 
    /// </summary>
    /// <param name="data">object to be ToString()'ed</param>
    /// <returns>A WPF Compatible Bitmap Image</returns>
    public static BitmapImage GetQrCode(object data)
    {
        using MemoryStream stream = new();
        QRCodeData qrCodeData = new QRCodeGenerator().CreateQrCode(data.ToString(), QRCodeGenerator.ECCLevel.Q);
        new QRCode(qrCodeData).GetGraphic(20).Save(stream, ImageFormat.Bmp);
        stream.Position = 0;

        BitmapImage image = new();
        image.BeginInit();
        image.StreamSource = stream;
        image.EndInit();

        return image;
    }
}
