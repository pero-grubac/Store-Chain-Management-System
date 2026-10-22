namespace OnlineKupovinaDB
{
    public class Kupac : Adresa
    {
        public int Id { get; set; }
        public string KorisnickoIme { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int BrojRacuna { get; set; }
        public decimal Potroseno { get; set; }

        public override List<string> Validiraj()
        {
            var greske = new List<string>();
            Validacija.KorisnickoIme(greske, KorisnickoIme);
            Validacija.Email(greske, Email);
            ValidirajAdresu(greske);
            return greske;
        }
    }
}
