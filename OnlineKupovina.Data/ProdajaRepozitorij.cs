using OnlineKupovinaDB;

namespace OnlineKupovina.Data
{
    public sealed class ProdajaRepozitorij(Baza baza)
    {
        private sealed class KuponProizvod
        {
            public int IdProizvoda { get; set; }
            public int Popust { get; set; }
        }

        /// <summary>Popusti koje kupon daje na proizvode prodavnice (id proizvoda -> procenat).</summary>
        public async Task<Dictionary<int, int>> ProvjeriKuponAsync(string sifra, int idProdavnice, CancellationToken ct = default)
        {
            string normalizovana = Kupon.NormalizujSifru(sifra);
            if (normalizovana.Length == 0) throw new PoslovnaGreskaException("Unesite šifru kupona.");

            var redovi = await baza.ProceduraUpitAsync<KuponProizvod>("provjeri_kupon",
                new { pSifra = normalizovana, pIdProdavnice = idProdavnice }, ct);
            return redovi.ToDictionary(r => r.IdProizvoda, r => r.Popust);
        }

        /// <returns>Broj novog racuna.</returns>
        public Task<int> NaruciAsync(int idKupca, Korpa korpa, string korisnik, CancellationToken ct = default)
        {
            if (korpa.JePrazna || korpa.IdProdavnice is null) throw new PoslovnaGreskaException("Korpa je prazna.");

            return baza.IzvrsiAsync(async k =>
            {
                // @korisnik ostaje u sesiji i trigger ga upisuje u istoriju zaliha
                await Dapper.SqlMapper.ExecuteAsync(k, "SET @korisnik = @ime", new { ime = korisnik });
                var p = new Dapper.DynamicParameters(new
                {
                    pIdKupca = idKupca, pIdProdavnice = korpa.IdProdavnice, pStavke = korpa.UJson(), pSifraKupona = korpa.Kupon
                });
                p.Add("pIdRacuna", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                await Dapper.SqlMapper.ExecuteAsync(k, new Dapper.CommandDefinition("narucivanje", p,
                    commandType: System.Data.CommandType.StoredProcedure, cancellationToken: ct));
                return p.Get<int>("pIdRacuna");
            }, ct);
        }

        public Task<List<Racun>> RacuniKupcaAsync(int idKupca, CancellationToken ct = default) =>
            baza.ProceduraUpitAsync<Racun>("detalji_racuna", new { pIdKupca = idKupca }, ct);

        public Task<List<Racun>> RacuniAsync(DateTime od, DateTime @do, int? idProdavnice = null, CancellationToken ct = default) =>
            baza.ProceduraUpitAsync<Racun>("pregled_racuna", new { pOd = od.Date, pDo = @do.Date, pIdProdavnice = idProdavnice }, ct);

        public Task<List<Stavka>> StavkeAsync(int idRacuna, CancellationToken ct = default) =>
            baza.ProceduraUpitAsync<Stavka>("stavke_racuna", new { pIdRacuna = idRacuna }, ct);

        public Task StornirajAsync(int idRacuna, string korisnik, CancellationToken ct = default) =>
            baza.ProceduraAsync("storniraj_racun", new { pIdRacuna = idRacuna, pKorisnik = korisnik }, ct);
    }
}
