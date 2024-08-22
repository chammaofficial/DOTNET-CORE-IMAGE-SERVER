using ImageMagick;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageServer
{
    public class ImageServerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _rootPath;
        private static int _CacheControlMaxTimeInSeconds = 10;
        private readonly Dictionary<string, MagickFormat> _supportedFormats = new Dictionary<string, MagickFormat>(StringComparer.OrdinalIgnoreCase)
        {
            { ".png", MagickFormat.Png },
            { ".jpg", MagickFormat.Jpeg },
            { ".jpeg", MagickFormat.Jpeg },
            { ".gif", MagickFormat.Gif },
            { ".tiff", MagickFormat.Tiff },
            { ".ico", MagickFormat.Ico },
            { ".bmp", MagickFormat.Bmp },
        };

        public ImageServerMiddleware(RequestDelegate next, string rootPath , int CacheControlMaxTimeInSeconds = 10)
        {
            _CacheControlMaxTimeInSeconds = CacheControlMaxTimeInSeconds;
            _next = next;
            _rootPath = Path.Combine(rootPath, "wwwroot");
        }


        public async Task Invoke(HttpContext context)
        {
            string imagePath = context.Request.Path.Value;
            string fileExtension = Path.GetExtension(imagePath);

            if (!string.IsNullOrEmpty(fileExtension) && _supportedFormats.ContainsKey(fileExtension))
            {
                try
                {
                    string physicalPath = Path.Combine(_rootPath, imagePath.TrimStart('/'));

                    if (!File.Exists(physicalPath))
                        await _next(context);

                    using Stream file = new FileStream(physicalPath, FileMode.Open, FileAccess.Read);
                    using (MagickImageCollection images = new MagickImageCollection(file))
                    {
                        foreach (IMagickImage<ushort> image in images)
                        {
                            SetQuality(context, image);
                            SetDefines(context, image);
                            SetCompression(context, image);
                            SetFormat(context, image);
                            SetSize(context, image);
                            SetRotation(context, image);
                        }

                        using (MemoryStream outputStream = new MemoryStream())
                        {
                            //images.Write(outputStream, _supportedFormats[fileExtension]);
                            //context.Response.ContentType = GetMimeType(_supportedFormats[fileExtension]);
                            images.Write(outputStream, MagickFormat.WebP);
                            context.Response.ContentType = GetMimeType(MagickFormat.WebP);
                            context.Response.Headers.Add("Content-Disposition", "inline"); // Set to 'inline' to display in browser
                            context.Response.Headers.Add("Cache-Control", $"public, max-age={_CacheControlMaxTimeInSeconds}"); // Set cache to 10s
                            await context.Response.Body.WriteAsync(outputStream.ToArray());
                        }
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Error logging
                }
            }
            else
            {
                // File extension not supported, pass through to the next middleware
                await _next(context);
            }
        }

        private void SetSize(HttpContext context, IMagickImage<ushort> image)
        {
            var height = String.IsNullOrEmpty(context.Request.Query["h"]) ? 0 : Convert.ToInt32(context.Request.Query["h"]);
            var width = String.IsNullOrEmpty(context.Request.Query["w"]) ? 0 : Convert.ToInt32(context.Request.Query["w"]);

            if(height == 0 && width == 0)
                return;
            else
                image.Resize(width, height);
        }

        private void SetFormat(HttpContext context, IMagickImage<ushort> image)
        {
            image.Format = MagickFormat.WebP;
        }

        private void SetCompression(HttpContext context, IMagickImage<ushort> image)
        {
            image.Settings.Compression = CompressionMethod.WebP;
        }

        private void SetDefines(HttpContext context, IMagickImage<ushort> image)
        {
            // TODO
            //image.Settings.SetDefines(new WebPWriteDefines() { Lossless = true, Method = 6 });
        }

        private void SetQuality(HttpContext context, IMagickImage<ushort> image)
        {
            var quality = String.IsNullOrEmpty(context.Request.Query["q"]) ? 50 : Convert.ToInt32(context.Request.Query["q"]);
            image.Quality = quality;
        }

        private void SetRotation(HttpContext context, IMagickImage<ushort> image)
        {
            var rotation = String.IsNullOrEmpty(context.Request.Query["r"]) ? 0 : Convert.ToInt32(context.Request.Query["r"]);
            image.Rotate(rotation);
        }

        
        private string GetMimeType(MagickFormat format)
        {
            return format switch
            {
                MagickFormat.Png => "image/png",
                MagickFormat.Jpg or MagickFormat.Jpeg => "image/jpeg",
                MagickFormat.Gif => "image/gif",
                MagickFormat.Tif or MagickFormat.Tiff => "image/tiff",
                MagickFormat.Ico => "image/x-icon",
                MagickFormat.Bmp => "image/bmp",
                _ => string.Empty
            };
        }


    }
}
