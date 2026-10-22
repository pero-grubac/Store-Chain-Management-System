namespace OnlineKupovinaDB
{
    public class StavkaKorpe
    {
        public int IdProizvoda { get; init; }
        public string Naziv { get; init; } = string.Empty;
        public string BarKod { get; init; } = string.Empty;
        public decimal PunaCijena { get; init; }
        public int PopustAkcija { get; init; }
        public int NaStanju { get; init; }
        public int Kolicina { get; internal set; }
        public int PopustKupon { get; internal set; }

        /// <summary>Vazi veci od dva popusta, kao u proceduri narucivanje.</summary>
        public int Popust => Math.Max(PopustAkcija, PopustKupon);
        public decimal Cijena => Cijene.SaPopustom(PunaCijena, Popust);
        public decimal Ukupno => Cijena * Kolicina;
        public decimal UkupnoBezPopusta => PunaCijena * Kolicina;
    }

    /// <summary>
    /// Korpa kupca u jednoj prodavnici. Racuna isto sto i baza (najveci popust po stavci,
    /// zaokruzivanje na 2 decimale), ali konacne cijene ipak odredjuje procedura narucivanje.
    /// </summary>
    public class Korpa
    {
        private readonly List<StavkaKorpe> stavke = new();
        private Dictionary<int, int> kuponPopusti = new();

        public int? IdProdavnice { get; private set; }
        public string? Kupon { get; private set; }
        public IReadOnlyList<StavkaKorpe> Stavke => stavke;
        public bool JePrazna => stavke.Count == 0;
        public int BrojArtikala => stavke.Sum(s => s.Kolicina);

        public decimal Medjuzbir => stavke.Sum(s => s.UkupnoBezPopusta);
        public decimal Ukupno => stavke.Sum(s => s.Ukupno);
        public decimal Usteda => Medjuzbir - Ukupno;

        public void Dodaj(Proizvod proizvod, int kolicina = 1)
        {
            if (kolicina <= 0) throw new ArgumentException("Količina mora biti veća od nule.", nameof(kolicina));
            if (IdProdavnice.HasValue && IdProdavnice != proizvod.IdProdavnice)
            {
                throw new InvalidOperationException("Korpa već sadrži proizvode druge prodavnice.");
            }

            var postojeca = stavke.FirstOrDefault(s => s.IdProizvoda == proizvod.Id);
            int ukupno = (postojeca?.Kolicina ?? 0) + kolicina;
            if (ukupno > proizvod.Zalihe)
            {
                throw new InvalidOperationException($"Na stanju je samo {proizvod.Zalihe} kom. proizvoda {proizvod.Naziv}.");
            }

            IdProdavnice = proizvod.IdProdavnice;
            if (postojeca != null)
            {
                postojeca.Kolicina = ukupno;
                return;
            }
            stavke.Add(new StavkaKorpe
            {
                IdProizvoda = proizvod.Id,
                Naziv = proizvod.Naziv,
                BarKod = proizvod.BarKod,
                PunaCijena = proizvod.Cijena,
                PopustAkcija = proizvod.Popust,
                NaStanju = proizvod.Zalihe,
                Kolicina = kolicina,
                PopustKupon = kuponPopusti.GetValueOrDefault(proizvod.Id)
            });
        }

        public void PromijeniKolicinu(int idProizvoda, int kolicina)
        {
            var stavka = stavke.FirstOrDefault(s => s.IdProizvoda == idProizvoda)
                ?? throw new InvalidOperationException("Proizvod nije u korpi.");
            if (kolicina <= 0)
            {
                Ukloni(idProizvoda);
                return;
            }
            if (kolicina > stavka.NaStanju)
            {
                throw new InvalidOperationException($"Na stanju je samo {stavka.NaStanju} kom.");
            }
            stavka.Kolicina = kolicina;
        }

        public void Ukloni(int idProizvoda)
        {
            stavke.RemoveAll(s => s.IdProizvoda == idProizvoda);
            if (stavke.Count == 0) IdProdavnice = null;
        }

        public void Isprazni()
        {
            stavke.Clear();
            IdProdavnice = null;
            UkloniKupon();
        }

        /// <summary>Kupon sa popustima po proizvodu (iz procedure provjeri_kupon).</summary>
        public void PrimijeniKupon(string sifra, IReadOnlyDictionary<int, int> popustiPoProizvodu)
        {
            Kupon = OnlineKupovinaDB.Kupon.NormalizujSifru(sifra);
            kuponPopusti = new Dictionary<int, int>(popustiPoProizvodu);
            foreach (var stavka in stavke)
            {
                stavka.PopustKupon = kuponPopusti.GetValueOrDefault(stavka.IdProizvoda);
            }
        }

        public void UkloniKupon()
        {
            Kupon = null;
            kuponPopusti = new Dictionary<int, int>();
            foreach (var stavka in stavke)
            {
                stavka.PopustKupon = 0;
            }
        }

        /// <summary>JSON za proceduru narucivanje: [{"id":1,"kolicina":2}, ...]</summary>
        public string UJson() => System.Text.Json.JsonSerializer.Serialize(
            stavke.Select(s => new { id = s.IdProizvoda, kolicina = s.Kolicina }));
    }
}
