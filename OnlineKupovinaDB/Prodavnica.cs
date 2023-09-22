namespace OnlineKupovinaDB
{
    public class Prodavnica : Kompanija
    {

        public Prodavnica(int id, string ime, Adresa adresa) : base(id, ime, adresa)
        {

        }
        public Prodavnica(int id, string ime, int idAdresa) : base(id, ime, idAdresa)
        {
        }
        public Prodavnica(string ime) : base(ime) { }
        public Prodavnica(int id) : base(id) { }
        public int ID
        {
            get { return id; }

        }
        public string Ime
        {
            get { return ime; }
            set { ime = value; }
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
