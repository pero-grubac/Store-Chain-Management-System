namespace OnlineKupovinaDB
{
    public class Kompanija
    {
        private int idAdresa;

        protected int id { get; set; }
        protected string ime { get; set; }
        protected Adresa adresa { get; set; }
        public Kompanija(int id)
        {
            this.id = id;
        }
        public Kompanija(string ime)
        {
            this.ime = ime;
        }
        public Kompanija(int idKompanija, string ime, Adresa adresa)
        {
            this.id = idKompanija;
            this.ime = ime;
            this.adresa = adresa;
        }
        public Kompanija(string ime, string drzava)
        {
            this.ime = ime;
            this.adresa = new Adresa(drzava);
        }
        public Kompanija(int id, string ime, string drzava)
        {
            this.id = id;
            this.ime = ime;
            this.adresa = new Adresa(drzava);
        }
        public Kompanija(int id, string ime, int idAdresa)
        {
            this.id = id;
            this.ime = ime;
            this.idAdresa = idAdresa;
        }
    }
}
