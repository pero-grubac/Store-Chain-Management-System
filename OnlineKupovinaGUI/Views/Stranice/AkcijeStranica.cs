using OnlineKupovinaDB;
using OnlineKupovinaGUI.Ui;

namespace OnlineKupovinaGUI.Views.Stranice
{
    /// <summary>Akcije (popust na proizvod u periodu) i kuponi (sifra koju kupac upise u korpi).</summary>
    internal sealed class AkcijeStranica : Stranica
    {
        private readonly DataGridView akcije;
        private readonly DataGridView kuponi;
        private readonly Dugme obrisiAkciju;
        private readonly Dugme dodajProizvod;
        private readonly Dugme obrisiKupon;
        private List<Proizvod> proizvodi = new();

        public AkcijeStranica(Kontekst k) : base(k)
        {
            akcije = Tabela.Nova();
            akcije.Columns.AddRange(
                Tabela.Kolona("Proizvod", nameof(Akcija.Proizvod), 130, podebljano: true),
                Tabela.Kolona("Prodavnica", nameof(Akcija.Prodavnica), 110),
                Tabela.Kolona("Popust", nameof(Akcija.Popust), 45, "0'%'", DataGridViewContentAlignment.MiddleRight),
                Tabela.Datum("Od", nameof(Akcija.Od), 60),
                Tabela.Datum("Do", nameof(Akcija.Do), 60),
                Tabela.Kolona("Status", nameof(Akcija.Status), 55));
            Tabela.BojaStatusa(akcije, nameof(Akcija.Status));

            kuponi = Tabela.Nova();
            kuponi.Columns.AddRange(
                Tabela.Kolona("Šifra", nameof(Kupon.Sifra), 70, podebljano: true),
                Tabela.Kolona("Popust", nameof(Kupon.Popust), 45, "0'%'", DataGridViewContentAlignment.MiddleRight),
                Tabela.Datum("Od", nameof(Kupon.Od), 60),
                Tabela.Datum("Do", nameof(Kupon.Do), 60),
                Tabela.Kolona("Status", nameof(Kupon.Status), 55),
                Tabela.Kolona("Važi za", nameof(Kupon.Proizvodi), 200));
            Tabela.BojaStatusa(kuponi, nameof(Kupon.Status));

            var novaAkcija = new Dugme("Nova akcija", Ikona.Dodaj, Varijanta.Primarno);
            novaAkcija.Click += async (_, _) => await NovaAkcijaAsync();
            obrisiAkciju = new Dugme("Obriši", Ikona.Obrisi);
            obrisiAkciju.Click += async (_, _) => await ObrisiAkcijuAsync();
            var noviKupon = new Dugme("Novi kupon", Ikona.Dodaj, Varijanta.Primarno);
            noviKupon.Click += async (_, _) => await NoviKuponAsync();
            dodajProizvod = new Dugme("Dodaj proizvod", Ikona.Proizvod);
            dodajProizvod.Click += async (_, _) => await DodajProizvodAsync();
            obrisiKupon = new Dugme("Obriši", Ikona.Obrisi);
            obrisiKupon.Click += async (_, _) => await ObrisiKuponAsync();

            akcije.SelectionChanged += (_, _) => OsvjeziDugmad();
            kuponi.SelectionChanged += (_, _) => OsvjeziDugmad();

            var mreza = Raspored.Mreza(1, 2);
            mreza.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            mreza.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            mreza.Controls.Add(Sekcija("Akcije", "popust na proizvod u periodu, važi automatski", akcije, novaAkcija, obrisiAkciju), 0, 0);
            var donja = Sekcija("Kuponi", "kupac upisuje šifru u korpi", kuponi, noviKupon, dodajProizvod, obrisiKupon);
            donja.Margin = new Padding(0);
            mreza.Controls.Add(donja, 0, 1);
            Controls.Add(mreza);
        }

        public override string Naslov => "Akcije i kuponi";
        public override string? Opis => "Kad se poklope akcija i kupon, kupac dobija veći popust. Cijenu na računu računa baza.";

        private static Kartica Sekcija(string naslov, string podnaslov, DataGridView tabela, params Dugme[] dugmad)
        {
            var k = new Kartica(naslov) { Dock = DockStyle.Fill, Podnaslov = "· " + podnaslov, Margin = new Padding(0, 0, 0, 16) };
            var alati = Raspored.Red(dugmad);
            alati.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            k.Resize += (_, _) => alati.Location = new Point(k.Width - alati.Width - 18, 8);
            tabela.Dock = DockStyle.Fill;
            k.Controls.Add(tabela);
            k.Controls.Add(alati);
            alati.BringToFront();
            return k;
        }

        private void OsvjeziDugmad()
        {
            obrisiAkciju.Enabled = Tabela.Odabrani<Akcija>(akcije) != null;
            dodajProizvod.Enabled = obrisiKupon.Enabled = Tabela.Odabrani<Kupon>(kuponi) != null;
        }

        public override Task UcitajAsync() => RadiAsync(async () =>
        {
            var a = K.Repo.Akcije.AkcijeAsync();
            var k = K.Repo.Akcije.KuponiAsync();
            var p = K.Repo.Proizvodi.SviAktivniAsync();
            await Task.WhenAll(a, k, p);
            Tabela.Postavi(akcije, a.Result);
            Tabela.Postavi(kuponi, k.Result);
            proizvodi = p.Result;
            OsvjeziDugmad();
        });

        private IzborPolje<Proizvod> IzborProizvoda() =>
            new("Proizvod", proizvodi, p => $"{p.Naziv} — {p.Prodavnica} ({Cijene.KM(p.Cijena)})", puna: true);

        private async Task NovaAkcijaAsync()
        {
            var proizvod = IzborProizvoda();
            var popust = new BrojPolje("Popust (%)", 10, 1, 90, puna: true);
            var od = new DatumPolje("Od", DateTime.Today);
            var @do = new DatumPolje("Do", DateTime.Today.AddDays(14));
            if (Dijalog.Prikazi(Vlasnik, "Nova akcija", [proizvod, popust, od, @do], () => K.Repo.Akcije.DodajAkcijuAsync(new Akcija
                {
                    IdProizvoda = proizvod.Vrijednost?.Id ?? 0, Popust = popust.Cijeli, Od = od.Vrijednost!.Value, Do = @do.Vrijednost!.Value
                }), "Akcija važi za proizvod u jednoj prodavnici, svaki dan u periodu."))
            {
                K.Shell.Obavijesti("Akcija je dodana.", VrstaObavjestenja.Uspjeh);
                await UcitajAsync();
            }
        }

        private async Task ObrisiAkcijuAsync()
        {
            if (Tabela.Odabrani<Akcija>(akcije) is not { } a ||
                !Dijalog.Potvrdi(Vlasnik, "Brisanje akcije", $"Obrisati akciju {a.Popust}% na {a.Proizvod} ({a.Prodavnica})?")) return;
            if (await RadiAsync(() => K.Repo.Akcije.ObrisiAkcijuAsync(a.Id), "Akcija je obrisana.")) await UcitajAsync();
        }

        private async Task NoviKuponAsync()
        {
            var sifra = new TekstPolje("Šifra", null, placeholder: "npr. ZIMA20", maxDuzina: 20);
            var popust = new BrojPolje("Popust (%)", 10, 1, 90);
            var od = new DatumPolje("Važi od", DateTime.Today);
            var @do = new DatumPolje("Važi do", DateTime.Today.AddMonths(1), opciono: true) { Pomoc = "Bez datuma kupon ne ističe." };
            var proizvod = IzborProizvoda();
            if (Dijalog.Prikazi(Vlasnik, "Novi kupon", [sifra, popust, od, @do, proizvod], () => K.Repo.Akcije.KreirajKuponAsync(new Kupon
                {
                    Sifra = sifra.Vrijednost, Popust = popust.Cijeli, Od = od.Vrijednost!.Value, Do = @do.Vrijednost
                }, proizvod.Vrijednost?.Id ?? 0), "Prvi proizvod na koji kupon važi. Ostale dodajte dugmetom „Dodaj proizvod”."))
            {
                K.Shell.Obavijesti("Kupon je kreiran.", VrstaObavjestenja.Uspjeh);
                await UcitajAsync();
            }
        }

        private async Task DodajProizvodAsync()
        {
            if (Tabela.Odabrani<Kupon>(kuponi) is not { } kupon) return;
            var proizvod = IzborProizvoda();
            if (Dijalog.Prikazi(Vlasnik, $"Kupon {kupon.Sifra}", [proizvod], () =>
                    proizvod.Vrijednost == null
                        ? throw new ArgumentException("Izaberite proizvod.")
                        : K.Repo.Akcije.DodajProizvodKuponuAsync(kupon.Id, proizvod.Vrijednost.Id),
                    "Kupon će važiti i za ovaj proizvod.", "Dodaj"))
            {
                K.Shell.Obavijesti("Proizvod je dodan kuponu.", VrstaObavjestenja.Uspjeh);
                await UcitajAsync();
            }
        }

        private async Task ObrisiKuponAsync()
        {
            if (Tabela.Odabrani<Kupon>(kuponi) is not { } k ||
                !Dijalog.Potvrdi(Vlasnik, "Brisanje kupona", $"Obrisati kupon {k.Sifra}? Računi koji su ga koristili ostaju, bez oznake kupona.")) return;
            if (await RadiAsync(() => K.Repo.Akcije.ObrisiKuponAsync(k.Id), "Kupon je obrisan.")) await UcitajAsync();
        }
    }
}
