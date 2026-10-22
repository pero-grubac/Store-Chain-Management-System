using OnlineKupovinaDB;

namespace OnlineKupovina.Data
{
    public sealed class AuthRepozitorij(Baza baza)
    {
        // Provjera i za nepostojeceg korisnika traje jednako dugo kao za postojeceg,
        // pa se iz vremena odgovora ne vidi koja korisnicka imena postoje.
        private const string LazniHash = "$2a$11$7EqJtq98hPqEX7fNZaFWoOhi5BXPfp3hQ9ZbJ8pXqU0yC1Z2fZx6S";

        private sealed class PrijavaRed
        {
            public int Id { get; set; }
            public string KorisnickoIme { get; set; } = string.Empty;
            public string PunoIme { get; set; } = string.Empty;
            public string Lozinka { get; set; } = string.Empty;
            public int? IdProdavnice { get; set; }
            public string? ImeProdavnice { get; set; }
        }

        private async Task<PrijavaRed?> PronadjiAsync(Uloga uloga, string korisnickoIme, CancellationToken ct) =>
            (await baza.ProceduraUpitAsync<PrijavaRed>("prijava_korisnika",
                new { pUloga = uloga.UBazi(), pKorisnickoIme = korisnickoIme.Trim().ToLowerInvariant() }, ct)).FirstOrDefault();

        /// <returns>Sesija ili null ako korisnicko ime ili lozinka nisu ispravni.</returns>
        public async Task<Sesija?> PrijavaAsync(Uloga uloga, string korisnickoIme, string lozinka, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(korisnickoIme) || string.IsNullOrEmpty(lozinka)) return null;

            var red = await PronadjiAsync(uloga, korisnickoIme, ct);
            bool ispravna = await Task.Run(() => Lozinke.Provjeri(lozinka, red?.Lozinka ?? LazniHash), ct);
            if (red == null || !ispravna) return null;

            return new Sesija(red.Id, red.KorisnickoIme, red.PunoIme, uloga, red.IdProdavnice, red.ImeProdavnice);
        }

        public async Task PromijeniLozinkuAsync(Sesija sesija, string trenutna, string nova, string potvrda, CancellationToken ct = default)
        {
            Greske.AkoIma(Validacija.Lozinka(nova, potvrda));
            if (trenutna == nova)
            {
                throw new PoslovnaGreskaException("Nova lozinka mora biti različita od trenutne.");
            }

            var red = await PronadjiAsync(sesija.Uloga, sesija.KorisnickoIme, ct);
            if (red == null || !await Task.Run(() => Lozinke.Provjeri(trenutna, red.Lozinka), ct))
            {
                throw new PoslovnaGreskaException("Trenutna lozinka nije ispravna.");
            }

            await PostaviLozinkuAsync(sesija.Uloga, sesija.KorisnickoIme, nova, ct);
        }

        /// <summary>Nova lozinka bez provjere stare (administrator postavlja lozinku drugom korisniku).</summary>
        public async Task PostaviLozinkuAsync(Uloga uloga, string korisnickoIme, string nova, CancellationToken ct = default)
        {
            Greske.AkoIma(Validacija.Lozinka(nova));
            string hash = await Task.Run(() => Lozinke.Hash(nova), ct);
            await baza.ProceduraAsync("promijeni_lozinku",
                new { pUloga = uloga.UBazi(), pKorisnickoIme = korisnickoIme, pLozinka = hash }, ct);
        }
    }
}
