namespace OnlineKupovina.Data
{
    public static class Lozinke
    {
        public const int WorkFactor = 11;

        public static string Hash(string lozinka) => BCrypt.Net.BCrypt.HashPassword(lozinka, WorkFactor);

        public static bool Provjeri(string? lozinka, string? hash)
        {
            if (string.IsNullOrEmpty(lozinka) || string.IsNullOrEmpty(hash)) return false;
            try
            {
                return BCrypt.Net.BCrypt.Verify(lozinka, hash);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                return false;
            }
        }
    }
}
