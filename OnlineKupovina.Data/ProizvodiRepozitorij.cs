using Dapper;
using OnlineKupovinaDB;

namespace OnlineKupovina.Data
{
    public sealed class ProizvodiRepozitorij(Baza baza)
    {
        public Task<List<Proizvod>> PoProdavniciAsync(int idProdavnice, CancellationToken ct = default) =>
            baza.ProceduraUpitAsync<Proizvod>("get_proizvod_by_prodavnica", new { pIdProdavnica = idProdavnice }, ct);

        public Task<List<Proizvod>> SviAktivniAsync(CancellationToken ct = default) =>
            baza.UpitAsync<Proizvod>("SELECT * FROM v_proizvodi WHERE Aktivan = 1 ORDER BY Prodavnica, Naziv", ct: ct);

        public async Task<Proizvod?> PoBarkoduAsync(int idProdavnice, string barkod, CancellationToken ct = default) =>
            (await baza.ProceduraUpitAsync<Proizvod>("pronadji_po_barkodu",
                new { pIdProdavnice = idProdavnice, pBarKod = Barkod.Normalizuj(barkod) }, ct)).FirstOrDefault();

        public Task<int> DodajAsync(Proizvod p, int? idZaposlenog, CancellationToken ct = default)
        {
            p.BarKod = Barkod.Normalizuj(p.BarKod);
            Greske.AkoIma(p.Validiraj(noviProizvod: true));
            return baza.ProceduraSaIdAsync("dodaj_proizvod", "pIdProizvoda", new
            {
                pNaziv = p.Naziv.Trim(), pCijena = p.Cijena, pDatumIsteka = p.DatumIsteka?.Date, pBarKod = p.BarKod,
                pIdProizvodjaca = p.IdProizvodjaca, pOpis = p.Opis, pZalihe = p.Zalihe, pIdProdavnice = p.IdProdavnice,
                pMinZalihe = p.MinZalihe, pIdZaposlenog = idZaposlenog
            }, ct);
        }

        public Task IzmijeniAsync(Proizvod p, CancellationToken ct = default)
        {
            p.BarKod = Barkod.Normalizuj(p.BarKod);
            Greske.AkoIma(p.Validiraj());
            return baza.ProceduraAsync("update_proizvod", new
            {
                pIdProizvoda = p.Id, pNaziv = p.Naziv.Trim(), pDatumIsteka = p.DatumIsteka?.Date, pBarKod = p.BarKod,
                pIdProizvodjaca = p.IdProizvodjaca, pOpis = p.Opis, pZalihe = p.Zalihe, pCijena = p.Cijena, pMinZalihe = p.MinZalihe
            }, ct);
        }

        public Task ObrisiAsync(int id, CancellationToken ct = default) =>
            baza.ProceduraAsync("obrisi_proizvod", new { pIdProizvoda = id }, ct);

        public Task PrijemRobeAsync(int idProizvoda, int kolicina, string korisnik, CancellationToken ct = default)
        {
            if (kolicina <= 0) throw new PoslovnaGreskaException("Količina prijema mora biti veća od nule.");
            return baza.ProceduraAsync("prijem_robe", new { pIdProizvoda = idProizvoda, pKolicina = kolicina, pKorisnik = korisnik }, ct);
        }

        public Task<List<Proizvod>> NiskeZaliheAsync(int? idProdavnice = null, CancellationToken ct = default) =>
            baza.ProceduraUpitAsync<Proizvod>("izvjestaj_niske_zalihe", new { pIdProdavnice = idProdavnice }, ct);

        // ------------------------------------------------------------ proizvodjaci
        public Task<List<Proizvodjac>> ProizvodjaciAsync(CancellationToken ct = default) =>
            baza.UpitAsync<Proizvodjac>("SELECT * FROM v_proizvodjaci ORDER BY Naziv", ct: ct);

        public Task<int> DodajProizvodjacaAsync(Proizvodjac p, CancellationToken ct = default)
        {
            Greske.AkoIma(p.Validiraj());
            return baza.ProceduraSaIdAsync("registruj_proizvodjaca", "pIdProizvodjaca",
                new { pIme = p.Naziv, pEmail = p.Email, pGrad = p.Grad, pUlica = p.Ulica, pDrzava = p.Drzava }, ct);
        }

        public Task IzmijeniProizvodjacaAsync(Proizvodjac p, CancellationToken ct = default)
        {
            Greske.AkoIma(p.Validiraj());
            return baza.ProceduraAsync("izmijeni_proizvodjaca",
                new { pIdProizvodjaca = p.Id, pIme = p.Naziv, pEmail = p.Email, pGrad = p.Grad, pUlica = p.Ulica, pDrzava = p.Drzava }, ct);
        }

        public Task ObrisiProizvodjacaAsync(int id, CancellationToken ct = default) =>
            baza.ProceduraAsync("obrisi_proizvodjaca", new { pIdProizvodjaca = id }, ct);
    }
}
