using KP_KW_Generator.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KP_KW_Generator.Helpers
{
    public static class ImageGenerator
    {
        public static void SaveWithTemplate(DocumentData d, string outputPath)
        {
            const int dpi = 96;

            // A4 w DIP
            int width = (int)(8.27 * dpi);   // 794
            int height = (int)(11.69 * dpi); // 1123

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string templatePath = Path.Combine(baseDir, "Templates", $"{d.Typ}_Template.png");

            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                // Tło
                var background = new BitmapImage(new Uri(templatePath));
                dc.DrawImage(background, new Rect(0, 0, width, height));

                Typeface font = new Typeface("Arial");
                Brush brush = Brushes.Black;

                const float COPY_OFFSET_Y = -564f;

                DrawCopy(0);
                DrawCopy(COPY_OFFSET_Y);

                void DrawCopy(float offsetY)
                {
                    DrawTextPdf(dc, d.OdKogo, 177, 950 + offsetY, 12);
                    DrawTextPdf(dc, d.Data.ToString("dd.MM.yyyy"), 365, 985 + offsetY, 12);

                    float startY = 905 + offsetY;
                    foreach (var p in d.Pozycje)
                    {
                        DrawTextPdf(dc, p.Opis, 111, startY, 12);
                        DrawTextPdf(dc, $"{p.Kwota:0.00}", 510, startY, 12);
                        startY -= 26;
                    }

                    DrawTextPdf(dc, $"{d.Suma:0.00} zł", 492, 774 + offsetY, 12);
                    DrawTextPdf(dc, d.Slownie, 167, 744 + offsetY, 9);
                }

                void DrawTextPdf(
                    DrawingContext ctx,
                    string text,
                    double xPdf,
                    double yPdf,
                    double pdfFontPt)
                {
                    // PDF (0,0) = dół → WPF (0,0) = góra
                    double yWpf = height - yPdf;

                    double fontDip = pdfFontPt * 96.0 / 72.0;

                    var formatted = new FormattedText(
                        text,
                        System.Globalization.CultureInfo.GetCultureInfo("pl-PL"),
                        FlowDirection.LeftToRight,
                        font,
                        fontDip,
                        brush,
                        1.0);

                    ctx.DrawText(formatted, new Point(xPdf, yWpf));
                }
            }

            var bitmap = new RenderTargetBitmap(width, height, dpi, dpi, PixelFormats.Pbgra32);
            bitmap.Render(visual);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using var fs = new FileStream(outputPath, FileMode.Create);
            encoder.Save(fs);
        }
    }
}
//By Jędrzej Kantor
