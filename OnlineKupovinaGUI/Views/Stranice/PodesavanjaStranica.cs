using OnlineKupovinaDB;
using OnlineKupovinaGUI.Ui;

namespace OnlineKupovinaGUI.Views.Stranice
{
    internal sealed class PodesavanjaStranica : Stranica
    {
        private readonly Unos trenutna;
        private readonly Unos nova;
        private readonly Unos potvrda;

        public PodesavanjaStranica(Kontekst k) : base(k)
        {
            var profil = new Kartica("Profil") { Dock = DockStyle.Fill };
            int y = 60;
            foreach (var (oznaka, vrijednost) in new[]
                     {
                         ("Ime", k.Sesija.PunoIme),
                         ("Korisničko ime", k.Sesija.KorisnickoIme),
                         ("Uloga", k.Sesija.Uloga.Naziv()),
                         ("Prodavnica", k.Sesija.ImeProdavnice ?? "–"),
                     })
            {
                profil.Controls.Add(new Label { Text = oznaka, Font = Tema.MaliPolu, ForeColor = Tema.TekstSlab, AutoSize = true, Location = new Point(18, y) });
                profil.Controls.Add(new Label { Text = vrijednost, Font = Tema.Polu, ForeColor = Tema.Tekst, AutoSize = true, Location = new Point(160, y) });
                y += 34;
            }
            profil.Controls.Add(new Label
            {
                Text = "Prečice: F11 – prikaz preko cijelog ekrana, Esc – izlaz iz tog prikaza.",
                ForeColor = Tema.TekstSlab, AutoSize = true, Location = new Point(18, y + 16)
            });

            var lozinka = new Kartica("Promjena lozinke") { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 16) };
            trenutna = new Unos("Trenutna lozinka", Ikona.Brava, lozinka: true) { Location = new Point(18, 60), Width = 360 };
            nova = new Unos("Nova lozinka", Ikona.Brava, lozinka: true) { Location = new Point(18, 112), Width = 360 };
            potvrda = new Unos("Ponovite novu lozinku", Ikona.Brava, lozinka: true) { Location = new Point(18, 164), Width = 360 };
            var pravila = new Label { Text = "Najmanje 8 znakova, bar jedno slovo i jedna cifra.", ForeColor = Tema.TekstSlab, AutoSize = true, Location = new Point(18, 212) };
            var sacuvaj = new Dugme("Promijeni lozinku", Ikona.Sacuvaj, Varijanta.Primarno) { Location = new Point(18, 244) };
            sacuvaj.Click += async (_, _) => await PromijeniAsync();
            lozinka.Controls.AddRange([trenutna, nova, potvrda, pravila, sacuvaj]);

            var mreza = Raspored.Mreza(2, 1);
            mreza.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            mreza.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            mreza.RowStyles.Add(new RowStyle(SizeType.Absolute, 320));
            mreza.Dock = DockStyle.Top;
            mreza.Height = 320;
            mreza.Controls.Add(lozinka, 0, 0);
            mreza.Controls.Add(profil, 1, 0);
            profil.Margin = new Padding(0, 0, 0, 16);
            lozinka.Margin = new Padding(0, 0, 16, 16);
            Controls.Add(mreza);
        }

        public override string Naslov => "Podešavanja";
        public override string? Opis => "Vaš nalog i lozinka.";

        public override Task UcitajAsync() => Task.CompletedTask;

        private async Task PromijeniAsync()
        {
            if (await RadiAsync(() => K.Repo.Auth.PromijeniLozinkuAsync(K.Sesija, trenutna.Text, nova.Text, potvrda.Text), "Lozinka je promijenjena."))
            {
                trenutna.Text = nova.Text = potvrda.Text = string.Empty;
            }
        }
    }
}
