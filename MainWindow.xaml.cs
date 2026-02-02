using iText.Kernel.Pdf;
using KP_KW_Generator.Helpers;
using KP_KW_Generator.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KP_KW_Generator
{
    public partial class MainWindow : Window
    {
        ObservableCollection<Pozycja> pozycje;

        public MainWindow()
        {
            InitializeComponent();
            TypBox.SelectedIndex = 0;
            DataPicker.SelectedDate = DateTime.Today;

            pozycje = new ObservableCollection<Pozycja>();
            pozycje.CollectionChanged += Pozycje_CollectionChanged;
            PozycjeGrid.ItemsSource = pozycje;
        }
        private void Pozycje_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Pozycja p in e.NewItems)
                    p.PropertyChanged += Pozycja_PropertyChanged;
            }

            if (e.OldItems != null)
            {
                foreach (Pozycja p in e.OldItems)
                    p.PropertyChanged -= Pozycja_PropertyChanged;
            }

            PrzeliczSume();
            AktualizujMozliwoscDodaniaWiersza();
        }


        private void Pozycja_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Pozycja.Kwota))
                PrzeliczSume();
        }


        private void PrzeliczSume()
        {
            decimal suma = pozycje.Sum(p => p.Kwota ?? decimal.Zero);
            TxtSuma.Text = suma.ToString("0.00");
            TxtSlownie.Text = Helpers.NumberToWords.KwotaSlownie(suma);
        }

        private DocumentData UzbierajDane()
        {
            return new DocumentData
            {
                Typ = ((ComboBoxItem)TypBox.SelectedItem).Content.ToString() ?? "KP",
                Data = DataPicker.SelectedDate ?? DateTime.Today,
                OdKogo = OdKogoBox.Text,
                Pozycje = pozycje.ToList(),
                Pojedyncze = RbPojedyncze.IsChecked == true
            };
        }

        private void GenerujPDF_Click(object sender, RoutedEventArgs e)
        {
            var data = UzbierajDane();
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"{data.Typ}_{data.OdKogo}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            string path = Path.Combine(desktopPath, fileName);
            PdfGenerator.SaveWithTemplate(data, path);
            MessageBox.Show($"PDF wygenerowany: {path}");
        }

        private void Wyczysc_Click(object sender, RoutedEventArgs e)
        {
            OdKogoBox.Text = "";
            DataPicker.Text = "";
            DataPicker.SelectedDate = DateTime.Today;
            pozycje.Clear();

        }

        private void Drukuj_Click(object sender, RoutedEventArgs e)
        {
            var data = UzbierajDane();
            string tempPath = Path.Combine(Path.GetTempPath(), $"KP_KW_{DateTime.Now:yyyyMMddHHmmss}.png");

            ImageGenerator.SaveWithTemplate(data, tempPath);

            PrintDialog dlg = new PrintDialog();
            if (dlg.ShowDialog() == true)
            {
                BitmapImage img = new BitmapImage(new Uri(tempPath));

                var visual = new DrawingVisual();
                using (var dc = visual.RenderOpen())
                {
                    dc.DrawImage(img, new Rect(0, 0, dlg.PrintableAreaWidth, dlg.PrintableAreaHeight));
                }

                dlg.PrintVisual(visual, "Druk KP/KW");
            }
        }



        private void TypBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string typ = ((ComboBoxItem)TypBox.SelectedItem).Content.ToString();
            Title = typ == "KP" ? "KP – Kasa Przyjmie" : "KW – Kasa Wyda";
            Komu.Text = typ == "KP" ? "Od kogo?" : "Komu?";
            if (typ == "KP")
            {
                RbPodwojne.IsChecked = true;
            }
            else
            {
                RbPojedyncze.IsChecked = true;
            }
        }

        private void AktualizujMozliwoscDodaniaWiersza()
        {
            PozycjeGrid.CanUserAddRows = pozycje.Count < 5;
        }

    }
}
