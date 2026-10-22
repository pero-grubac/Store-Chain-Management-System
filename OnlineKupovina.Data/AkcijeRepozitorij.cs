using OnlineKupovinaDB;

namespace OnlineKupovina.Data
{
    public sealed class AkcijeRepozitorij(Baza baza)
    {
        public Task<List<Akcija>> AkcijeAsync(CancellationToken ct = default) =>
            baza.UpitAsync<Akcija>("SELECT * FROM v_popusti ORDER BY FIELD(Status, 'Aktivan', 'Najavljen', 'Istekao'), Od DESC", ct: ct);

        public Task DodajAkcijuAsync(Akcija a, CancellationToken ct = default)
        {
            Greske.AkoIma(a.Validiraj());
            return baza.ProceduraAsync("dodaj_popust",
                new { pIdProizvoda = a.IdProizvoda, pPopust = a.Popust, pStartDate = a.Od.Date, pEndDate = a.Do.Date }, ct);
        }

        public Task ObrisiAkcijuAsync(int id, CancellationToken ct = default) =>
            baza.ProceduraAsync("obrisi_popust", new { pIdPopusta = id }, ct);

        public Task<List<Kupon>> KuponiAsync(CancellationToken ct = default) =>
            baza.UpitAsync<Kupon>("SELECT * FROM v_kuponi ORDER BY FIELD(Status, 'Aktivan', 'Najavljen', 'Istekao'), Od DESC", ct: ct);

        public Task KreirajKuponAsync(Kupon k, int idProizvoda, CancellationToken ct = default)
        {
            Greske.AkoIma(k.Validiraj());
            if (idProizvoda <= 0) throw new PoslovnaGreskaException("Izaberite proizvod na koji kupon važi.");
            return baza.ProceduraAsync("kreiraj_kupon", new
            {
                pIdProizvoda = idProizvoda, pPopust = k.Popust, pSifra = Kupon.NormalizujSifru(k.Sifra), pStart = k.Od.Date, pEnd = k.Do?.Date
            }, ct);
        }

        public Task DodajProizvodKuponuAsync(int idKupona, int idProizvoda, CancellationToken ct = default) =>
            baza.ProceduraAsync("dodaj_proizvod_kuponu", new { pIdKupona = idKupona, pIdProizvoda = idProizvoda }, ct);

        public Task ObrisiKuponAsync(int id, CancellationToken ct = default) =>
            baza.ProceduraAsync("obrisi_kupon", new { pIdKupona = id }, ct);
    }
}
