using System.Text.RegularExpressions;

namespace OnlineKupovinaDB
{
    /// <summary>
    /// Bar kodovi: format koji baza prihvata, GTIN kontrolna cifra (EAN-8, UPC-A, EAN-13, GTIN-14)
    /// i interni EAN-13 za robu bez bar koda (prefiks 20-29 je rezervisan za internu upotrebu).
    /// Skeneri salju kod kao tastatura, pa ulaz moze imati razmake ili znak za novi red.
    /// </summary>
    public static partial class Barkod
    {
        public const int MaxDuzina = 45;

        public static string Normalizuj(string? unos)
        {
            return unos == null ? string.Empty : new string(unos.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }

        public static bool JeDozvoljenFormat(string? barkod)
        {
            string kod = Normalizuj(barkod);
            return kod.Length is >= 4 and <= MaxDuzina && FormatRegex().IsMatch(kod);
        }

        public static bool JeGtin(string? barkod)
        {
            string kod = Normalizuj(barkod);
            return kod.Length is 8 or 12 or 13 or 14 && kod.All(char.IsAsciiDigit);
        }

        /// <summary>Provjera kontrolne cifre za GTIN kodove (zadnja cifra).</summary>
        public static bool ImaIspravnuKontrolnuCifru(string? barkod)
        {
            string kod = Normalizuj(barkod);
            if (!JeGtin(kod)) return false;
            return KontrolnaCifra(kod[..^1]) == kod[^1] - '0';
        }

        /// <summary>Kontrolna cifra za GTIN bez zadnje cifre (tezine 3 i 1 s desna).</summary>
        public static int KontrolnaCifra(string bezKontrolne)
        {
            int suma = 0;
            for (int i = 0; i < bezKontrolne.Length; i++)
            {
                int cifra = bezKontrolne[bezKontrolne.Length - 1 - i] - '0';
                suma += i % 2 == 0 ? cifra * 3 : cifra;
            }
            return (10 - suma % 10) % 10;
        }

        /// <summary>Novi interni EAN-13 sa prefiksom 20 i ispravnom kontrolnom cifrom.</summary>
        public static string GenerisiInterni(Random? random = null)
        {
            random ??= Random.Shared;
            string osnova = "20" + random.NextInt64(0, 10_000_000_000).ToString("D10");
            return osnova + KontrolnaCifra(osnova);
        }

        [GeneratedRegex("^[0-9A-Za-z-]+$")]
        private static partial Regex FormatRegex();
    }
}
