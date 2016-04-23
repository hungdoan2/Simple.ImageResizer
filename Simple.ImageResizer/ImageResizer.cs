using System;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace Simple.ImageResizer
{
    public class ImageResizer : IDisposable
    {
        private byte[] _imageBytes;
        private readonly BitmapImage _orgBitMap;

        public ImageResizer(string path)
        {
            _imageBytes = LoadImageData(path);
            _orgBitMap = LoadBitmapImage(_imageBytes);
        }

        public ImageResizer(byte[] imageBytes)
        {
            _imageBytes = imageBytes;
            _orgBitMap = LoadBitmapImage(_imageBytes);
        }

        public int InputImageWidth
        {
            get { return _orgBitMap.PixelWidth; }
        }
        public int InputImageHeight
        {
            get { return _orgBitMap.PixelHeight; }
        }
        public byte[] Resize(int width, ImageEncoding encoding)
        {
            return Resize(width, 0, encoding);
        }

        public byte[] Resize(int width, ImageEncode encode)
        {
            return Resize(width, 0, encode);
        }

        public byte[] ResizeImageWidth(int width, ImageEncode encode)
        {
            //return Resize(width, 0, encoding);
            BitmapSource bitmapSource = null;

            if (width > 0)
            {
                bitmapSource = ResizeImageByWidth(_imageBytes, width);
            }

            _imageBytes = EncodeImageData(bitmapSource, encode);
            return _imageBytes;
        }

        public byte[] ResizeImageHeight(int height, ImageEncode encoding)
        {
            //return Resize(width, 0, encoding);
            BitmapSource bitmapSource = null;

            if (height > 0)
            {
                bitmapSource = ResizeImageByHeight(_imageBytes, height);
            }

            _imageBytes = EncodeImageData(bitmapSource, encoding);
            return _imageBytes;
        }

        /// <summary>
        /// Resizes with ScaleToFill
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public byte[] Resize(int width, int height, ImageEncoding encoding)
        {
            return Resize(width, height, true, encoding);
        }

        public byte[] Resize(int width, int height, ImageEncode encode)
        {
            return Resize(width, height, true, encode);
        }

        /// <summary>
        /// Resizes with ScaleToFit of crop is true
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="crop">if set to true ScaleToFill is used, if not ScaleToFit</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public byte[] Resize(int width, int height, bool crop, ImageEncoding encoding)
        {
            if (width < 0)
                throw new ArgumentException("width < 0");
            if (height < 0)
                throw new ArgumentException("height < 0");

            BitmapSource bitmapSource = null;

            if (width > 0 && height > 0 && crop)
            {
                bitmapSource = ScaleToFill(width, height);
            }
            else if (width > 0 && height > 0 && !crop)
            {
                bitmapSource = ScaleToFit(width, height);
            }
            else if (width > 0)
            {
                bitmapSource = ResizeImageByWidth(_imageBytes, width);
            }
            else if (height > 0)
            {
                bitmapSource = ResizeImageByHeight(_imageBytes, height);
            }

            _imageBytes = EncodeImageData(bitmapSource, encoding);
            return _imageBytes;
        }


        public byte[] Resize(int width, int height, bool crop, ImageEncode encode)
        {
            if (width < 0)
                throw new ArgumentException("width < 0");
            if (height < 0)
                throw new ArgumentException("height < 0");

            BitmapSource bitmapSource = null;

            if (width > 0 && height > 0 && crop)
            {
                bitmapSource = ScaleToFill(width, height);
            }
            else if (width > 0 && height > 0 && !crop)
            {
                bitmapSource = ScaleToFit(width, height);
            }
            else if (width > 0)
            {
                bitmapSource = ResizeImageByWidth(_imageBytes, width);
            }
            else if (height > 0)
            {
                bitmapSource = ResizeImageByHeight(_imageBytes, height);
            }

            _imageBytes = EncodeImageData(bitmapSource, encode);
            return _imageBytes;
        }

        private BitmapSource ScaleToFill(int width, int height)
        {
            Contract.Requires(width > 0);
            Contract.Requires(height > 0);

            double heightRatio = height / (double)_orgBitMap.PixelHeight;
            double widthRatio = width / (double)_orgBitMap.PixelWidth;

            BitmapSource bitmapSource;
            ImageSize imageSize;

            if (heightRatio > widthRatio)
            {
                bitmapSource = ResizeImageByHeight(_imageBytes, height);
                var calc = new ImageSizeCalculator(bitmapSource.PixelWidth, height);
                imageSize = calc.ScaleToFill(width, height);
            }
            else
            {
                bitmapSource = ResizeImageByWidth(_imageBytes, width);
                var calc = new ImageSizeCalculator(width, bitmapSource.PixelHeight);
                imageSize = calc.ScaleToFill(width, height);
            }

            var croppedBitmap = new CroppedBitmap(bitmapSource, new Int32Rect(imageSize.XOffset, imageSize.YOffset, imageSize.Width, imageSize.Height));
            return croppedBitmap;
        }

        private BitmapSource ScaleToFit(int width, int height)
        {
            Contract.Requires(width > 0);
            Contract.Requires(height > 0);

            double heightRatio = ((double)_orgBitMap.PixelHeight) / height;
            double widthRatio = ((double)_orgBitMap.PixelWidth) / width;

            if (heightRatio > widthRatio)
            {
                return ResizeImageByHeight(_imageBytes, height);
            }

            return ResizeImageByWidth(_imageBytes, width);
        }

        public void SaveToFile(string path)
        {
            SaveImageToFile(_imageBytes, path);
        }

        public void Dispose()
        {
            _imageBytes = null;
        }

        private BitmapSource ResizeImageByWidth(byte[] imageData, int width)
        {
            Contract.Requires(imageData != null);
            Contract.Requires(width > 0);

            var newBitmap = new BitmapImage();
            newBitmap.BeginInit();
            newBitmap.DecodePixelWidth = width;
            newBitmap.StreamSource = new MemoryStream(imageData);
            newBitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            newBitmap.CacheOption = BitmapCacheOption.Default;
            newBitmap.EndInit();
            return newBitmap;
        }

        private BitmapSource ResizeImageByHeight(byte[] imageData, int height)
        {
            Contract.Requires(imageData != null);
            Contract.Requires(height > 0);

            var newBitmap = new BitmapImage();
            newBitmap.BeginInit();
            newBitmap.DecodePixelHeight = height;
            newBitmap.StreamSource = new MemoryStream(imageData);
            newBitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            newBitmap.CacheOption = BitmapCacheOption.Default;
            newBitmap.EndInit();
            return newBitmap;
        }

        private byte[] EncodeImageData(ImageSource image, ImageEncoding encoding)
        {
            byte[] buffer = null;
            BitmapEncoder encoder = null;
            switch (encoding)
            {
                case ImageEncoding.Jpg100:
                    encoder = new JpegBitmapEncoder { QualityLevel = 100 };
                    break;

                case ImageEncoding.Jpg95:
                    encoder = new JpegBitmapEncoder { QualityLevel = 95 };
                    break;

                case ImageEncoding.Jpg90:
                    encoder = new JpegBitmapEncoder { QualityLevel = 90 };
                    break;

                case ImageEncoding.Jpg:
                    encoder = new JpegBitmapEncoder();
                    break;

                case ImageEncoding.Bmp:
                    encoder = new BmpBitmapEncoder();
                    break;

                case ImageEncoding.Png:
                    encoder = new PngBitmapEncoder();
                    break;

                case ImageEncoding.Tiff:
                    encoder = new TiffBitmapEncoder();
                    break;

                case ImageEncoding.Gif:
                    encoder = new GifBitmapEncoder();
                    break;

                case ImageEncoding.Wmp:
                    encoder = new WmpBitmapEncoder();
                    break;
            }
            if (image is BitmapSource)
            {
                using (var stream = new MemoryStream())
                {
                    if (encoder != null)
                    {
                        var bitmapFrame = BitmapFrame.Create(image as BitmapSource);
                        encoder.Frames.Add(bitmapFrame);
                        encoder.Save(stream);
                    }
                    stream.Seek(0L, SeekOrigin.Begin);
                    buffer = new byte[stream.Length];
                    using (var reader = new BinaryReader(stream))
                    {
                        reader.Read(buffer, 0, (int)stream.Length);
                        reader.Close();
                    }
                    stream.Close();
                }

            }
            return buffer;
        }


        private byte[] EncodeImageData(ImageSource image, ImageEncode encode)
        {
            byte[] buffer = null;
            BitmapEncoder encoder = null;
            encoder = encode.GetBitmapEncoder();

            if (image is BitmapSource)
            {
                using (var stream = new MemoryStream())
                {
                    if (encoder != null)
                    {
                        var bitmapFrame = BitmapFrame.Create(image as BitmapSource);
                        encoder.Frames.Add(bitmapFrame);
                        encoder.Save(stream);
                    }
                    stream.Seek(0L, SeekOrigin.Begin);
                    buffer = new byte[stream.Length];
                    using (var reader = new BinaryReader(stream))
                    {
                        reader.Read(buffer, 0, (int)stream.Length);
                        reader.Close();
                    }
                    stream.Close();
                }
            }
            return buffer;
        }

        private static byte[] LoadImageData(string filePath)
        {
            var input = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var reader = new BinaryReader(input);
            byte[] buffer = reader.ReadBytes((int)input.Length);
            reader.Close();
            input.Close();
            return buffer;
        }

        private BitmapImage LoadBitmapImage(byte[] bytes)
        {
            Contract.Requires(bytes != null);

            var newBitmap = new BitmapImage();
            newBitmap.BeginInit();
            newBitmap.StreamSource = new MemoryStream(bytes);
            newBitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            newBitmap.CacheOption = BitmapCacheOption.Default;
            newBitmap.EndInit();
            return newBitmap;
        }

        private void SaveImageToFile(byte[] bytes, string path)
        {
            Contract.Requires(bytes != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(path));

            var output = new FileStream(path, FileMode.Create, FileAccess.Write);
            var writer = new BinaryWriter(output);
            writer.Write(bytes);
            writer.Close();
            output.Close();
        }
    }
}
