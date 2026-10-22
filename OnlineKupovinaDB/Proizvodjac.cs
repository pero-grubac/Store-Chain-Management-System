namespace OnlineKupovinaDB
{
    public class Proizvodjac : Adresa
    {
        public int Id { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int BrojProizvoda { get; set; }

        public override List<string> Validiraj()
        {
            var greske = new List<string>();
            Validacija.Obavezno(greske, Naziv, "Naziv proizvođača", 45);
            Validacija.Email(greske, Email);
            ValidirajAdresu(greske);
            return greske;
        }

        public override string ToString() => Naziv;
    }
}
