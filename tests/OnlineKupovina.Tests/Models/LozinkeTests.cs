using OnlineKupovina.Data;

namespace OnlineKupovina.Tests.Models
{
    public class LozinkeTests
    {
        [Fact]
        public void Hash_IsBcrypt_AndVerifies()
        {
            string hash = Lozinke.Hash("Tajna123!");

            Assert.StartsWith("$2", hash);
            Assert.Equal(60, hash.Length);
            Assert.True(Lozinke.Provjeri("Tajna123!", hash));
            Assert.False(Lozinke.Provjeri("tajna123!", hash));
        }

        [Fact]
        public void Hash_UsesNewSaltEachTime()
        {
            Assert.NotEqual(Lozinke.Hash("ista1234"), Lozinke.Hash("ista1234"));
        }

        [Theory]
        [InlineData("", "$2a$10$JiBMEXrmsh6JFGj1nZO3aeOq2N8wxjQVIqe7XUw8LXPOnsvjwEOEu")]
        [InlineData("Admin123!", "")]
        [InlineData("Admin123!", null)]
        [InlineData("Admin123!", "nije-hash")]
        public void Verify_RejectsEmptyOrMalformedInput(string? lozinka, string? hash)
        {
            Assert.False(Lozinke.Provjeri(lozinka, hash));
        }

        [Fact]
        public void SeededAdminHash_MatchesDocumentedPassword()
        {
            Assert.True(Lozinke.Provjeri("Admin123!", "$2a$10$JiBMEXrmsh6JFGj1nZO3aeOq2N8wxjQVIqe7XUw8LXPOnsvjwEOEu"));
        }

        [Fact]
        public void BusinessErrors_AreCollectedIntoOneMessage()
        {
            var e = Assert.Throws<PoslovnaGreskaException>(() => Greske.AkoIma(["Prva greška.", "Druga greška."]));

            Assert.Equal($"Prva greška.{Environment.NewLine}Druga greška.", e.Message);
            Greske.AkoIma([]);
        }
    }
}
