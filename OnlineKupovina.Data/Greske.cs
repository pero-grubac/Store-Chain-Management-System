using MySqlConnector;

namespace OnlineKupovina.Data
{
    /// <summary>Krsenje poslovnog pravila: poruka je namijenjena korisniku.</summary>
    public class PoslovnaGreskaException(string poruka, Exception? uzrok = null) : Exception(poruka, uzrok);

    /// <summary>Baza nije dostupna (server ugasen, pogresan connection string).</summary>
    public class BazaNedostupnaException(string poruka, Exception? uzrok = null) : Exception(poruka, uzrok);

    public static class Greske
    {
        private static readonly (string Kljuc, string Poruka)[] Jedinstveni =
        [
            ("korisnicko_ime", "Korisničko ime je već zauzeto."),
            ("email", "Email adresa je već zauzeta."),
            ("barkod", "Proizvod sa ovim bar kodom već postoji u prodavnici."),
            ("uq_prodavnica_ime", "Prodavnica sa tim nazivom već postoji."),
            ("uq_proizvodjac_ime", "Proizvođač sa tim nazivom već postoji."),
            ("uq_kupon_sifra", "Kupon sa tom šifrom već postoji."),
        ];

        /// <summary>Baca gresku sa svim porukama validacije, ako ih ima.</summary>
        public static void AkoIma(IReadOnlyCollection<string> greske)
        {
            if (greske.Count > 0)
            {
                throw new PoslovnaGreskaException(string.Join(Environment.NewLine, greske));
            }
        }

        /// <summary>Pretvara MySQL gresku u poruku koju korisnik razumije.</summary>
        public static Exception Prevedi(MySqlException e)
        {
            switch (e.ErrorCode)
            {
                case (MySqlErrorCode)1644: // SIGNAL SQLSTATE '45000'
                    return new PoslovnaGreskaException(e.Message, e);
                case MySqlErrorCode.DuplicateKeyEntry:
                    string poruka = Jedinstveni.FirstOrDefault(j => e.Message.Contains(j.Kljuc, StringComparison.OrdinalIgnoreCase)).Poruka
                        ?? "Zapis sa istom vrijednošću već postoji.";
                    return new PoslovnaGreskaException(poruka, e);
                case MySqlErrorCode.RowIsReferenced:
                case MySqlErrorCode.RowIsReferenced2:
                    return new PoslovnaGreskaException("Zapis se koristi na drugom mjestu i ne može se obrisati.", e);
                case MySqlErrorCode.NoReferencedRow:
                case MySqlErrorCode.NoReferencedRow2:
                    return new PoslovnaGreskaException("Povezani zapis ne postoji.", e);
                case MySqlErrorCode.DataTooLong:
                    return new PoslovnaGreskaException("Unesena vrijednost je preduga.", e);
                case (MySqlErrorCode)3819: // CHECK ogranicenje
                    return new PoslovnaGreskaException("Vrijednost nije dozvoljena: " + e.Message, e);
                case MySqlErrorCode.UnableToConnectToHost:
                case MySqlErrorCode.AccessDenied:
                case MySqlErrorCode.UnknownDatabase:
                    return new BazaNedostupnaException(
                        "Baza nije dostupna. Provjerite da li je MySQL pokrenut (docker compose up -d) i da li je connection string ispravan.", e);
                default:
                    return e;
            }
        }
    }
}
