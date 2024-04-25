using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using Color = System.Drawing.Color;

namespace UIComponents.Generators.Helpers
{
    public static class CreateThumbnailFromImage
    {

        //set the resolution, 72 is usually good enough for displaying images on monitors
        private static float imageResolution = 72;

        //set the compression level. higher compression = better quality = bigger images
        private static long compressionLevel = 80L;

        public static string Create(string imagePath, int maxWidth, int maxHeight,bool padImage = false)
        {
            if (!File.Exists(imagePath))
                return "";
            var image = System.Drawing.Image.FromFile(imagePath);
            try
            {

                int newWidth;
                int newHeight;

                //first we check if the image needs rotating (eg phone held vertical when taking a picture for example)
                foreach (var prop in image.PropertyItems)
                {
                    if (prop.Id == 0x0112)
                    {
                        int orientationValue = image.GetPropertyItem(prop.Id).Value[0];
                        var rotateFlipType = getRotateFlipType(orientationValue);
                        image.RotateFlip(rotateFlipType);
                        break;
                    }
                }

                //apply the padding to make a square image
                if (padImage == true)
                {
                    image = applyPaddingToImage(image, System.Drawing.Color.Black);
                }

                //check if the with or height of the image exceeds the maximum specified, if so calculate the new dimensions
                if (image.Width > maxWidth || image.Height > maxHeight)
                {
                    var ratioX = (double)maxWidth / image.Width;
                    var ratioY = (double)maxHeight / image.Height;
                    var ratio = Math.Min(ratioX, ratioY);

                    newWidth = (int)(image.Width * ratio);
                    newHeight = (int)(image.Height * ratio);
                }
                else
                {
                    newWidth = image.Width;
                    newHeight = image.Height;
                }

                //start the resize with a new image
                var newImage = new Bitmap(newWidth, newHeight);

                //set the new resolution
                newImage.SetResolution(imageResolution, imageResolution);

                //start the resizing
                using (var graphics = Graphics.FromImage(newImage))
                {
                    //set some encoding specs
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                }

                //save the image to a memorystream to apply the compression level
                using (var ms = new MemoryStream())
                {
                    var encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, compressionLevel);

                    newImage.Save(ms, ImageFormat.Png);
                    ms.Position = 0;
                    var array = ms.ToArray();
                    var result = Convert.ToBase64String(array);
                    return result;
                }
            }
            finally
            {
                image.Dispose();
            }
        }


        //=== image padding
        private static Image applyPaddingToImage(Image image, System.Drawing.Color backColor)
        {
            //get the maximum size of the image dimensions
            var maxSize = Math.Max(image.Height, image.Width);
            var squareSize = new Size(maxSize, maxSize);

            //create a new square image
            var squareImage = new Bitmap(squareSize.Width, squareSize.Height);

            using (var graphics = Graphics.FromImage(squareImage))
            {
                //fill the new square with a color
                graphics.FillRectangle(new SolidBrush(backColor), 0, 0, squareSize.Width, squareSize.Height);

                //put the original image on top of the new square
                graphics.DrawImage(image, squareSize.Width / 2 - image.Width / 2, squareSize.Height / 2 - image.Height / 2, image.Width, image.Height);
            }

            //return the image
            return squareImage;
        }


        //=== get encoder info
        private static ImageCodecInfo getEncoderInfo(string mimeType)
        {
            var encoders = ImageCodecInfo.GetImageEncoders();

            for (var j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType.ToLower() == mimeType.ToLower())
                {
                    return encoders[j];
                }
            }

            return null;
        }


        //=== determine image rotation
        private static RotateFlipType getRotateFlipType(int rotateValue)
        {
            var flipType = RotateFlipType.RotateNoneFlipNone;

            switch (rotateValue)
            {
                case 1:
                    flipType = RotateFlipType.RotateNoneFlipNone;
                    break;
                case 2:
                    flipType = RotateFlipType.RotateNoneFlipX;
                    break;
                case 3:
                    flipType = RotateFlipType.Rotate180FlipNone;
                    break;
                case 4:
                    flipType = RotateFlipType.Rotate180FlipX;
                    break;
                case 5:
                    flipType = RotateFlipType.Rotate90FlipX;
                    break;
                case 6:
                    flipType = RotateFlipType.Rotate90FlipNone;
                    break;
                case 7:
                    flipType = RotateFlipType.Rotate270FlipX;
                    break;
                case 8:
                    flipType = RotateFlipType.Rotate270FlipNone;
                    break;
                default:
                    flipType = RotateFlipType.RotateNoneFlipNone;
                    break;
            }

            return flipType;
        }


        //== convert image to base64
        private static string convertImageToBase64(Image image)
        {
            using (var ms = new MemoryStream())
            {
                //convert the image to byte array
                image.Save(ms, ImageFormat.Jpeg);
                var bin = ms.ToArray();

                //convert byte array to base64 string
                return Convert.ToBase64String(bin);
            }
        }
    }
}
