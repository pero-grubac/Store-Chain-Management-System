namespace OnlineKupovinaDB
{
    public class Kpi
    {
        public decimal Prihod { get; set; }
        public int BrojRacuna { get; set; }
        public decimal ProsjecnaKorpa { get; set; }
        public int BrojKupaca { get; set; }
        public int NiskeZalihe { get; set; }
    }

    public class PrihodProdavnice
    {
        public int IdProdavnice { get; set; }
        public string Prodavnica { get; set; } = string.Empty;
        public int BrojRacuna { get; set; }
        public decimal Prihod { get; set; }
    }

    public class PrihodDana
    {
        public DateTime Dan { get; set; }
        public decimal Prihod { get; set; }
        public int BrojRacuna { get; set; }
    }

    public class NajprodavanijiProizvod
    {
        public int Rang { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string BarKod { get; set; } = string.Empty;
        public int Kolicina { get; set; }
        public decimal Prihod { get; set; }
        public int BrojProdavnica { get; set; }
    }

    /// <summary>Red iz zalihe_istorija (puni ga trigger na proizvodu).</summary>
    public class PromjenaZaliha
    {
        public int Id { get; set; }
        public DateTime Vrijeme { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string BarKod { get; set; } = string.Empty;
        public string Prodavnica { get; set; } = string.Empty;
        public int StaraKolicina { get; set; }
        public int NovaKolicina { get; set; }
        public int Promjena { get; set; }
        public string Razlog { get; set; } = string.Empty;
        public string? Korisnik { get; set; }
    }
}
