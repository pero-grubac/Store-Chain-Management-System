namespace OnlineKupovinaDB
{
    public class Kupac
    {
        private int idKupac { get; set; }
        private string korisnickoIme { get; set; }

        private string email { get; set; }
        private Adresa adresa { get; set; }
        public Kupac(int idKupac)
        {
            this.idKupac = idKupac;
        }
        public Kupac(int idKupac, string korisnickoIme, string email, Adresa adresa)
        {
            this.idKupac = idKupac;
            this.korisnickoIme = korisnickoIme;
            this.email = email;
            this.adresa = adresa;
        }
        public Kupac(int idKupac, string korisnickoIme, string email, int idAdresa)
        {
            KorisnickoIme = korisnickoIme;
            Email = email;
            adresa = new Adresa(idAdresa);

        }

        public int IdKupca
        {
            get { return idKupac; }
        }
        public string KorisnickoIme
        {
            get { return korisnickoIme; }
            set { korisnickoIme = value; }
        }
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        public int idAdresa
        {
            get { return adresa.idAdresa; }
            set { adresa.idAdresa = value; }

        }
        public string Drzava
        {
            get { return adresa?.drzava; }
            set { adresa.drzava = value; }
        }
        public string Grad
        {
            get { return adresa?.grad; }
            set { adresa.grad = value; }
        }
        public string Ulica
        {
            get { return adresa?.ulica; }
            set { adresa.ulica = value; }
        }
    }
}
