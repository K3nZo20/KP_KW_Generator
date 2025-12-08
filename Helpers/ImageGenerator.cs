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
            const int dpi = 300;

            // A4 portrait
            int width = (int)(8.27 * dpi);
            int height = (int)(11.69 * dpi);

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string templatePath = Path.Combine(baseDir, "Templates", $"{d.Typ}_Template.png");

            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                // Tło – odpowiednik PDF template
                var background = new BitmapImage(new Uri(templatePath));
                dc.DrawImage(background, new Rect(0, 0, width, height));

                Typeface font = new Typeface("Arial");
                Brush brush = Brushes.Black;

                //
                // ——— POLA TAK JAK W PDF-GENERATOR ———
                //

                // Od kogo / Komu
                DrawText(dc, d.OdKogo, 130, height - 700, 12, font, brush);

                // Data
                DrawText(dc, d.Data.ToString("dd.MM.yyyy"), 275, height - 728, 12, font, brush);

                //
                // Pozycje
                //
                float startY = 667;
                foreach (var p in d.Pozycje)
                {
                    // opis
                    DrawText(dc, p.Opis, 83, height - startY, 12, font, brush);

                    // kwota
                    DrawText(dc, $"{p.Kwota:0.00}", 383, height - startY, 12, font, brush);

                    startY -= 19;
                }

                // Suma
                DrawText(dc, $"{d.Suma:0.00} zł", 371, height - 572, 12, font, brush);

                // Słownie
                DrawText(dc, d.Slownie, 125, height - 551, 9, font, brush);
            }

            // Render
            var bitmap = new RenderTargetBitmap(width, height, dpi, dpi, PixelFormats.Pbgra32);
            bitmap.Render(visual);

            // Zapis PNG
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var fs = new FileStream(outputPath, FileMode.Create))
                encoder.Save(fs);
        }

        private static void DrawText(DrawingContext dc, string text, double x, double y,
            int size, Typeface font, Brush brush)
        {
            var formatted = new FormattedText(
                text,
                System.Globalization.CultureInfo.GetCultureInfo("pl-PL"),
                FlowDirection.LeftToRight,
                font,
                size,
                brush,
                1.0);

            dc.DrawText(formatted, new Point(x, y));
        }
    }
}
