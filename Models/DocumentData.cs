using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace KP_KW_Generator.Models
{
    public class DocumentData
    {
        public string? Typ { get; set; } = "KP";
        public DateTime Data { get; set; } = DateTime.Today;
        public string OdKogo { get; set; } = "";
        public List<Pozycja> Pozycje { get; set; } = new List<Pozycja>();
        public decimal Suma => Pozycje?.Sum(p => p.Kwota) ?? decimal.Zero;
        public string Slownie => Helpers.NumberToWords.KwotaSlownie(Suma);
        public bool Pojedyncze {  get; set; }
    }

    public class Pozycja : INotifyPropertyChanged
    {
        private string opis = "";
        public string Opis
        {
            get => opis;
            set { opis = value; OnPropertyChanged(nameof(Opis)); }
        }

        private decimal? kwota = decimal.Zero;
        public decimal? Kwota
        {
            get => kwota;
            set { kwota = value; OnPropertyChanged(nameof(Kwota)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
