using OnlineKupovinaDB;
using OnlineKupovinaGUI.Ui;

namespace OnlineKupovinaGUI.Views.Stranice
{
    internal sealed class ProdavniceStranica : TabelaStranica<Prodavnica>
    {
        public ProdavniceStranica(Kontekst k) : base(k)
        {
            DodajAlat("Nova prodavnica", Ikona.Dodaj, NovaAsync, Varijanta.Primarno);
            DodajAlat("Izmijeni", Ikona.Izmijeni, IzmijeniAsync, trebaOdabir: true);
            DodajAlat("Obriši", Ikona.Obrisi, ObrisiAsync, trebaOdabir: true);
        }

        public override string Naslov => "Prodavnice";
        public override string? Opis => "Prodavnice lanca sa adresama, brojem zaposlenih i proizvoda.";

        protected override IEnumerable<DataGridViewColumn> Kolone() =>
        [
            Tabela.Kolona("Naziv", nameof(Prodavnica.Naziv), 140, podebljano: true),
            Tabela.Kolona("Ulica", nameof(Prodavnica.Ulica), 140),
            Tabela.Kolona("Grad", nameof(Prodavnica.Grad), 90),
            Tabela.Kolona("Država", nameof(Prodavnica.Drzava), 70),
            Tabela.Broj("Zaposlenih", nameof(Prodavnica.BrojZaposlenih)),
            Tabela.Broj("Proizvoda", nameof(Prodavnica.BrojProizvoda)),
        ];

        protected override Task<List<Prodavnica>> UcitajPodatkeAsync() => K.Repo.Prodavnice.SveAsync();
        protected override string TekstZaPretragu(Prodavnica p) => $"{p.Naziv} {p.Grad} {p.Ulica} {p.Drzava}";
        protected override Task DvostrukiKlikAsync() => IzmijeniAsync();

        private bool Uredi(string naslov, Prodavnica p, Func<Prodavnica, Task> sacuvaj)
        {
            var naziv = new TekstPolje("Naziv", p.Naziv, puna: true, maxDuzina: 45);
            var ulica = new TekstPolje("Ulica i broj", p.Ulica, puna: true, maxDuzina: 100);
            var grad = new TekstPolje("Grad", p.Grad, maxDuzina: 45);
            var drzava = new TekstPolje("Država", p.Drzava, maxDuzina: 45);
            return Dijalog.Prikazi(Vlasnik, naslov, [naziv, ulica, grad, drzava], () => sacuvaj(new Prodavnica
            {
                Id = p.Id, Naziv = naziv.Vrijednost, Ulica = ulica.Vrijednost, Grad = grad.Vrijednost, Drzava = drzava.Vrijednost
            }));
        }

        private async Task NovaAsync()
        {
            if (Uredi("Nova prodavnica", new Prodavnica { Drzava = "BiH" }, p => K.Repo.Prodavnice.DodajAsync(p)))
            {
                K.Shell.Obavijesti("Prodavnica je dodana.", VrstaObavjestenja.Uspjeh);
                await UcitajAsync();
            }
        }

        private async Task IzmijeniAsync()
        {
            if (Odabrani is { } p && Uredi("Izmjena prodavnice", p, x => K.Repo.Prodavnice.IzmijeniAsync(x)))
            {
                K.Shell.Obavijesti("Izmjene su sačuvane.", VrstaObavjestenja.Uspjeh);
                await UcitajAsync();
            }
        }

        private async Task ObrisiAsync()
        {
            if (Odabrani is not { } p || !Dijalog.Potvrdi(Vlasnik, "Brisanje prodavnice", $"Obrisati prodavnicu „{p.Naziv}”?")) return;
            if (await RadiAsync(() => K.Repo.Prodavnice.ObrisiAsync(p.Id), "Prodavnica je obrisana.")) await UcitajAsync();
        }
    }

    internal sealed class ZaposleniStranica : TabelaStranica<Zaposleni>
    {
        private readonly ComboBox filter;
        private List<Prodavnica> prodavnice = new();

        public ZaposleniStranica(Kontekst k) : base(k)
        {
            filter = Raspored.Izbor(200);
            filter.SelectedIndexChanged += async (_, _) => { if (filter.Focused) await UcitajAsync(); };
            Alati.Controls.Add(filter);
            DodajAlat("Novi zaposleni", Ikona.Dodaj, NoviAsync, Varijanta.Primarno);
            DodajAlat("Izmijeni", Ikona.Izmijeni, IzmijeniAsync, trebaOdabir: true);
            DodajAlat("Nova lozinka", Ikona.Brava, LozinkaAsync, trebaOdabir: true);
            DodajAlat("Obriši", Ikona.Obrisi, ObrisiAsync, trebaOdabir: true);
        }

        public override string Naslov => "Zaposleni";
        public override string? Opis => "Radnici po prodavnicama. Radnik vidi i uređuje samo proizvode svoje prodavnice.";

        protected override IEnumerable<DataGridViewColumn> Kolone() =>
        [
            Tabela.Kolona("Ime i prezime", nameof(Zaposleni.PunoIme), 120, podebljano: true),
            Tabela.Kolona("Korisničko ime", nameof(Zaposleni.KorisnickoIme), 80),
            Tabela.Kolona("Email", nameof(Zaposleni.Email), 140),
            Tabela.Kolona("Telefon", nameof(Zaposleni.BrojTelefona), 80),
            Tabela.Kolona("Prodavnica", nameof(Zaposleni.Prodavnica), 110),
            Tabela.Novac("Plata (KM)", nameof(Zaposleni.Plata), 70),
        ];

        protected override async Task<List<Zaposleni>> UcitajPodatkeAsync()
        {
            if (prodavnice.Count == 0)
            {
                prodavnice = await K.Repo.Prodavnice.SveAsync();
                filter.Items.Add("Sve prodavnice");
                filter.Items.AddRange(prodavnice.ToArray<object>());
                filter.SelectedIndex = 0;
            }
            int? id = filter.SelectedItem is Prodavnica p ? p.Id : null;
            return await K.Repo.Korisnici.ZaposleniAsync(id);
        }

        protected override string TekstZaPretragu(Zaposleni z) => $"{z.PunoIme} {z.KorisnickoIme} {z.Email} {z.Prodavnica}";
        protected override Task DvostrukiKlikAsync() => IzmijeniAsync();

        private bool Uredi(string naslov, Zaposleni z, bool novi, Func<Zaposleni, string, Task> sacuvaj)
        {
            var ime = new TekstPolje("Ime", z.Ime, maxDuzina: 50);
            var prezime = new TekstPolje("Prezime", z.Prezime, maxDuzina: 50);
            var korisnicko = new TekstPolje("Korisničko ime", z.KorisnickoIme, maxDuzina: 50);
            var email = new TekstPolje("Email", z.Email, maxDuzina: 100);
            var telefon = new TekstPolje("Telefon", z.BrojTelefona, maxDuzina: 20, placeholder: "065 123 456");
            var plata = new BrojPolje("Plata (KM)", z.Plata == 0 ? 1200 : z.Plata, 0, 1_000_000, 2);
            var prodavnica = new IzborPolje<Prodavnica>("Prodavnica", prodavnice, p => p.Naziv, p => p.Id == z.IdProdavnice, puna: !novi);
            var lozinka = new TekstPolje("Početna lozinka", null, lozinka: true) { Pomoc = "Najmanje 8 znakova, slovo i cifra." };
            var polja = new List<Polje> { ime, prezime, korisnicko, email, telefon, plata, prodavnica };
            if (novi) polja.Add(lozinka);

            return Dijalog.Prikazi(Vlasnik, naslov, polja, () => sacuvaj(new Zaposleni
            {
                Id = z.Id, Ime = ime.Vrijednost, Prezime = prezime.Vrijednost, KorisnickoIme = korisnicko.Vrijednost.ToLowerInvariant(),
                Email = email.Vrijednost, BrojTelefona = telefon.Vrijednost, Plata = plata.Vrijednost, IdProdavnice = prodavnica.Vrijednost?.Id ?? 0
            }, lozinka.Sirovo));
        }

        private async Task NoviAsync()
        {
            if (Uredi("Novi zaposleni", new Zaposleni { IdProdavnice = (filter.SelectedItem as Prodavnica)?.Id ?? 0 }, true,
                    (z, l) => K.Repo.Korisnici.DodajZaposlenogAsync(z, l)))
            {
                K.Shell.Obavijesti("Zaposleni je dodan.", VrstaObavjestenja.Uspjeh);
                await UcitajAsync();
            }
        }

        private async Task IzmijeniAsync()
        {
            if (Odabrani is { } z && Uredi("Izmjena zaposlenog", z, false, (n, _) => K.Repo.Korisnici.IzmijeniZaposlenogAsync(n)))
            {
                K.Shell.Obavijesti("Izmjene su sačuvane.", VrstaObavjestenja.Uspjeh);
                await UcitajAsync();
            }
        }

        private Task LozinkaAsync()
        {
            if (Odabrani is { } z) NovaLozinka.Prikazi(Vlasnik, K, Uloga.Zaposleni, z.KorisnickoIme, z.PunoIme);
            return Task.CompletedTask;
        }

        private async Task ObrisiAsync()
        {
            if (Odabrani is not { } z || !Dijalog.Potvrdi(Vlasnik, "Brisanje zaposlenog", $"Obrisati nalog {z.PunoIme} ({z.KorisnickoIme})?")) return;
            if (await RadiAsync(() => K.Repo.Korisnici.ObrisiZaposlenogAsync(z.Id), "Zaposleni je obrisan.")) await UcitajAsync();
        }
    }

    internal sealed class KupciStranica : TabelaStranica<Kupac>
    {
        public KupciStranica(Kontekst k) : base(k)
        {
            DodajAlat("Novi kupac", Ikona.Dodaj, NoviAsync, Varijanta.Primarno);
            DodajAlat("Izmijeni", Ikona.Izmijeni, IzmijeniAsync, trebaOdabir: true);
            DodajAlat("Nova lozinka", Ikona.Brava, LozinkaAsync, trebaOdabir: true);
            DodajAlat("Obriši", Ikona.Obrisi, ObrisiAsync, trebaOdabir: true);
        }

        public override string Naslov => "Kupci";
        public override string? Opis => "Registrovani kupci, broj plaćenih računa i ukupna potrošnja.";

        protected override IEnumerable<DataGridViewColumn> Kolone() =>
        [
            Tabela.Kolona("Korisničko ime", nameof(Kupac.KorisnickoIme), 90, podebljano: true),
            Tabela.Kolona("Email", nameof(Kupac.Email), 130),
            Tabela.Kolona("Adresa", nameof(Kupac.PunaAdresa), 170),
            Tabela.Broj("Računa", nameof(Kupac.BrojRacuna)),
            Tabela.Novac("Potrošeno (KM)", nameof(Kupac.Potroseno), 70, podebljano: true),
        ];

        protected override Task<List<Kupac>> UcitajPodatkeAsync() => K.Repo.Korisnici.KupciAsync();
        protected override string TekstZaPretragu(Kupac k) => $"{k.KorisnickoIme} {k.Email} {k.PunaAdresa}";
        protected override Task DvostrukiKlikAsync() => IzmijeniAsync();

        private bool Uredi(string naslov, Kupac k, bool novi, Func<Kupac, string, Task> sacuvaj)
        {
            var korisnicko = new TekstPolje("Korisničko ime", k.KorisnickoIme, maxDuzina: 50);
            var email = new TekstPolje("Email", k.Email, maxDuzina: 100);
            var ulica = new TekstPolje("Ulica i broj", k.Ulica, puna: true, maxDuzina: 100);
            var grad = new TekstPolje("Grad", k.Grad, maxDuzina: 45);
            var drzava = new TekstPolje("Država", k.Drzava, maxDuzina: 45);
            var lozinka = new TekstPolje("Početna lozinka", null, lozinka: true, puna: true) { Pomoc = "Najmanje 8 znakova, slovo i cifra." };
            var polja = new List<Polje> { korisnicko, email, ulica, grad, drzava };
            if (novi) polja.Add(lozinka);

            return Dijalog.Prikazi(Vlasnik, naslov, polja, () => sacuvaj(new Kupac
            {
                Id = k.Id, KorisnickoIme = korisnicko.Vrijednost.ToLowerInvariant(), Email = email.Vrijednost,
                Ulica = ulica.Vrijednost, Grad = grad.Vrijednost, Drzava = drzava.Vrijednost
            }, lozinka.Sirovo));
        }

        private async Task NoviAsync()
        {
            if (Uredi("Novi kupac", new Kupac { Drzava = "BiH" }, true, (k, l) => K.Repo.Korisnici.DodajKupcaAsync(k, l)))
            {
                K.Shell.Obavijesti("Kupac je registrovan.", VrstaObavjestenja.Uspjeh);
                await UcitajAsync();
            }
        }

        private async Task IzmijeniAsync()
        {
            if (Odabrani is { } k && Uredi("Izmjena kupca", k, false, (n, _) => K.Repo.Korisnici.IzmijeniKupcaAsync(n)))
            {
                K.Shell.Obavijesti("Izmjene su sačuvane.", VrstaObavjestenja.Uspjeh);
                await UcitajAsync();
            }
        }

        private Task LozinkaAsync()
        {
            if (Odabrani is { } k) NovaLozinka.Prikazi(Vlasnik, K, Uloga.Kupac, k.KorisnickoIme, k.KorisnickoIme);
            return Task.CompletedTask;
        }

        private async Task ObrisiAsync()
        {
            if (Odabrani is not { } k || !Dijalog.Potvrdi(Vlasnik, "Brisanje kupca", $"Obrisati kupca {k.KorisnickoIme}? Kupci sa računima se ne mogu obrisati.")) return;
            if (await RadiAsync(() => K.Repo.Korisnici.ObrisiKupcaAsync(k.Id), "Kupac je obrisan.")) await UcitajAsync();
        }
    }

    internal sealed class AdministratoriStranica : TabelaStranica<Administrator>
    {
        public AdministratoriStranica(Kontekst k) : base(k)
        {
            DodajAlat("Novi administrator", Ikona.Dodaj, NoviAsync, Varijanta.Primarno);
            DodajAlat("Izmijeni", Ikona.Izmijeni, IzmijeniAsync, trebaOdabir: true);
            DodajAlat("Nova lozinka", Ikona.Brava, LozinkaAsync, trebaOdabir: true);
            DodajAlat("Obriši", Ikona.Obrisi, ObrisiAsync, trebaOdabir: true);
        }

        public override string Naslov => "Administratori";
        public override string? Opis => "Nalozi sa punim pristupom sistemu.";

        protected override IEnumerable<DataGridViewColumn> Kolone() =>
        [
            Tabela.Kolona("Ime i prezime", nameof(Administrator.PunoIme), 120, podebljano: true),
            Tabela.Kolona("Korisničko ime", nameof(Administrator.KorisnickoIme), 90),
            Tabela.Kolona("Email", nameof(Administrator.Email), 160),
        ];

        protected override Task<List<Administrator>> UcitajPodatkeAsync() => K.Repo.Korisnici.AdministratoriAsync();
        protected override string TekstZaPretragu(Administrator a) => $"{a.PunoIme} {a.KorisnickoIme} {a.Email}";
        protected override Task DvostrukiKlikAsync() => IzmijeniAsync();

        private bool Uredi(string naslov, Administrator a, bool novi, Func<Administrator, string, Task> sacuvaj)
        {
            var ime = new TekstPolje("Ime", a.Ime, maxDuzina: 50);
            var prezime = new TekstPolje("Prezime", a.Prezime, maxDuzina: 50);
            var korisnicko = new TekstPolje("Korisničko ime", a.KorisnickoIme, maxDuzina: 50);
            var email = new TekstPolje("Email", a.Email, maxDuzina: 100);
            var lozinka = new TekstPolje("Početna lozinka", null, lozinka: true, puna: true) { Pomoc = "Najmanje 8 znakova, slovo i cifra." };
            var polja = new List<Polje> { ime, prezime, korisnicko, email };
            if (novi) polja.Add(lozinka);

            return Dijalog.Prikazi(Vlasnik, naslov, polja, () => sacuvaj(new Administrator
            {
                Id = a.Id, Ime = ime.Vrijednost, Prezime = prezime.Vrijednost, KorisnickoIme = korisnicko.Vrijednost.ToLowerInvariant(), Email = email.Vrijednost
            }, lozinka.Sirovo));
        }

        private async Task NoviAsync()
        {
            if (Uredi("Novi administrator", new Administrator(), true, (a, l) => K.Repo.Korisnici.DodajAdministratoraAsync(a, l)))
            {
                K.Shell.Obavijesti("Administrator je dodan.", VrstaObavjestenja.Uspjeh);
                await UcitajAsync();
            }
        }

        private async Task IzmijeniAsync()
        {
            if (Odabrani is { } a && Uredi("Izmjena administratora", a, false, (n, _) => K.Repo.Korisnici.IzmijeniAdministratoraAsync(n)))
            {
                K.Shell.Obavijesti("Izmjene su sačuvane.", VrstaObavjestenja.Uspjeh);
                await UcitajAsync();
            }
        }

        private Task LozinkaAsync()
        {
            if (Odabrani is { } a) NovaLozinka.Prikazi(Vlasnik, K, Uloga.Administrator, a.KorisnickoIme, a.PunoIme);
            return Task.CompletedTask;
        }

        private async Task ObrisiAsync()
        {
            if (Odabrani is not { } a || !Dijalog.Potvrdi(Vlasnik, "Brisanje administratora", $"Obrisati nalog {a.PunoIme}?")) return;
            if (await RadiAsync(() => K.Repo.Korisnici.ObrisiAdministratoraAsync(a.Id, K.Sesija.Id), "Administrator je obrisan.")) await UcitajAsync();
        }
    }

    internal static class NovaLozinka
    {
        public static void Prikazi(IWin32Window vlasnik, Kontekst k, Uloga uloga, string korisnickoIme, string ime)
        {
            var lozinka = new TekstPolje("Nova lozinka", null, lozinka: true) { Pomoc = "Najmanje 8 znakova, slovo i cifra." };
            var potvrda = new TekstPolje("Ponovite lozinku", null, lozinka: true);
            if (Dijalog.Prikazi(vlasnik, "Nova lozinka", [lozinka, potvrda], async () =>
                {
                    if (lozinka.Sirovo != potvrda.Sirovo) throw new ArgumentException("Lozinke se ne poklapaju.");
                    await k.Repo.Auth.PostaviLozinkuAsync(uloga, korisnickoIme, lozinka.Sirovo);
                }, $"Postavljanje lozinke za {ime}. Korisnik je može promijeniti u podešavanjima."))
            {
                k.Shell.Obavijesti("Lozinka je promijenjena.", VrstaObavjestenja.Uspjeh);
            }
        }
    }
}
