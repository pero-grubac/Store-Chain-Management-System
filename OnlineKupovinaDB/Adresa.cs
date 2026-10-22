namespace OnlineKupovinaDB
{
    public class Adresa
    {
        public int IdAdrese { get; set; }
        public string Grad { get; set; } = string.Empty;
        public string Ulica { get; set; } = string.Empty;
        public string Drzava { get; set; } = string.Empty;

        public string PunaAdresa => string.Join(", ", new[] { Ulica, Grad, Drzava }.Where(d => !string.IsNullOrWhiteSpace(d)));

        protected void ValidirajAdresu(List<string> greske)
        {
            Validacija.Obavezno(greske, Grad, "Grad", 45);
            Validacija.Obavezno(greske, Ulica, "Ulica", 100);
            Validacija.Obavezno(greske, Drzava, "Država", 45);
        }

        public virtual List<string> Validiraj()
        {
            var greske = new List<string>();
            ValidirajAdresu(greske);
            return greske;
        }
    }
}
