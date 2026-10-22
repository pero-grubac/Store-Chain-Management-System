using System.Text.Json;
namespace OnlineKupovinaGUI
{
    public class DBUtil
    {
        public const string EnvironmentVariable = "PRODAVNICA_CONNECTION_STRING";
        public const string ConnectionStringName = "Prodavnica";

        // Redoslijed: varijabla okruzenja, appsettings.Local.json (nije u gitu), appsettings.json
        private static readonly Lazy<string> Resolved = new(() => ResolveConnectionString(
            Environment.GetEnvironmentVariable(EnvironmentVariable),
            Path.Combine(AppContext.BaseDirectory, "appsettings.Local.json"),
            Path.Combine(AppContext.BaseDirectory, "appsettings.json")));

        public static string ConnectionString => Resolved.Value;

        public static string ResolveConnectionString(string? environmentValue, params string[] settingsFiles)
        {
            if (!string.IsNullOrWhiteSpace(environmentValue))
            {
                return environmentValue;
            }
            foreach (string file in settingsFiles)
            {
                string? fromFile = ReadFromSettings(file);
                if (!string.IsNullOrWhiteSpace(fromFile))
                {
                    return fromFile;
                }
            }
            throw new InvalidOperationException(
                $"Connection string nije podesen. Postavite {EnvironmentVariable} ili ConnectionStrings:{ConnectionStringName} u appsettings.json.");
        }

        private static string? ReadFromSettings(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            using JsonDocument document = JsonDocument.Parse(File.ReadAllText(path));
            if (document.RootElement.TryGetProperty("ConnectionStrings", out JsonElement connectionStrings) &&
                connectionStrings.TryGetProperty(ConnectionStringName, out JsonElement value))
            {
                return value.GetString();
            }
            return null;
        }
    }
}
