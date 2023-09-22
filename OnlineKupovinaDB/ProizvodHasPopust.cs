namespace OnlineKupovinaDB
{
    public class ProizvodHasPopust
    {
        private int idProizvod { get; set; }
        private int idPopust { get; set; }
        public ProizvodHasPopust(int idProizvod, int idPopust)
        {
            this.idPopust = idPopust;
            this.idProizvod = idProizvod;
        }
    }
}
