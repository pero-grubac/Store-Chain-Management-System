namespace OnlineKupovinaDB
{
    public class Proizvod
    {
        public int Id { get; set; }
        public int IdProdavnice { get; set; }
        public string Prodavnica { get; set; } = string.Empty;
        public string Naziv { get; set; } = string.Empty;
        public string BarKod { get; set; } = string.Empty;
        public string? Opis { get; set; }
        public decimal Cijena { get; set; }

        /// <summary>Aktivna akcija u procentima (racuna baza).</summary>
        public int Popust { get; set; }
        public decimal CijenaSaPopustom { get; set; }
        public int Zalihe { get; set; }
        public int MinZalihe { get; set; } = 10;

        /// <summary>OK, Nisko ili Nema (fn_status_zaliha).</summary>
        public string StatusZaliha { get; set; } = "OK";
        public DateTime? DatumIsteka { get; set; }
        public int IdProizvodjaca { get; set; }
        public string Proizvodjac { get; set; } = string.Empty;
        public string DrzavaPorijekla { get; set; } = string.Empty;
        public bool Aktivan { get; set; } = true;

        public bool NaAkciji => Popust > 0;

        public static string Status(int zalihe, int minZalihe) =>
            zalihe <= 0 ? "Nema" : zalihe <= minZalihe ? "Nisko" : "OK";

        public List<string> Validiraj(bool noviProizvod = false)
        {
            var greske = new List<string>();
            Validacija.Obavezno(greske, Naziv, "Naziv", 45);
            Validacija.Opciono(greske, Opis, "Opis", 255);
            if (!Barkod.JeDozvoljenFormat(BarKod))
            {
                greske.Add("Bar kod mora imati 4-45 znakova: cifre, slova ili crtica.");
            }
            else if (Barkod.JeGtin(BarKod) && !Barkod.ImaIspravnuKontrolnuCifru(BarKod))
            {
                greske.Add("Bar kod nema ispravnu kontrolnu cifru (EAN/UPC).");
            }
            Validacija.Raspon(greske, Cijena, 0.01m, 99_999_999m, "Cijena");
            if (Zalihe < 0) greske.Add("Zalihe ne mogu biti negativne.");
            if (MinZalihe < 0) greske.Add("Minimalne zalihe ne mogu biti negativne.");
            if (IdProizvodjaca <= 0) greske.Add("Izaberite proizvođača.");
            if (noviProizvod && DatumIsteka.HasValue && DatumIsteka.Value.Date < DateTime.Today)
            {
                greske.Add("Rok trajanja je već istekao.");
            }
            return greske;
        }

        public override string ToString() => $"{Naziv} ({BarKod})";
    }
}
