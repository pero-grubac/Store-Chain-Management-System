using MySql.Data.MySqlClient;

namespace OnlineKupovinaGUI
{
    public class Password
    {
        private static string getLozinka = "SELECT Lozinka FROM {0} WHERE KorisnickoIme =  @UserName;";
        private static string updateLozinka = "UPDATE  {0} SET Lozinka=@lozinka WHERE KorisnickoIme =  @UserName;";

        private static string role;
        private static string korisnickoIme;
        public static void SaveData(string selectdRole, string ime)
        {

            role = selectdRole;
            korisnickoIme = ime;
        }

        public static string GetRole() { return role; }
        public static string GetKoriscnickoIme() { return korisnickoIme; }
        public static string GetHashedPassword()
        {
            if (role == null || korisnickoIme == null) return null;
            string hashedPasswordFromDB = "";

            using (var connection = DBUtil.GetConnection())
            {
                connection.Open();
                string commandText = string.Format(getLozinka, GetRole());
                using (MySqlCommand command = new(commandText, connection))
                {
                    command.Parameters.AddWithValue("@UserName", GetKoriscnickoIme());
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            object hashedPasswordValue = reader["Lozinka"];
                            if (hashedPasswordValue != DBNull.Value)
                            {
                                hashedPasswordFromDB = hashedPasswordValue.ToString();
                            }
                        }
                    }
                }

            }
            return hashedPasswordFromDB;
        }
        public static bool Verify(string password)
        {
            string hashedPassword = GetHashedPassword();
            if (hashedPassword == null) return false;
            bool result;
            try
            {
                result = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (Exception e)
            {
                return false;
            }
            return result;

        }
        public static bool UpdatePassword(string newPassword)
        {
            if (!Verify(newPassword))
            {
                using (var connection = DBUtil.GetConnection())
                {
                    connection.Open();
                    string commandText = string.Format(updateLozinka, GetRole());
                    using (MySqlCommand command = new(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", GetKoriscnickoIme());
                        command.Parameters.AddWithValue("@Lozinka", HashValue(newPassword));
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            return false;
        }

        public static string HashValue(string newPassword)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            return BCrypt.Net.BCrypt.HashPassword(newPassword, salt);
        }


    }
}
