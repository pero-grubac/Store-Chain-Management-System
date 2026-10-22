namespace OnlineKupovinaDB
{
    public enum Uloga
    {
        Administrator,
        Zaposleni,
        Kupac
    }

    public static class UlogaExtensions
    {
        /// <summary>Vrijednost koju ocekuju procedure prijava_korisnika i promijeni_lozinku.</summary>
        public static string UBazi(this Uloga uloga) => uloga switch
        {
            Uloga.Administrator => "administrator",
            Uloga.Zaposleni => "zaposleni",
            Uloga.Kupac => "kupac",
            _ => throw new ArgumentOutOfRangeException(nameof(uloga))
        };

        public static string Naziv(this Uloga uloga) => uloga switch
        {
            Uloga.Administrator => "Administrator",
            Uloga.Zaposleni => "Radnik",
            Uloga.Kupac => "Kupac",
            _ => throw new ArgumentOutOfRangeException(nameof(uloga))
        };
    }

    /// <summary>Prijavljeni korisnik. Za radnika nosi i prodavnicu u kojoj radi.</summary>
    public sealed record Sesija(int Id, string KorisnickoIme, string PunoIme, Uloga Uloga, int? IdProdavnice = null, string? ImeProdavnice = null);
}
