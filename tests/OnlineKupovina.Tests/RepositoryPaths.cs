namespace OnlineKupovina.Tests
{
    internal static class RepositoryPaths
    {
        public static string Root { get; } = FindRoot();

        public static string DatabaseScripts => Path.Combine(Root, "database");

        private static string FindRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null && !File.Exists(Path.Combine(directory.FullName, "OnlineKupovina.sln")))
            {
                directory = directory.Parent;
            }
            return directory?.FullName ?? throw new DirectoryNotFoundException("OnlineKupovina.sln nije pronadjen");
        }
    }
}
