using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace CliFramework
{
    public static class PrettyConsole
    {
        public static string OneLine(this string text) 
        {
            text = text.Trim();
            int i = text.IndexOf('\n');
            if (i != -1)
            {
                text = text.Substring(0, i) + "...";
                return text;
            }
            else return text;
        }

        public static string Bullet(this string text)
        {
            var lines = text.Split('\n');
            StringBuilder formatted = new();
            formatted.Append(" \u2022 " + lines[0].TrimEnd());
            string indentation = "   ";
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].TrimEnd();
                formatted.AppendLine();
                formatted.Append(indentation + line);
            }
            return formatted.ToString();
        }

        public static string FormatKeyValue(KeyValuePair<string, string> keyValue, char token = '\u2192') =>
            FormatKeyValue(keyValue.Key, keyValue.Value, token);

        public static string FormatKeyValue(string key, string value, char token = '\u2192') =>
            Indent(key + " " + token + " ", value);

        public static void PrintKeyValue(string key, string value, char token = '\u2192') =>
            Console.WriteLine(FormatKeyValue(key, value, token));

        private static string Indent(string key, string value)
        {
            var lines = value.Split('\n');
            StringBuilder formatted = new();
            formatted.Append(key + lines[0].TrimEnd());
            string indentation = new(' ', key.Length);
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].TrimEnd();
                formatted.AppendLine();
                formatted.Append(indentation + line);
            }
            return formatted.ToString();
        }

        public static IEnumerable<string> FormatPairs(IEnumerable<(string, string)> values, int atLeastBy = 5)
        {
            int max = 0;
            if (values.Any())
                max = values.Max(tuple => tuple.Item1.Length);
            return values.Select(tuple =>
            {
                var item1 = tuple.Item1.OneLine();
                var count = max - item1.Length + atLeastBy;
                return Indent(item1 + new string(' ', count), tuple.Item2);
            });
        }

        public static IEnumerable<string> FormatPairsOneLine(IEnumerable<(string, string)> values, int atLeastBy = 5)
        {
            int max = 0;
            if (values.Any())
                max = values.Max(tuple => tuple.Item1.Length);
            return values.Select(tuple =>
            {
                var item1 = tuple.Item1.OneLine();
                var count = max - item1.Length + atLeastBy;
                return item1 + new string(' ', count) +  tuple.Item2.OneLine();
            });
        }

        public static void PrintError(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + text);
            Console.ResetColor();
        }

        public static void PrintList(IEnumerable<string> strings)
        {
            foreach (string s in strings)
                Console.WriteLine(s);
        }

        public static void PrintList(IEnumerable<(string, string)> pairs)
        {
            var strings = FormatPairs(pairs).Select(line => line.Bullet());
            PrintList(strings);
        }

        public static void PrintList(IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            var strings = keyValues.Select(keyValues => FormatKeyValue(keyValues).Bullet());
            PrintList(strings);
        }


        public static void PrintPagedList(IEnumerable<string> strings, int resultsPerPage, string header = null)
        {
            int currentPage = 0, total = strings.Count();
            if (total > 0)
            {
                while (true)
                {
                    Console.Clear();
                    var currentPageResults = strings.Skip(currentPage * resultsPerPage).Take(resultsPerPage);
                    int count = 0;
                    if (!string.IsNullOrEmpty(header))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(header);
                        Console.ResetColor();
                    }
                    foreach (var searchResult in currentPageResults)
                    {
                        Console.WriteLine(searchResult);
                        count++;
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(currentPage * resultsPerPage + count + " / " + total + " result(s)." +
                            "\nUse the arrow keys to display the next " + resultsPerPage + " results or Enter to stop.");
                    Console.ResetColor();
                    while (true)
                    {
                        var keyInfo = Console.ReadKey();
                        if (keyInfo.Key == ConsoleKey.Enter)
                            return;
                        else if (keyInfo.Key == ConsoleKey.RightArrow)
                        {
                            if ((currentPage + 1) * resultsPerPage < total) currentPage++;
                            break;
                        }
                        else if (keyInfo.Key == ConsoleKey.LeftArrow)
                        {
                            if (currentPage > 0) currentPage--;
                            break;
                        }
                    }
                }
            }
        }

        public static void PrintPagedList(IEnumerable<(string, string)> pairs, int resultsPerPage, string header = null)
        {
            var strings = FormatPairs(pairs).Select(line => line.Bullet());
            PrintPagedList(strings, resultsPerPage, header);
        }

        public static void PrintPagedList(IEnumerable<KeyValuePair<string, string>> keyValues, int resultsPerPage, char token = '\u2192', string header = null)
        {
            var strings = keyValues.Select(keyValue => FormatKeyValue(keyValue, token).Bullet());
            PrintPagedList(strings, resultsPerPage, header);
        }

        public static void PrintPagedListOneLine(IEnumerable<(string, string)> pairs, int resultsPerPage, string header = null)
        {
            var strings = FormatPairsOneLine(pairs).Select(line => line.Bullet());
            PrintPagedList(strings, resultsPerPage, header);
        }

        public static void PrintPagedListOneLine(IEnumerable<KeyValuePair<string, string>> keyValues, int resultsPerPage, char token = '\u2192', string header = null)
        {
            var strings = keyValues.Select(keyValue => (keyValue.Key.OneLine() + " " + token + " " + keyValue.Value.OneLine()).Bullet());
            PrintPagedList(strings, resultsPerPage, header);
        }
    }
}
