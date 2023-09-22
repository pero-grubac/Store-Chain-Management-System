namespace OnlineKupovinaDB
{
    public class Proizvod
    {
        private int idProizvod { get; set; }
        private string naziv { get; set; }
        private DateTime datumIsteka { get; set; }
        private int barkod { get; set; }
        private string opis { get; set; }
        private Proizvodjac proizvodjac { get; set; }
        private double cijena { get; set; }
        private int zalihe { get; set; }
        public Proizvod(int idProizvod, string naziv, double cijena, DateTime datumIsteka, int barkod, string opis, int zalihe, Proizvodjac proizvodjac)
        {
            this.idProizvod = idProizvod;
            this.naziv = naziv;
            this.datumIsteka = datumIsteka;
            this.barkod = barkod;
            this.opis = opis;
            this.zalihe = zalihe;
            this.cijena = cijena;
            this.proizvodjac = proizvodjac;

        }
        public Proizvod(int id)
        {
            idProizvod = id;
        }
        public Proizvod(string nazivProizvoda)
        {
            naziv = nazivProizvoda;
        }
        public int ID
        {
            get { return idProizvod; }
            set { idProizvod = value; }
        }
        public string Naziv
        {
            get { return naziv; }
            set { naziv = value; }
        }
        public double Cijena
        {
            get { return cijena; }
            set { cijena = value; }
        }
        public DateTime Datum
        {
            get { return datumIsteka; }
            set { datumIsteka = value; }
        }
        public string Opis
        {
            get { return opis; }
            set { opis = value; }
        }
        public int BarKod
        {
            get { return barkod; }
            set { barkod = value; }
        }
        public int Zalihe
        {
            get { return zalihe; }
            set { zalihe = value; }
        }
        public int IdProizvodjaca
        {
            get { return proizvodjac.ID; }
            set { proizvodjac.ID = value; }

        }
        public string ImeProizvodjaca
        {
            get { return proizvodjac.Ime; }
            set { proizvodjac.Ime = value; }
        }
        public string EmailProizvodjaca
        {
            get { return proizvodjac.Email; }
            set { proizvodjac.Email = value; }

        }
        public string DrzavaPorijekla
        {
            get { return proizvodjac.Drzava; }
        }

    }
}
