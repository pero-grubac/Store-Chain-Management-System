using OnlineKupovinaDB;

namespace OnlineKupovina.Data
{
    public sealed class IzvjestajiRepozitorij(Baza baza)
    {
        public async Task<Kpi> KpiAsync(DateTime od, DateTime @do, CancellationToken ct = default) =>
            (await baza.ProceduraUpitAsync<Kpi>("izvjestaj_kpi", new { pOd = od.Date, pDo = @do.Date }, ct)).Single();

        public Task<List<PrihodProdavnice>> PrihodPoProdavnicamaAsync(DateTime od, DateTime @do, CancellationToken ct = default) =>
            baza.ProceduraUpitAsync<PrihodProdavnice>("izvjestaj_prihod_po_prodavnici", new { pOd = od.Date, pDo = @do.Date }, ct);

        public Task<List<PrihodDana>> PrihodPoDanimaAsync(DateTime od, DateTime @do, CancellationToken ct = default) =>
            baza.ProceduraUpitAsync<PrihodDana>("izvjestaj_prihod_po_danima", new { pOd = od.Date, pDo = @do.Date }, ct);

        public Task<List<NajprodavanijiProizvod>> NajprodavanijiAsync(DateTime od, DateTime @do, int limit = 10, CancellationToken ct = default) =>
            baza.ProceduraUpitAsync<NajprodavanijiProizvod>("izvjestaj_najprodavaniji", new { pOd = od.Date, pDo = @do.Date, pLimit = limit }, ct);

        public Task<List<PromjenaZaliha>> IstorijaZalihaAsync(DateTime od, DateTime @do, int? idProdavnice = null, CancellationToken ct = default) =>
            baza.ProceduraUpitAsync<PromjenaZaliha>("istorija_zaliha", new { pOd = od.Date, pDo = @do.Date, pIdProdavnice = idProdavnice }, ct);
    }
}
