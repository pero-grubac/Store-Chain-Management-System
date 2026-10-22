using Dapper;
using MySqlConnector;
using System.Data;

namespace OnlineKupovina.Data
{
    /// <summary>
    /// Pristup bazi: otvara konekciju po pozivu (pool je u drajveru),
    /// sve je asinhrono i svaka MySQL greska prolazi kroz <see cref="Greske.Prevedi"/>.
    /// </summary>
    public sealed class Baza
    {
        private readonly string connectionString;

        public Baza(string connectionString)
        {
            // Sesijske varijable (@korisnik) citaju trigeri za istoriju zaliha
            this.connectionString = new MySqlConnectionStringBuilder(connectionString) { AllowUserVariables = true }.ConnectionString;
        }

        public async Task<T> IzvrsiAsync<T>(Func<MySqlConnection, Task<T>> rad, CancellationToken ct = default)
        {
            try
            {
                await using var konekcija = new MySqlConnection(connectionString);
                await konekcija.OpenAsync(ct);
                return await rad(konekcija);
            }
            catch (MySqlException e)
            {
                throw Greske.Prevedi(e);
            }
        }

        public Task IzvrsiAsync(Func<MySqlConnection, Task> rad, CancellationToken ct = default) =>
            IzvrsiAsync<object?>(async k => { await rad(k); return null; }, ct);

        public Task<List<T>> UpitAsync<T>(string sql, object? parametri = null, CancellationToken ct = default) =>
            IzvrsiAsync(async k => (await k.QueryAsync<T>(new CommandDefinition(sql, parametri, cancellationToken: ct))).AsList(), ct);

        public Task<T?> JedanAsync<T>(string sql, object? parametri = null, CancellationToken ct = default) =>
            IzvrsiAsync(k => k.QueryFirstOrDefaultAsync<T>(new CommandDefinition(sql, parametri, cancellationToken: ct)), ct);

        public Task<List<T>> ProceduraUpitAsync<T>(string procedura, object? parametri = null, CancellationToken ct = default) =>
            IzvrsiAsync(async k => (await k.QueryAsync<T>(
                new CommandDefinition(procedura, parametri, commandType: CommandType.StoredProcedure, cancellationToken: ct))).AsList(), ct);

        public Task ProceduraAsync(string procedura, object? parametri = null, CancellationToken ct = default) =>
            IzvrsiAsync(k => k.ExecuteAsync(
                new CommandDefinition(procedura, parametri, commandType: CommandType.StoredProcedure, cancellationToken: ct)), ct);

        /// <summary>Poziva proceduru sa jednim OUT parametrom tipa INT i vraca njegovu vrijednost.</summary>
        public Task<int> ProceduraSaIdAsync(string procedura, string outParametar, object parametri, CancellationToken ct = default) =>
            IzvrsiAsync(async k =>
            {
                var p = new DynamicParameters(parametri);
                p.Add(outParametar, dbType: DbType.Int32, direction: ParameterDirection.Output);
                await k.ExecuteAsync(new CommandDefinition(procedura, p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
                return p.Get<int>(outParametar);
            }, ct);

        public Task<bool> JeDostupnaAsync(CancellationToken ct = default) =>
            IzvrsiAsync(async k => await k.PingAsync(ct), ct);
    }
}
