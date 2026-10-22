using OnlineKupovina.Data;
using OnlineKupovinaGUI.Views;
using System.Globalization;

namespace OnlineKupovinaGUI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            var kultura = CultureInfo.GetCultureInfo("bs-Latn-BA");
            CultureInfo.DefaultThreadCurrentCulture = kultura;
            CultureInfo.DefaultThreadCurrentUICulture = kultura;
            Thread.CurrentThread.CurrentCulture = kultura;
            Thread.CurrentThread.CurrentUICulture = kultura;

            string connectionString;
            try
            {
                connectionString = DBUtil.ConnectionString;
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(e.Message, "Store Chain Management System", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool cijeliEkran = args.Contains("--fullscreen", StringComparer.OrdinalIgnoreCase);
            Application.Run(new GlavnaForma(new Repozitoriji(connectionString), cijeliEkran));
        }
    }
}
