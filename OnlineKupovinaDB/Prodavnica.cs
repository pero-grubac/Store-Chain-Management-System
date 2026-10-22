namespace OnlineKupovinaDB
{
    public class Prodavnica : Adresa
    {
        public int Id { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public int BrojZaposlenih { get; set; }
        public int BrojProizvoda { get; set; }

        public override List<string> Validiraj()
        {
            var greske = new List<string>();
            Validacija.Obavezno(greske, Naziv, "Naziv prodavnice", 45);
            ValidirajAdresu(greske);
            return greske;
        }

        public override string ToString() => Naziv;
    }
}
