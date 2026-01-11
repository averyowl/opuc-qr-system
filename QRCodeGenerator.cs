/*
    Hunter Gambee-Iddings
    Natalya Langford
    Nathan Waggoner
    Philip Grazhdan
    Sean McCoy
    Wenkang Yu Zhen
*/


// `Install-Package QRCoder` with NuGet (under Tools -> NuGet Package Manager -> Package Manager Console in VS).
using QRCoder;

public static class QRCodeGenerator
{
    public static void Run(string[] args)
    {
        var payload = new PayloadGenerator.Url(args[0]);
        using var generator = new QRCoder.QRCodeGenerator();
        var level = QRCoder.QRCodeGenerator.ECCLevel.H;
        var qrCodeData = generator.CreateQrCode(payload, level);
        using var pngRenderer = new QRCoder.PngByteQRCode(qrCodeData);
        byte[] qrCodeImage = pngRenderer.GetGraphic(20);
        string path = "test.png";
        File.WriteAllBytes(path, qrCodeImage);
    }
}