using Dapper;
using MySqlConnector;
using OnlineKupovina.Data;
using OnlineKupovinaDB;
using Testcontainers.MySql;

namespace OnlineKupovina.Tests.Database
{
    /// <summary>
    /// Pokrece MySQL 8.4 u Dockeru i izvrsava skripte iz database/ istim redoslijedom
    /// kao docker-compose: sema, funkcije, procedure, trigeri, pogledi, demo podaci.
    /// Jedan kontejner za sve testove baze; testovi prave svoje podatke.
    /// </summary>
    public sealed class MySqlFixture : IAsyncLifetime
    {
        private MySqlContainer? container;
        private Exception? startupError;
        private Repozitoriji? repo;

        public string ConnectionString { get; private set; } = string.Empty;

        public Repozitoriji Repo
        {
            get
            {
                EnsureAvailable();
                return repo!;
            }
        }

        public async ValueTask InitializeAsync()
        {
            try
            {
                container = new MySqlBuilder("mysql:8.4")
                    .WithDatabase("prodavnica")
                    .WithUsername("prodavnica")
                    .WithPassword("prodavnica")
                    .WithCommand("--log-bin-trust-function-creators=1")
                    .WithResourceMapping(new DirectoryInfo(RepositoryPaths.DatabaseScripts), "/docker-entrypoint-initdb.d/")
                    .Build();
                await container.StartAsync();
                ConnectionString = container.GetConnectionString();
                repo = new Repozitoriji(ConnectionString);
            }
            catch (Exception e)
            {
                startupError = e;
            }
        }

        /// <summary>Lokalno bez Dockera testovi baze se preskacu; u CI-ju (CI=true) padaju.</summary>
        public void EnsureAvailable()
        {
            if (startupError == null) return;
            if (Environment.GetEnvironmentVariable("CI") == "true")
            {
                throw new InvalidOperationException("MySQL kontejner nije pokrenut", startupError);
            }
            Assert.Skip($"Docker nije dostupan: {startupError.Message}");
        }

        public MySqlConnection Open()
        {
            EnsureAvailable();
            var connection = new MySqlConnection(new MySqlConnectionStringBuilder(ConnectionString) { AllowUserVariables = true }.ConnectionString);
            connection.Open();
            return connection;
        }

        public T Scalar<T>(string sql, object? parametri = null)
        {
            using var k = Open();
            return k.ExecuteScalar<T>(sql, parametri)!;
        }

        public int Execute(string sql, object? parametri = null)
        {
            using var k = Open();
            return k.Execute(sql, parametri);
        }

        /// <summary>SQL koji treba da padne zbog SIGNAL-a ili ogranicenja; vraca poruku greske.</summary>
        public string Fails(string sql, object? parametri = null)
        {
            using var k = Open();
            var e = Assert.Throws<MySqlException>(() => k.Execute(sql, parametri));
            return e.Message;
        }

        public static string NoviBarkod() => Barkod.GenerisiInterni();

        public static string Jedinstveno(string prefiks) => prefiks + Guid.NewGuid().ToString("N")[..8];

        /// <summary>Novi proizvod sa jedinstvenim bar kodom, da testovi ne dijele zalihe.</summary>
        public async Task<Proizvod> NoviProizvodAsync(int idProdavnice = 1, int zalihe = 10, decimal cijena = 2.00m, int minZalihe = 3)
        {
            var p = new Proizvod
            {
                Naziv = Jedinstveno("Test "), BarKod = NoviBarkod(), Cijena = cijena, Zalihe = zalihe, MinZalihe = minZalihe,
                IdProizvodjaca = 1, IdProdavnice = idProdavnice, DatumIsteka = DateTime.Today.AddYears(1), Opis = "Test"
            };
            p.Id = await Repo.Proizvodi.DodajAsync(p, idZaposlenog: null);
            return (await Repo.Proizvodi.PoBarkoduAsync(idProdavnice, p.BarKod))!;
        }

        public int Zalihe(int idProizvoda) => Scalar<int>("SELECT Zalihe FROM proizvod WHERE idPROIZVOD = @id", new { id = idProizvoda });

        public async ValueTask DisposeAsync()
        {
            if (container != null)
            {
                await container.DisposeAsync();
            }
        }
    }

    [CollectionDefinition(Name)]
    public sealed class DatabaseCollection : ICollectionFixture<MySqlFixture>
    {
        public const string Name = "Database";
    }
}
