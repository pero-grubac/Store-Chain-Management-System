using OnlineKupovinaGUI;

namespace OnlineKupovinaGUI.Tests
{
    public sealed class DBUtilTests : IDisposable
    {
        private readonly string directory = Directory.CreateTempSubdirectory("prodavnica-tests-").FullName;

        private string Settings(string name, string json)
        {
            string path = Path.Combine(directory, name);
            File.WriteAllText(path, json);
            return path;
        }

        private static string WithConnection(string value) =>
            $$"""{ "ConnectionStrings": { "Prodavnica": "{{value}}" } }""";

        [Fact]
        public void EnvironmentVariable_WinsOverFiles()
        {
            string file = Settings("appsettings.json", WithConnection("Server=fajl"));

            Assert.Equal("Server=env", DBUtil.ResolveConnectionString("Server=env", file));
        }

        [Fact]
        public void LocalSettings_OverrideSharedSettings()
        {
            string local = Settings("appsettings.Local.json", WithConnection("Server=lokalno"));
            string shared = Settings("appsettings.json", WithConnection("Server=zajednicko"));

            Assert.Equal("Server=lokalno", DBUtil.ResolveConnectionString(null, local, shared));
        }

        [Fact]
        public void MissingLocalFile_FallsBackToSharedSettings()
        {
            string shared = Settings("appsettings.json", WithConnection("Server=zajednicko"));

            Assert.Equal("Server=zajednicko", DBUtil.ResolveConnectionString("  ", Path.Combine(directory, "nema.json"), shared));
        }

        [Fact]
        public void FileWithoutConnectionString_IsSkipped()
        {
            string empty = Settings("appsettings.Local.json", """{ "Logging": {} }""");
            string shared = Settings("appsettings.json", WithConnection("Server=zajednicko"));

            Assert.Equal("Server=zajednicko", DBUtil.ResolveConnectionString(null, empty, shared));
        }

        [Fact]
        public void NothingConfigured_ThrowsWithHint()
        {
            var error = Assert.Throws<InvalidOperationException>(() => DBUtil.ResolveConnectionString(null, Path.Combine(directory, "nema.json")));

            Assert.Contains(DBUtil.EnvironmentVariable, error.Message);
        }

        [Fact]
        public void ShippedSettings_PointOnlyToLocalDockerDatabase()
        {
            string shipped = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            string connection = DBUtil.ResolveConnectionString(null, shipped);

            Assert.Contains("Server=127.0.0.1", connection);
            Assert.Contains("Port=13306", connection);
        }

        public void Dispose() => Directory.Delete(directory, recursive: true);
    }
}
