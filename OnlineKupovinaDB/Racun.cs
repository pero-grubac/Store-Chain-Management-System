namespace OnlineKupovinaDB
{
    public class Racun
    {
        public const string Placen = "Placen";
        public const string Storniran = "Storniran";

        public int Id { get; set; }
        public DateTime Datum { get; set; }
        public int IdKupca { get; set; }
        public string Kupac { get; set; } = string.Empty;
        public int IdProdavnice { get; set; }
        public string Prodavnica { get; set; } = string.Empty;
        public string? Kupon { get; set; }
        public int BrojArtikala { get; set; }
        public decimal Ukupno { get; set; }
        public string Status { get; set; } = Placen;

        public bool JeStorniran => Status == Storniran;
        public string Broj => $"R-{Id:D6}";
    }
}
