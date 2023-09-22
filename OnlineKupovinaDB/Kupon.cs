namespace OnlineKupovinaDB
{
    public class Kupon
    {
        private int idKupon { get; set; }
        private int sifra { get; set; }
        private DateTime datumKreiranja { get; set; }
        private DateTime datumIsteka { get; set; }
        private int popust { get; set; }

        public Kupon(int idKupon, int sifra, DateTime datumKreiranja, DateTime datumIsteka, int popust)
        {
            this.idKupon = idKupon;
            this.sifra = sifra;
            this.datumKreiranja = datumKreiranja;
            this.datumIsteka = datumIsteka;
            this.popust = popust;
        }
    }
}
