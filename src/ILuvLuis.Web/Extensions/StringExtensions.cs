namespace ILuvLuis.Web.Extensions
{
    public static class StringExtensions
    {
        public static string NormalizeString(this string str) => str?.ToLowerInvariant()?.Trim() ?? null;

        public static int LevenshteinDistance(this string str1, string str2)
        {
            var cost = 0;
            var m = str1.Length;
            var n = str2.Length;
            var d = new int[m + 1, n + 1];

            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            for (var i = 0; i <= m; i++)
            {
                d[i, 0] = i;
            }

            for (var j = 0; j <= n; j++)
            {
                d[0, j] = j;
            }

            for (var i = 1; i <= m; i++)
            {
                for (var j = 1; j <= n; j++)
                {
                    cost = (str1[i - 1] == str2[j - 1]) ? 0 : 1;
                    d[i, j] = System.Math.Min(System.Math.Min(
                        //deletion
                        d[i - 1, j] + 1,
                        //Insertion
                        d[i, j - 1] + 1),
                        //Sustitution
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[m, n];
        }

        public static double LevenshteinDistanceScore(this string str1, string str2)
        {
            var percentage = 0.0d;
            var distance = str1.LevenshteinDistance(str2);

            if (str1.Length > str2.Length)
            {
                percentage = (distance / (double)str1.Length);
            }
            else
            {
                percentage = (distance / (double)str2.Length);
            }

            return percentage;
        }
    }
}
