namespace OnlineKupovinaDB
{
    public class ProizvodHasKupon
    {
        private int idProizvod { get; set; }
        private int idKupon { get; set; }
        public ProizvodHasKupon(int idProizvod, int idKupon)
        {
            this.idProizvod = idProizvod;
            this.idKupon = idKupon;
        }
    }
}
