using System.Globalization;

namespace OnlineKupovinaDB
{
    public static class Cijene
    {
        private static readonly CultureInfo Kultura = CultureInfo.GetCultureInfo("bs-Latn-BA");

        /// <summary>Isto kao ROUND(cijena * (100 - popust) / 100, 2) u bazi.</summary>
        public static decimal SaPopustom(decimal cijena, int popust)
        {
            if (popust is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(popust));
            return Math.Round(cijena * (100 - popust) / 100m, 2, MidpointRounding.AwayFromZero);
        }

        public static string KM(decimal iznos) => iznos.ToString("N2", Kultura) + " KM";
    }
}
