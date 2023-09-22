namespace OnlineKupovinaDB
{
    public class Racun
    {
        private int? idRacun { get; set; }
        private DateTime datumKupovine { get; set; }

        private Prodavnica prodavnica { get; set; }
        public Racun(int? idRacun, DateTime datumKupovine, int idProdavnica)
        {
            this.idRacun = idRacun;
            this.datumKupovine = datumKupovine;

            this.prodavnica = new Prodavnica(idProdavnica);
        }
        public Racun(int idProdavnica, DateTime datumKupovine)
        {

            prodavnica = new(idProdavnica);
            this.datumKupovine = datumKupovine;
        }
        public Racun(int id)
        {
            this.idRacun = id;
        }
        public Racun(int idRaacun, DateTime datumKupovine, string imeProdavnice)
        {
            this.idRacun = idRaacun;
            this.datumKupovine = datumKupovine;
            this.prodavnica = new(imeProdavnice);
        }
        public int? IdRacun
        {
            get { return idRacun; }
            set { idRacun = value; }
        }
        public string ImeProdavnice
        {
            get { return prodavnica.Ime; }
        }
        public DateTime DatumKupovine
        {
            get { return datumKupovine; }
        }
    }
}
