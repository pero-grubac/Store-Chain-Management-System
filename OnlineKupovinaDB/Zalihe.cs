namespace OnlineKupovinaDB
{
    public class Zalihe
    {
        private int kolicina { get; set; }
        private int idProizvod { get; set; }
        private int idProdavnica { get; set; }
        public Zalihe(int kolicina, int idProizvod, int idProdavnica)
        {
            this.kolicina = kolicina;
            this.idProizvod = idProizvod;
            this.idProdavnica = idProdavnica;
        }
    }
}
