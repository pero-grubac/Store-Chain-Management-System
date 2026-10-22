namespace OnlineKupovinaDB
{
    public class Zaposleni
    {
        public int Id { get; set; }
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string KorisnickoIme { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? BrojTelefona { get; set; }
        public decimal Plata { get; set; }
        public int IdProdavnice { get; set; }
        public string Prodavnica { get; set; } = string.Empty;

        public string PunoIme => $"{Ime} {Prezime}";

        public List<string> Validiraj()
        {
            var greske = new List<string>();
            Validacija.Obavezno(greske, Ime, "Ime", 50);
            Validacija.Obavezno(greske, Prezime, "Prezime", 50);
            Validacija.KorisnickoIme(greske, KorisnickoIme);
            Validacija.Email(greske, Email);
            Validacija.Telefon(greske, BrojTelefona);
            Validacija.Raspon(greske, Plata, 0.01m, 99_999_999m, "Plata");
            if (IdProdavnice <= 0) greske.Add("Izaberite prodavnicu.");
            return greske;
        }
    }
}
