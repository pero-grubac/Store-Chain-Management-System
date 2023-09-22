namespace OnlineKupovinaDB
{
    public class Proizvodjac : Kompanija
    {
        private string email { get; set; }

        public Proizvodjac(string email, int id, string ime, Adresa adresa) : base(id, ime, adresa)
        {
            this.email = email;
        }
        public Proizvodjac(string email, string ime, string drzava) : base(ime, drzava)
        {
            this.email = email;
        }
        public Proizvodjac(int id, string email, string ime, string drzava) : base(id, ime, drzava)
        {
            this.email = email;
        }
        public int ID
        {
            get { return id; }
            set { id = value; }

        }
        public string Ime
        {
            get { return ime; }
            set { ime = value; }
        }


        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        public int IdAdresa
        {
            get { return adresa.idAdresa; }
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
