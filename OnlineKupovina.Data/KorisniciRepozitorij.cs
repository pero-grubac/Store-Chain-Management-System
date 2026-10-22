using OnlineKupovinaDB;

namespace OnlineKupovina.Data
{
    public sealed class KorisniciRepozitorij(Baza baza)
    {
        private static async Task<string> HashAsync(string lozinka, CancellationToken ct)
        {
            Greske.AkoIma(Validacija.Lozinka(lozinka));
            return await Task.Run(() => Lozinke.Hash(lozinka), ct);
        }

        // ------------------------------------------------------------ zaposleni
        public Task<List<Zaposleni>> ZaposleniAsync(int? idProdavnice = null, CancellationToken ct = default) =>
            baza.UpitAsync<Zaposleni>(
                "SELECT * FROM v_zaposleni WHERE @id IS NULL OR IdProdavnice = @id ORDER BY Prezime, Ime",
                new { id = idProdavnice }, ct);

        public async Task DodajZaposlenogAsync(Zaposleni z, string lozinka, CancellationToken ct = default)
        {
            Greske.AkoIma(z.Validiraj());
            string hash = await HashAsync(lozinka, ct);
            await baza.ProceduraAsync("dodaj_radnika", new
            {
                pIme = z.Ime, pPrezime = z.Prezime, pEmail = z.Email, pBrojTelefona = z.BrojTelefona,
                pIdProdavnice = z.IdProdavnice, pPlata = z.Plata, pLozinka = hash, pKorisnickoIme = z.KorisnickoIme
            }, ct);
        }

        public Task IzmijeniZaposlenogAsync(Zaposleni z, CancellationToken ct = default)
        {
            Greske.AkoIma(z.Validiraj());
            return baza.IzvrsiAsync(k => Dapper.SqlMapper.ExecuteAsync(k,
                @"UPDATE zaposleni
                  SET Ime = @Ime, Prezime = @Prezime, KorisnickoIme = @KorisnickoIme, Email = @Email,
                      BrojTelefona = NULLIF(TRIM(@BrojTelefona), ''), Plata = @Plata, Prodavnica_idProdavnica = @IdProdavnice
                  WHERE idZAPOSLENI = @Id", z), ct);
        }

        public Task ObrisiZaposlenogAsync(int id, CancellationToken ct = default) =>
            baza.IzvrsiAsync(k => Dapper.SqlMapper.ExecuteAsync(k, "DELETE FROM zaposleni WHERE idZAPOSLENI = @id", new { id }), ct);

        // ------------------------------------------------------------ kupci
        public Task<List<Kupac>> KupciAsync(CancellationToken ct = default) =>
            baza.UpitAsync<Kupac>("SELECT * FROM v_kupci ORDER BY KorisnickoIme", ct: ct);

        public async Task<int> DodajKupcaAsync(Kupac kupac, string lozinka, CancellationToken ct = default)
        {
            Greske.AkoIma(kupac.Validiraj());
            string hash = await HashAsync(lozinka, ct);
            return await baza.ProceduraSaIdAsync("registruj_kupca", "pIdKupca", new
            {
                pKorisnickoIme = kupac.KorisnickoIme, pLozinka = hash, pEmail = kupac.Email,
                pGrad = kupac.Grad, pUlica = kupac.Ulica, pDrzava = kupac.Drzava
            }, ct);
        }

        public Task IzmijeniKupcaAsync(Kupac kupac, CancellationToken ct = default)
        {
            Greske.AkoIma(kupac.Validiraj());
            return baza.ProceduraAsync("izmijeni_kupca", new
            {
                pIdKupca = kupac.Id, pKorisnickoIme = kupac.KorisnickoIme, pEmail = kupac.Email,
                pGrad = kupac.Grad, pUlica = kupac.Ulica, pDrzava = kupac.Drzava
            }, ct);
        }

        public Task ObrisiKupcaAsync(int id, CancellationToken ct = default) =>
            baza.ProceduraAsync("obrisi_kupca", new { pIdKupca = id }, ct);

        // ------------------------------------------------------------ administratori
        public Task<List<Administrator>> AdministratoriAsync(CancellationToken ct = default) =>
            baza.UpitAsync<Administrator>("SELECT * FROM v_administratori ORDER BY Prezime, Ime", ct: ct);

        public async Task DodajAdministratoraAsync(Administrator a, string lozinka, CancellationToken ct = default)
        {
            Greske.AkoIma(a.Validiraj());
            string hash = await HashAsync(lozinka, ct);
            await baza.ProceduraAsync("dodaj_administratora", new
            {
                pIme = a.Ime, pPrezime = a.Prezime, pEmail = a.Email, pLozinka = hash, pKorisnickoIme = a.KorisnickoIme
            }, ct);
        }

        public Task IzmijeniAdministratoraAsync(Administrator a, CancellationToken ct = default)
        {
            Greske.AkoIma(a.Validiraj());
            return baza.IzvrsiAsync(k => Dapper.SqlMapper.ExecuteAsync(k,
                "UPDATE administrator SET Ime = @Ime, Prezime = @Prezime, KorisnickoIme = @KorisnickoIme, Email = @Email WHERE idAdministrator = @Id",
                a), ct);
        }

        public async Task ObrisiAdministratoraAsync(int id, int idPrijavljenog, CancellationToken ct = default)
        {
            if (id == idPrijavljenog)
            {
                throw new PoslovnaGreskaException("Ne možete obrisati nalog sa kojim ste prijavljeni.");
            }
            await baza.IzvrsiAsync(k => Dapper.SqlMapper.ExecuteAsync(k, "DELETE FROM administrator WHERE idAdministrator = @id", new { id }), ct);
        }
    }
}
