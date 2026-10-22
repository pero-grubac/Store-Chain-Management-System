namespace OnlineKupovinaDB
{
    /// <summary>Stavka sacuvanog racuna. Cijena je konacna jedinicna cijena nakon popusta.</summary>
    public class Stavka
    {
        public int IdRacuna { get; set; }
        public int IdProizvoda { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string BarKod { get; set; } = string.Empty;
        public int Kolicina { get; set; }
        public decimal PunaCijena { get; set; }
        public int Popust { get; set; }
        public decimal Cijena { get; set; }

        public decimal Ukupno => Kolicina * Cijena;
        public decimal Usteda => Kolicina * (PunaCijena - Cijena);
    }
}
