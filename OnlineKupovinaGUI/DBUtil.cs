using MySql.Data.MySqlClient;
namespace OnlineKupovinaGUI
{
    public class DBUtil
    {
        // TO DO 
        // napravi config fajl i iz njega citaj podatke

        private static string ConnectioString = "Server=localhost; Port=3306; Database=prodavnica; User Id=pero;Password=mysq1pa55w0rd$p3r0";
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectioString);
        }
    }
}
