namespace OnlineKupovinaDB
{
    public class DodavanjeProizvoda
    {
        private int idZaposleni { get; set; }
        private int idProizvod { get; set; }
        private DateTime datum { get; set; }
        public DodavanjeProizvoda(int idZaposleni, int idProizvod, DateTime datum)
        {
            this.idZaposleni = idZaposleni;
            this.idProizvod = idProizvod;
            this.datum = datum;
        }
    }
}
