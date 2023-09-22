namespace OnlineKupovinaDB
{
    public class Administrator
    {
        private int idAdministrator { get; set; }
        private string ime { get; set; }
        private string prezime { get; set; }
        private string email { get; set; }
        private string korisnickoIme { get; set; }
        public Administrator(int idAdministrator, string ime, string prezime, string email, string korisnickoIme)
        {
            this.idAdministrator = idAdministrator;
            this.ime = ime;
            this.prezime = prezime;
            this.email = email;
            this.korisnickoIme = korisnickoIme;

        }

        public int ID
        {
            get { return idAdministrator; }
        }
        public string Ime
        {
            get { return ime; }
            set { ime = value; }
        }
        public string Prezime
        {
            get { return prezime; }
            set { prezime = value; }
        }
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        public string KorisnickoIme
        {
            get { return korisnickoIme; }
            set { korisnickoIme = value; }
        }
    }
}
