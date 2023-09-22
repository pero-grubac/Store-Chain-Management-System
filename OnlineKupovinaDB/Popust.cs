namespace OnlineKupovinaDB
{
    public class Popust
    {
        private int idPopust { get; set; }
        private int popust { get; set; }
        private DateTime datumKreiranja { get; set; }
        private DateTime datumIsteka { get; set; }
        public Popust(int idPopust, int popust, DateTime datumKreiranja, DateTime datumIsteka)
        {
            this.idPopust = idPopust;
            this.popust = popust;
            this.datumKreiranja = datumKreiranja;
            this.datumIsteka = datumIsteka;
        }
    }
}
