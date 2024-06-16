using System.Text;
using System.Text.RegularExpressions;

namespace CliFramework
{
    public class Repl
    {
        private readonly List<(
            Func<string[], bool> predicate, 
            Func<string[], bool> func
            )> commandAssociations;

        private readonly List<(
            string parameterDescription,
            string actionDescription
        )> commandDescriptions;
        public string invalidCommandMessage = "Invalid command.";
        public Func<string, string> preprocessArg = arg => arg.ToLower();
        public Action onQuit = () => {};
        public Action onClear = () => Console.Clear();
        public Action<string[], List<(string parameterDescription, string actionDescription)>> onHelp;
        public int pagifyHelp = 15;

        public Func<string, string[]> split = input =>
        {
            var matches = Regex.Matches(input, @"[\""].+?[\""]|'.+?'|\S+");
            return matches.Select(match => match.Value.Trim().Trim('\'', '"')).ToArray();
        };

        private void Initialize()
        {
            AddCommand(
                args => args.Length == 1 && (args[0].Equals("quit") || args[0].Equals("q") || args[0].Equals("exit")),
                args => {
                    onQuit();
                    return false;
                },
                "quit (q) (exit)",
                "Exit the program."
            );
            AddCommand(
                args => args.Length == 1 && (args[0].Equals("clear") || args[0].Equals("cls")),
                args => onClear(),
                "clear (cls)",
                "Clear the console screen."
            );
            AddCommand(
                args => args.Length >= 1 && (args[0].Equals("help") || args[0].Equals("h")),
                args => onHelp(args, commandDescriptions),
                "help (h)",
                "Display this message."
            );
        }

        public Repl()
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            commandAssociations = new List<(
                Func<string[], bool> predicate,
                Func<string[], bool> func)>();
            commandDescriptions = new List<(
                string parameterDescription,
                string actionDescription)>();
            onHelp = (args, commandDescriptions) => {
                var descriptions = commandDescriptions as IEnumerable<(string, string)>;
                if (args.Length > 1)
                {
                    var filter = string.Join(' ', args.Skip(1));
                    descriptions = descriptions.Where(pair => pair.Item1.Contains(filter));
                }
                if (descriptions.Count() > pagifyHelp) PrettyConsole.PrintPagedList(descriptions, pagifyHelp);
                else PrettyConsole.PrintList(descriptions);
            };
            Initialize();
        }


        public void AddCommand(Func<string[], bool> predicate, Action<string[]> action, string parameterDescription, string actionDescription)
        {
            AddCommand(predicate, (args) =>
            {
                action(args);
                return true;
            }, parameterDescription, actionDescription);
        }

        public void AddCommand(Func<string[], bool> predicate, Func<string[], bool> func, string parameterDescription, string actionDescription)
        {
            AddCommand(predicate, func);
            AddDescription(parameterDescription, actionDescription);
        }

        public void AddCommand(Func<string[], bool> predicate, Action<string[]> action)
        {
            AddCommand(predicate, (args) =>
            {
                action(args);
                return true;
            });
        }

        public void AddCommand(Func<string[], bool> predicate, Func<string[], bool> func)
        {
            commandAssociations.Add((predicate, func));
        }

        public void AddDescription(string parameterDescription, string actionDescriptionc)
        {
            commandDescriptions.Add((parameterDescription, actionDescriptionc));
        }

        private void PreprocessArgs(string[] args) 
        {
            for (var i = 0; i < args.Length; i++) args[i] = preprocessArg(args[i]);
        }

        public bool Process(string[] args)
        {
            PreprocessArgs(args);
            if (args.Length > 0)
            {
                foreach (var (predicate, func) in commandAssociations)
                {
                    if (predicate(args))
                        return func(args);
                }
                PrettyConsole.PrintError(invalidCommandMessage);
            }
            return true;
        }

        public void Run() => Run(null);

        public void Run(string[] args, bool doNotLoop = false)
        {
            if (args != null && args.Length > 0)
            {
                if (Process(args) || doNotLoop)
                {
                    onQuit();
                    return;
                }
            }
            do
            {
                Console.Write("> ");
                string input = Console.ReadLine().Trim();
                args = split(input);
            }
            while (Process(args));
        }
    }

}