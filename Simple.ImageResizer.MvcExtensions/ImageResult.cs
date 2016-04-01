using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Windows.Media.Imaging;

namespace Simple.ImageResizer.MvcExtensions
{
    public class ImageResult : FilePathResult
    {
        private readonly string _filePath;
        private readonly int _width;
        private readonly int _height;
        private readonly int _quality;

        public ImageResult(string filePath, int width = 0, int height = 0, int quality=100) :
            base(filePath, string.Format("image/{0}",
                filePath.FileExtensionForContentType()))
        {
            _filePath = filePath;
            _width = width;
            _height = height;
            _quality = quality;
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            string resizedFilePath = GetResizedImagePath(_filePath, _width, _height, _quality);
            response.SetDefaultImageHeaders(resizedFilePath);
            WriteFileToResponse(resizedFilePath, response);
        }

        private static void WriteFileToResponse(string filePath, HttpResponseBase response)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                const int bufferLength = 65536;
                var buffer = new byte[bufferLength];

                while (true)
                {
                    int bytesRead = fs.Read(buffer, 0, bufferLength);

                    if (bytesRead == 0)
                    {
                        break;
                    }

                    response.OutputStream.Write(buffer, 0, bytesRead);
                }
            }
        }

        private static string GetResizedImagePath(string filepath, int width, int height, int quality)
        {
            string resizedPath = filepath;

            if (width > 0 || height > 0)
            {
                resizedPath = filepath.GetPathForResizedImage(width, height, quality);

                if (!Directory.Exists(resizedPath))
                    Directory.CreateDirectory(new FileInfo(resizedPath).DirectoryName);

                if (!File.Exists(resizedPath))
                {
                    var encoder =  new JpegImageEncode { QualityLevel = quality };

                    var imageResizer = new ImageResizer(filepath);
                    if (width > 0 && height > 0)
                    {
                        imageResizer.Resize(width, height, encoder);
                    }
                    else if (width > 0)
                    {
                        imageResizer.Resize(width, encoder);
                    }
                    imageResizer.SaveToFile(resizedPath);
                    imageResizer.Dispose();
                }
            }
            return resizedPath;
        }
    }
}
