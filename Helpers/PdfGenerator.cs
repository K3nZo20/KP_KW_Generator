using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using KP_KW_Generator.Models;
using System;
using System.IO;
using static iText.Kernel.Font.PdfFontFactory;

namespace KP_KW_Generator.Helpers
{
    public static class PdfGenerator
    {
        public static void SaveWithTemplate(DocumentData d, string outputPath)
        {
            // Wybór szablonu
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string templatePath = Path.Combine(baseDir, "Templates", d.Typ + "_Template.pdf");
            string fontPath = Path.Combine(baseDir, "Fonts", "LiberationSans-Regular.ttf");

            PdfDocument pdf = new PdfDocument(
            new PdfReader(templatePath),
            new PdfWriter(outputPath)
            );

            Document doc = new Document(pdf);

            PdfPage page = pdf.GetFirstPage();
            PdfCanvas canvas = new PdfCanvas(page);

            PdfFont font = PdfFontFactory.CreateFont(
                fontPath,
                PdfEncodings.IDENTITY_H,
                EmbeddingStrategy.PREFER_EMBEDDED
            );

            const float COPY_OFFSET_Y = -423f;

            DrawCopy(0);               // górny druk
            DrawCopy(COPY_OFFSET_Y);   // dolny druk

            doc.Close();

            void DrawCopy(float offsetY)
            {
                // Od kogo / Komu + Data
                canvas.BeginText()
                    .SetFontAndSize(font, 12)
                    .MoveText(130, 700 + offsetY)
                    .ShowText(d.OdKogo)
                    .MoveText(145, 28)
                    .ShowText(d.Data.ToString("dd.MM.yyyy"))
                    .EndText();

                // Pozycje
                float startY = 667 + offsetY;
                foreach (var p in d.Pozycje)
                {
                    canvas.BeginText()
                        .SetFontAndSize(font, 12)
                        .MoveText(83, startY)
                        .ShowText(p.Opis)
                        .MoveText(300, 0)
                        .ShowText($"{p.Kwota:0.00}")
                        .EndText();

                    startY -= 19;
                }

                // Suma
                canvas.BeginText()
                    .SetFontAndSize(font, 12)
                    .MoveText(371, 572 + offsetY)
                    .ShowText($"{d.Suma:0.00} zł")
                    .EndText();

                // Słownie
                canvas.BeginText()
                    .SetFontAndSize(font, 9)
                    .MoveText(125, 551 + offsetY)
                    .ShowText(d.Slownie)
                    .EndText();
            }



        }
    }
}
//By Jędrzej Kantor
