using System.Text.RegularExpressions;

namespace OnlineKupovinaDB
{
    /// <summary>
    /// Pravila unosa koja forma provjerava prije slanja u bazu.
    /// Ista pravila baza provjerava i sama (trigeri, CHECK), ovo je samo da korisnik
    /// dobije poruku odmah i za sva polja odjednom.
    /// </summary>
    public static partial class Validacija
    {
        public static void Obavezno(List<string> greske, string? vrijednost, string polje, int maxDuzina)
        {
            if (string.IsNullOrWhiteSpace(vrijednost))
            {
                greske.Add($"{polje} je obavezno polje.");
            }
            else if (vrijednost.Trim().Length > maxDuzina)
            {
                greske.Add($"{polje} može imati najviše {maxDuzina} znakova.");
            }
        }

        public static void Opciono(List<string> greske, string? vrijednost, string polje, int maxDuzina)
        {
            if (vrijednost != null && vrijednost.Trim().Length > maxDuzina)
            {
                greske.Add($"{polje} može imati najviše {maxDuzina} znakova.");
            }
        }

        public static void Email(List<string> greske, string? email)
        {
            Obavezno(greske, email, "Email", 100);
            if (!string.IsNullOrWhiteSpace(email) && !EmailRegex().IsMatch(email.Trim()))
            {
                greske.Add("Email adresa nije ispravna.");
            }
        }

        public static void KorisnickoIme(List<string> greske, string? korisnickoIme)
        {
            if (string.IsNullOrWhiteSpace(korisnickoIme) || !KorisnickoImeRegex().IsMatch(korisnickoIme.Trim().ToLowerInvariant()))
            {
                greske.Add("Korisničko ime: 3-50 znakova, slova, cifre, tačka, crtica ili donja crta.");
            }
        }

        public static void Telefon(List<string> greske, string? telefon)
        {
            if (!string.IsNullOrWhiteSpace(telefon) && !TelefonRegex().IsMatch(telefon.Trim()))
            {
                greske.Add("Broj telefona smije sadržati samo cifre, razmake, +, / i -.");
            }
        }

        public static void Raspon(List<string> greske, decimal vrijednost, decimal min, decimal max, string polje)
        {
            if (vrijednost < min || vrijednost > max)
            {
                greske.Add($"{polje} mora biti između {min} i {max}.");
            }
        }

        /// <summary>Lozinka: najmanje 8 znakova, bar jedno slovo i jedna cifra.</summary>
        public static List<string> Lozinka(string? lozinka, string? potvrda = null)
        {
            var greske = new List<string>();
            if (string.IsNullOrEmpty(lozinka) || lozinka.Length < 8)
            {
                greske.Add("Lozinka mora imati najmanje 8 znakova.");
            }
            else if (!lozinka.Any(char.IsLetter) || !lozinka.Any(char.IsDigit))
            {
                greske.Add("Lozinka mora sadržati bar jedno slovo i jednu cifru.");
            }
            if (potvrda != null && lozinka != potvrda)
            {
                greske.Add("Lozinke se ne poklapaju.");
            }
            return greske;
        }

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex EmailRegex();

        [GeneratedRegex(@"^[a-z0-9._-]{3,50}$")]
        private static partial Regex KorisnickoImeRegex();

        [GeneratedRegex(@"^[0-9 +/-]{6,20}$")]
        private static partial Regex TelefonRegex();
    }
}
