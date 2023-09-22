namespace OnlineKupovinaDB
{
    public class Zaposleni
    {
        private int idZaposelni { get; set; }
        private string ime { get; set; }
        private string prezime { get; set; }
        private string email { get; set; }
        private string brojTellefona { get; set; }
        private int idProdavnica { get; set; }
        private double plata { get; set; }
        private string korisnickoIme { get; set; }
        public Zaposleni(int idZaposelni, string ime, string prezime, string email, string brojTelefona, int idProdavnica, double plata, string korisnickoIme)
        {
            this.idZaposelni = idZaposelni;
            this.ime = ime;
            this.brojTellefona = brojTelefona;
            this.prezime = prezime;
            this.email = email;
            this.idProdavnica = idProdavnica;
            this.plata = plata;
            this.korisnickoIme = korisnickoIme;
        }
        public int ID { get { return idZaposelni; } }
        public string KorisnickoIme { get { return korisnickoIme; } set { korisnickoIme = value; } }
        public string Ime { get { return ime; } set { ime = value; } }
        public string Prezime { get { return prezime; } set { prezime = value; } }
        public string Email { get { return email; } set { email = value; } }
        public double Plata { get { return plata; } set { plata = value; } }
        public string BrojTelefona { get { return brojTellefona; } set { brojTellefona = value; } }
        public int IdProdavnice { get { return idProdavnica; } set { idProdavnica = value; } }
    }
}
