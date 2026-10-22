using OnlineKupovinaDB;

namespace OnlineKupovina.Data
{
    public sealed class ProdavniceRepozitorij(Baza baza)
    {
        public Task<List<Prodavnica>> SveAsync(CancellationToken ct = default) =>
            baza.UpitAsync<Prodavnica>("SELECT * FROM v_prodavnice ORDER BY Naziv", ct: ct);

        public Task<int> DodajAsync(Prodavnica p, CancellationToken ct = default)
        {
            Greske.AkoIma(p.Validiraj());
            return baza.ProceduraSaIdAsync("dodaj_prodavnicu", "pIdProdavnice",
                new { pIme = p.Naziv.Trim(), pGrad = p.Grad, pUlica = p.Ulica, pDrzava = p.Drzava }, ct);
        }

        public Task IzmijeniAsync(Prodavnica p, CancellationToken ct = default)
        {
            Greske.AkoIma(p.Validiraj());
            return baza.ProceduraAsync("izmijeni_prodavnicu",
                new { pIdProdavnice = p.Id, pIme = p.Naziv.Trim(), pGrad = p.Grad, pUlica = p.Ulica, pDrzava = p.Drzava }, ct);
        }

        public Task ObrisiAsync(int id, CancellationToken ct = default) =>
            baza.ProceduraAsync("obrisi_prodavnicu", new { pIdProdavnice = id }, ct);
    }
}
