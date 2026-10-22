using System.Text.RegularExpressions;

namespace OnlineKupovinaDB
{
    public class Kupon
    {
        public int Id { get; set; }
        public string Sifra { get; set; } = string.Empty;
        public int Popust { get; set; }
        public DateTime Od { get; set; }
        public DateTime? Do { get; set; }
        public string Status { get; set; } = string.Empty;
        public int BrojProizvoda { get; set; }
        public string Proizvodi { get; set; } = string.Empty;

        public static string NormalizujSifru(string? sifra) => (sifra ?? string.Empty).Trim().ToUpperInvariant();

        public List<string> Validiraj()
        {
            var greske = new List<string>();
            if (!Regex.IsMatch(NormalizujSifru(Sifra), "^[A-Z0-9_-]{3,20}$"))
            {
                greske.Add("Šifra kupona: 3-20 slova, cifara, crtica ili donjih crta.");
            }
            Validacija.Raspon(greske, Popust, 1, 90, "Popust (%)");
            if (Do.HasValue && Do.Value.Date < Od.Date) greske.Add("Kupon ne može isteći prije početka.");
            return greske;
        }
    }

    /// <summary>Akcija: popust na proizvod u periodu (tabela popust).</summary>
    public class Akcija
    {
        public int Id { get; set; }
        public int IdProizvoda { get; set; }
        public string Proizvod { get; set; } = string.Empty;
        public string Prodavnica { get; set; } = string.Empty;
        public int Popust { get; set; }
        public DateTime Od { get; set; }
        public DateTime Do { get; set; }
        public string Status { get; set; } = string.Empty;

        public List<string> Validiraj()
        {
            var greske = new List<string>();
            Validacija.Raspon(greske, Popust, 1, 90, "Popust (%)");
            if (IdProizvoda <= 0) greske.Add("Izaberite proizvod.");
            if (Do.Date < Od.Date) greske.Add("Akcija ne može završiti prije početka.");
            return greske;
        }
    }
}
