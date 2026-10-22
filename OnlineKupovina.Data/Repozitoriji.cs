namespace OnlineKupovina.Data
{
    /// <summary>Svi repozitoriji nad jednom bazom.</summary>
    public sealed class Repozitoriji
    {
        public Repozitoriji(string connectionString)
        {
            Baza = new Baza(connectionString);
            Auth = new AuthRepozitorij(Baza);
            Prodavnice = new ProdavniceRepozitorij(Baza);
            Korisnici = new KorisniciRepozitorij(Baza);
            Proizvodi = new ProizvodiRepozitorij(Baza);
            Prodaja = new ProdajaRepozitorij(Baza);
            Akcije = new AkcijeRepozitorij(Baza);
            Izvjestaji = new IzvjestajiRepozitorij(Baza);
        }

        public Baza Baza { get; }
        public AuthRepozitorij Auth { get; }
        public ProdavniceRepozitorij Prodavnice { get; }
        public KorisniciRepozitorij Korisnici { get; }
        public ProizvodiRepozitorij Proizvodi { get; }
        public ProdajaRepozitorij Prodaja { get; }
        public AkcijeRepozitorij Akcije { get; }
        public IzvjestajiRepozitorij Izvjestaji { get; }
    }
}
