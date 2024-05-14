using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Generators.Helpers
{
    public class CreateThumbnailFromPdf
    {
        public static void CreateThumbnail(Stream saveToStream, string pdfPath, int width = 297, int height = 210)
        {
            if (!File.Exists(pdfPath) || true)
            {
                return;
            }
            //using var pdfDocument = new PdfDocument(pdfPath);
            //var firstPage = pdfDocument.Pages[0];

            //using var pageBitmap = new PDFiumBitmap(width, height, true);

            //firstPage.Render(pageBitmap);

            //var image = Image.FromStream(pageBitmap.AsBmpStream());


            //image.Save(saveToStream, ImageFormat.Png);
            
        }
    }
}
