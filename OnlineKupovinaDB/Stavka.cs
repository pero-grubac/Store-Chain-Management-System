namespace OnlineKupovinaDB
{
    public class Stavka
    {
        private Racun racun { get; set; }
        private Proizvod proizvod { get; set; }
        private int kolicina;
        private double cijena;
        private double ukupnaCijena;
        public Stavka(int idRaacun, DateTime datumKupovine, string imeProdavnice, string nazivProizvoda, int kolicina, double cijena)
        {
            this.racun = new Racun(idRaacun, datumKupovine, imeProdavnice);
            this.proizvod = new Proizvod(nazivProizvoda);
            this.kolicina = kolicina;
            this.cijena = cijena;
            this.ukupnaCijena = kolicina * cijena;
        }
        public Stavka(int idProizvoda, int idProdavnice, int kolicina, double cijena, DateTime datum)
        {
            proizvod = new Proizvod(idProizvoda);
            this.kolicina = kolicina;
            this.cijena = cijena;
            racun = new(idProdavnice, datum);
        }

        public int? IdRacun
        {
            get { return racun.IdRacun; }
            set { racun.IdRacun = value; }
        }
        public string ImeProdavnice
        {
            get { return racun.ImeProdavnice; }
        }
        public DateTime DatumKupovine
        {
            get { return racun.DatumKupovine; }
        }
        public string NazivProizvoda
        {
            get { return proizvod.Naziv; }
        }
        public int Kolicina
        {
            get { return kolicina; }
        }
        public double Cijena
        {
            get { return cijena; }
        }
        public double Ukupno
        {
            get { return ukupnaCijena; }
        }
    }
}
