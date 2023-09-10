using System.Text;

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

        public Func<string, string> preprocessArg = arg => arg.ToLower();
        public Action onQuit = () => { };
        public int pagifyHelp = 10;

        private void Initialize()
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;
            AddCommand(
                args => args.Length == 1 && (args[0].Equals("quit") || args[0].Equals("q")),
                _ => {
                    onQuit();
                    return false;
                },
                "quit (q)",
                "Exit the program."
            );
            AddCommand(
                args => args.Length == 1 && (args[0].Equals("clear") || args[0].Equals("cls")),
                _ => Console.Clear(),
                "clear (cls)",
                "Clear the console screen."
            );
            AddCommand(
                args => args.Length == 1 && (args[0].Equals("help") || args[0].Equals("h")),
                _ =>
                {
                    if (commandDescriptions.Count > pagifyHelp) PrettyConsole.PrintPagedList(commandDescriptions, pagifyHelp);
                    else PrettyConsole.PrintList(commandDescriptions);
                },
                "help (h)",
                "Display this message."
            );
        }

        public Repl()
        {
            commandAssociations = new List<(
                Func<string[], bool> predicate,
                Func<string[], bool> func)>();
            commandDescriptions = new List<(
                string parameterDescription,
                string actionDescription)>();
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
            commandDescriptions.Add((parameterDescription, actionDescription));
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
                PrettyConsole.PrintError("Invalid command.");
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
            else
            {
                do
                {
                    Console.Write("> ");
                    string input = Console.ReadLine().Trim();
                    args = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                }
                while (Process(args));
            }
        }
    }

}