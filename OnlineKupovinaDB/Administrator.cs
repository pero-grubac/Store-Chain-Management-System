namespace OnlineKupovinaDB
{
    public class Administrator
    {
        public int Id { get; set; }
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string KorisnickoIme { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string PunoIme => $"{Ime} {Prezime}";

        public List<string> Validiraj()
        {
            var greske = new List<string>();
            Validacija.Obavezno(greske, Ime, "Ime", 50);
            Validacija.Obavezno(greske, Prezime, "Prezime", 50);
            Validacija.KorisnickoIme(greske, KorisnickoIme);
            Validacija.Email(greske, Email);
            return greske;
        }
    }
}
