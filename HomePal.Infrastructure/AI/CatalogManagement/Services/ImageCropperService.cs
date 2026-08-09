using SkiaSharp;

namespace HomePal.Infrastructure.AI.CatalogManagement.Services;

public class BoundingBoxDto
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}

public static class ImageCropperService
{
    public static byte[]? CropRegion(byte[] imageBytes, BoundingBoxDto? box)
    {
        if (imageBytes == null || imageBytes.Length == 0 || box == null)
            return null;

        using var original = SKBitmap.Decode(imageBytes);
        if (original == null)
            return null;

        var imgWidth = original.Width;
        var imgHeight = original.Height;

        var maxComponent = Math.Max(Math.Max(box.X, box.Y), Math.Max(box.Width, box.Height));
        var scale = maxComponent switch
        {
            > 100 => 1000.0,
            > 1 => 100.0,
            _ => 1.0
        };

        var normX = box.X / scale;
        var normY = box.Y / scale;
        var normWidth = box.Width / scale;
        var normHeight = box.Height / scale;

        if (normWidth < 0.01 || normHeight < 0.01)
            return null;

        var x = (int)Math.Clamp(normX * imgWidth, 0, imgWidth - 1);
        var y = (int)Math.Clamp(normY * imgHeight, 0, imgHeight - 1);
        var width = (int)Math.Clamp(normWidth * imgWidth, 1, imgWidth - x);
        var height = (int)Math.Clamp(normHeight * imgHeight, 1, imgHeight - y);

        using var cropped = new SKBitmap(width, height);
        var cropRegion = new SKRectI(x, y, x + width, y + height);
        if (!original.ExtractSubset(cropped, cropRegion))
            return null;

        using var image = SKImage.FromBitmap(cropped);
        using var encoded = image.Encode(SKEncodedImageFormat.Jpeg, 90);
        using var memoryStream = new MemoryStream();
        encoded.SaveTo(memoryStream);
        return memoryStream.ToArray();
    }
}
