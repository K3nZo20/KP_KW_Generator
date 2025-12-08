using System;

namespace KP_KW_Generator.Helpers
{
    public static class NumberToWords
    {
        private static readonly string[] jednostki =
        { "", "jeden", "dwa", "trzy", "cztery", "pięć", "sześć", "siedem", "osiem", "dziewięć" };

        private static readonly string[] nastki =
        { "dziesięć", "jedenaście", "dwanaście", "trzynaście", "czternaście",
          "piętnaście", "szesnaście", "siedemnaście", "osiemnaście", "dziewiętnaście" };

        private static readonly string[] dziesiatki =
        { "", "", "dwadzieścia", "trzydzieści", "czterdzieści",
          "pięćdziesiąt", "sześćdziesiąt", "siedemdziesiąt", "osiemdziesiąt", "dziewięćdziesiąt" };

        private static readonly string[] setki =
        { "", "sto", "dwieście", "trzysta", "czterysta",
          "pięćset", "sześćset", "siedemset", "osiemset", "dziewięćset" };

        public static string KwotaSlownie(decimal kwota)
        {
            long zlotych = (long)Math.Floor(kwota);
            long groszy = (long)((kwota - zlotych) * 100);

            string wynik = $"{LiczbaSlownie(zlotych)} {Waluta(zlotych)}";

            if (groszy > 0)
                wynik += $" {LiczbaSlownie(groszy)} {Grosze(groszy)}";

            return wynik;
        }

        public static string LiczbaSlownie(long n)
        {
            if (n == 0) return "zero";

            long tys = n / 1000;
            long reszta = n % 1000;

            string wynik = "";

            if (tys > 0)
            {
                wynik += $"{Trojki(tys)} {Tysiace(tys)} ";
            }

            wynik += Trojki(reszta);

            return wynik.Trim();
        }

        private static string Trojki(long n)
        {
            long s = n / 100;
            long dz = (n % 100) / 10;
            long j = n % 10;

            string wynik = setki[s] + " ";

            if (dz == 1) // nastki
            {
                wynik += nastki[j];
            }
            else
            {
                wynik += dziesiatki[dz] + " " + jednostki[j];
            }

            return wynik.Trim();
        }

        private static string Tysiace(long n)
        {
            if (n == 1) return "tysiąc";
            if (n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 > 20)) return "tysiące";
            return "tysięcy";
        }

        private static string Waluta(long n)
        {
            if (n == 1) return "złoty";
            if (n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 > 20)) return "złote";
            return "złotych";
        }
        private static string Grosze(long n)
        {
            if (n == 1) return "grosz";
            if (n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 > 20)) return "grosze";
            return "groszy";
        }
    }
}
