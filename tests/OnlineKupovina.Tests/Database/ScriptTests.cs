using System.Text.RegularExpressions;

namespace OnlineKupovina.Tests.Database
{
    /// <summary>Provjere SQL skripti koje ne traze bazu.</summary>
    public class ScriptTests
    {
        private static string Read(string file) => File.ReadAllText(Path.Combine(RepositoryPaths.DatabaseScripts, file));

        [Fact]
        public void Scripts_RunInDependencyOrder()
        {
            var files = Directory.GetFiles(RepositoryPaths.DatabaseScripts, "*.sql").Select(Path.GetFileName).Order().ToArray();

            Assert.Equal(["01-schema.sql", "02-functions.sql", "03-procedures.sql", "04-triggers.sql", "05-views.sql", "06-seed.sql"], files);
        }

        [Fact]
        public void Scripts_HaveNoDefiner()
        {
            // DEFINER vezan za lokalnog korisnika ne postoji na novoj instalaciji i objekat tada ne radi
            foreach (var file in Directory.GetFiles(RepositoryPaths.DatabaseScripts, "*.sql"))
            {
                Assert.DoesNotMatch(new Regex("DEFINER\\s*=", RegexOptions.IgnoreCase), File.ReadAllText(file));
            }
        }

        [Fact]
        public void Scripts_UseLowercaseTableNames_SoTheyRunOnLinuxMySql()
        {
            foreach (var file in Directory.GetFiles(RepositoryPaths.DatabaseScripts, "*.sql"))
            {
                Assert.DoesNotMatch(new Regex(@"\bProdavnica\.[A-Za-z]"), File.ReadAllText(file));
            }
        }

        [Fact]
        public void TriggerScript_DefinesAllTriggers()
        {
            Assert.Equal(21, Regex.Matches(Read("04-triggers.sql"), @"^CREATE TRIGGER", RegexOptions.Multiline).Count);
        }

        [Fact]
        public void Seed_PasswordsAreWellFormedBcryptHashes()
        {
            var hashes = Regex.Matches(Read("06-seed.sql"), "'(\\$2[aby]\\$\\d{2}\\$[./A-Za-z0-9]{53})'");

            Assert.Equal(3, hashes.Count);
            Assert.All(hashes, h => Assert.True(OnlineKupovina.Data.Lozinke.Provjeri(
                h.Groups[1].Value.StartsWith("$2a$10$JiB") ? "Admin123!" : h.Groups[1].Value.StartsWith("$2a$10$XtK") ? "Radnik123!" : "Kupac123!",
                h.Groups[1].Value)));
        }

        [Fact]
        public void Seed_BarcodesAreValidEan13()
        {
            var kodovi = Regex.Matches(Read("06-seed.sql"), "'(\\d{13})'").Select(m => m.Groups[1].Value).Distinct().ToList();

            Assert.True(kodovi.Count >= 14);
            Assert.All(kodovi, k => Assert.True(OnlineKupovinaDB.Barkod.ImaIspravnuKontrolnuCifru(k), k));
        }
    }
}
