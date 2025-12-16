using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;

namespace OnlineStoreAPIs.Services
{
    public static class ImageProcessingService
    {
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".tiff"
        };

        private const int DefaultQuality = 82;

        public static bool IsSupported(string extension) => AllowedExtensions.Contains(extension);

        public static async Task<string> SaveCompressedImageAsync(IFormFile imageFile, string folderName)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("No file uploaded.", nameof(imageFile));
            }

            var originalExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!IsSupported(originalExtension))
            {
                throw new InvalidOperationException("نوع الملف غير مدعوم.");
            }

            // convert bmp & tiff to webp to keep file size acceptable
            var targetExtension = originalExtension is ".bmp" or ".tiff" ? ".webp" : originalExtension;
            var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName);
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var fileName = $"{Guid.NewGuid()}{targetExtension}";
            var filePath = Path.Combine(uploadDirectory, fileName);

            if (targetExtension == ".gif" && originalExtension == ".gif")
            {
                await using var copyStream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(copyStream);
                return $"/{folderName}/{fileName}";
            }

            await using var stream = imageFile.OpenReadStream();
            using var image = await Image.LoadAsync(stream);
            var encoder = ResolveEncoder(targetExtension);
            await using var output = new FileStream(filePath, FileMode.Create);
            await image.SaveAsync(output, encoder);
            return $"/{folderName}/{fileName}";
        }

        private static IImageEncoder ResolveEncoder(string extension) =>
            extension switch
            {
                ".jpg" or ".jpeg" => new JpegEncoder { Quality = DefaultQuality, Interleaved = true },
                ".png" => new PngEncoder { CompressionLevel = PngCompressionLevel.Level6 },
                ".webp" => new WebpEncoder { Quality = DefaultQuality, FileFormat = WebpFileFormatType.Lossy },
                ".gif" => new GifEncoder { ColorTableMode = GifColorTableMode.Global },
                ".bmp" => new BmpEncoder(),
                _ => new JpegEncoder { Quality = DefaultQuality }
            };
    }
}


