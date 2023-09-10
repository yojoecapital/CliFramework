# CliFramework Library

- This is a library for creating interactive command-line applications (REPLs) in C#. 
- It provides a flexible framework for defining command associations, descriptions, and handling user input.

## Features

- Easily create custom REPL applications.
- Define command associations with predicates and actions.
- Add descriptions for each command to provide context to users.
- Customize preprocessing of command arguments.
- Default commands for "quit," "clear," and "help."

## Installation

To use the CliFramework project library in your solution, follow these steps:

1. Navigate to your solution directory: `cd <your_solution_path>`
2. Clone the repository: `git clone https://github.com/yojoecapital/CliFramework`
3. Navigate to the project directory: `cd CliFramework`
4. Add a reference to the project to your solution.
   - You can add the submodule to your own repository using `git submodule add https://github.com/yojoecapital/CliFramework CliFramework`
   - Commit the changes with `git commit -m "Add CliFramework as submodule`

## Usage

Here's a basic example of how to use the library:

```c#
using CliFramework;

static void Main(string[] args)
{
    Repl repl = new();

    repl.AddCommand(
        args => args.Length == 2 && args[0].Equals("hello"),
        args => Console.WriteLine("Hello " + args[1] + "!"),
        "hello [your-name]",
        "Prints out a friendly greeting using your name."
    );
    
    repl.Run();
}
```

Running the application:

```
> help
 • quit (q)              Exit the program.
 • clear (cls)           Clear the console screen.
 • help (h)              Display this message.
 • hello [your-name]     Prints out a friendly greeting using your name.
> hello World
Hello world!
```

## Contact

For any inquiries or feedback, contact me at [yousefsuleiman10@gmail.com](mailto:yousefsuleiman10@gmail.com).