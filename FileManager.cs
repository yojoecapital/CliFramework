using Newtonsoft.Json;

namespace CliFramework
{
    public class FileManager
    {
        protected static string GetFilePath(string path)
        {
            if (!Path.IsPathRooted(path))
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            else return path;
        }

        protected static string GetDictionaryFilePath(string path)
        {
            path = GetFilePath(path);
            if (!File.Exists(path))
                File.WriteAllText(path, "{}");
            return path;
        }

        protected static Dictionary<string, string> GetDictionary(string filePath) =>
            GetDictionary<string>(filePath);

        protected static Dictionary<string, TValue> GetDictionary<TValue>(string filePath) =>
            GetDictionary<string, TValue>(filePath);

        protected static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(string filePath) =>
            GetObject<Dictionary<TKey, TValue>>(filePath);

        protected static T GetObject<T>(string filePath)
        {
            string json = File.ReadAllText(filePath);
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default;
            }
        }

        protected static void SetDictionary<TKey, TValue>(string filePath, Dictionary<TKey, TValue> dictionary, Formatting formatting = Formatting.None)
            => SetObject(filePath, dictionary);

        protected static void SetObject<T>(string filePath, T value, Formatting formatting = Formatting.None)
        {
            string json = JsonConvert.SerializeObject(value, formatting);
            File.WriteAllText(filePath, json);
        }

        private static int LongestCommonSubsequence(string s1, string s2)
        {
            int[,] dp = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    if (s1[i - 1] == s2[j - 1])
                    {
                        dp[i, j] = dp[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                    }
                }
            }

            return dp[s1.Length, s2.Length];
        }

        public static IEnumerable<KeyValuePair<string, T>> SortDictionaryByKeyText<T>(Dictionary<string, T> dictionary, string text) =>
            dictionary.OrderByDescending(item => LongestCommonSubsequence(item.Key, text));

        public static IEnumerable<KeyValuePair<string, string>> SortDictionaryByValueText(Dictionary<string, string> dictionary, string text) =>
            dictionary.OrderByDescending(item => LongestCommonSubsequence(item.Value, text));
    }
}
