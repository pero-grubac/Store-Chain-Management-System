namespace OnlineKupovinaDB
{
    public class Adresa
    {
        public int idAdresa { get; set; }
        public string grad { get; set; }
        public string ulica { get; set; }
        public string drzava { get; set; }

        public Adresa(int idAdresa, string grad, string ulica, string drzava)
        {
            this.idAdresa = idAdresa;
            this.grad = grad;
            this.ulica = ulica;
            this.drzava = drzava;
        }
        public Adresa(string drzava)
        {
            this.drzava = drzava;
        }
        public Adresa(int id) { this.idAdresa = id; }
        public Adresa(string grad, string ulica, string drzava)
        {
            this.grad = grad;
            this.ulica = ulica;
            this.drzava = drzava;
        }
        public int ID
        {
            get { return idAdresa; }
            set { idAdresa = value; }
        }
        public string Ulica
        {
            get { return ulica; }
            set { ulica = value; }
        }
        public string Grad
        {
            get { return grad; }
            set { grad = value; }
        }
        public string Drzava
        {
            get { return drzava; }
            set { drzava = value; }
        }
    }
}
