public static class Arg
{
    public static bool Parse(
        IEnumerable<string> args, 
        int mandatory, 
        out string[] mandatoryResults)
    {
        mandatoryResults = args.ToArray();
        return mandatoryResults.Length == mandatory;
    }

    public static bool Parse(
        IEnumerable<string> args, 
        int mandatory, 
        IEnumerable<string> flagNames, 
        IEnumerable<string> stringNames, 
        out string[] mandatoryResults, 
        out Dictionary<string, bool> flagResults, 
        out Dictionary<string, string> stringResults)
    {
        // Initialize the outputs
        mandatoryResults = null;
        flagResults = null;
        stringResults = null;

        // Find the length of unnamed args
        var unnamedLength = 0;
        foreach (var arg in args)
        {
            if (flagNames.Contains(arg) || stringNames.Contains(arg)) break;
            unnamedLength++;
        }
        if (unnamedLength != mandatory) return false;

        // Fill the mandatory
        string[] optionals;
        mandatoryResults = args.Take(mandatory).ToArray();
        optionals = args.Skip(mandatory).ToArray();
        
        // Parse the flags and strings
        flagResults = flagNames.ToDictionary(flag => flag, _ => false);
        stringResults = stringNames.ToDictionary(str => str, str => null as string);
        for (int i = 0; i < optionals.Length; i++)
        {
            if (flagNames.Contains(optionals[i])) flagResults[optionals[i]] = true;
            else if (stringNames.Contains(optionals[i])) 
            {
                if (i + 1 < optionals.Length) stringResults[optionals[i]] = optionals[++i];
                else return false;
            }
            else return false;
        }
        return true;
    }

    public static bool Parse(
        IEnumerable<string> args, 
        int mandatory, 
        bool spreadFirst,
        out string[] mandatoryResults, 
        out string[] spreadResults)
    {
        // Initialize the outputs
        mandatoryResults = null;
        spreadResults = null;

        // Find the length of unnamed args
        var unnamedLength = args.Count();
        if (unnamedLength < mandatory) return false;

        // Fill the spread & mandatory
        string[] optionals;
        int spreadLength = unnamedLength - mandatory;
        if (spreadFirst)
        {
            spreadResults = args.Take(spreadLength).ToArray();
            args = args.Skip(spreadLength);
            mandatoryResults = args.Take(mandatory).ToArray();
            optionals = args.Skip(mandatory).ToArray();
        }
        else
        {
            mandatoryResults = args.Take(mandatory).ToArray();
            args = args.Skip(mandatory);
            spreadResults = args.Take(spreadLength).ToArray();
            optionals = args.Skip(spreadLength).ToArray();
        }
        return true;
    }

    public static bool Parse(
        IEnumerable<string> args, 
        int mandatory, 
        bool spreadFirst,
        IEnumerable<string> flagNames, 
        IEnumerable<string> stringNames, 
        out string[] mandatoryResults, 
        out string[] spreadResults, 
        out Dictionary<string, bool> flagResults, 
        out Dictionary<string, string> stringResults)
    {
        // Initialize the outputs
        mandatoryResults = null;
        spreadResults = null;
        flagResults = null;
        stringResults = null;

        // Find the length of unnamed args
        var unnamedLength = 0;
        foreach (var arg in args)
        {
            if (flagNames.Contains(arg) || stringNames.Contains(arg)) break;
            unnamedLength++;
        }
        if (unnamedLength < mandatory) return false;

        // Fill the spread & mandatory
        string[] optionals;
        int spreadLength = unnamedLength - mandatory;
        if (spreadFirst)
        {
            spreadResults = args.Take(spreadLength).ToArray();
            args = args.Skip(spreadLength);
            mandatoryResults = args.Take(mandatory).ToArray();
            optionals = args.Skip(mandatory).ToArray();
        }
        else
        {
            mandatoryResults = args.Take(mandatory).ToArray();
            args = args.Skip(mandatory);
            spreadResults = args.Take(spreadLength).ToArray();
            optionals = args.Skip(spreadLength).ToArray();
        }
        
        // Parse the flags and strings
        flagResults = flagNames.ToDictionary(flag => flag, _ => false);
        stringResults = stringNames.ToDictionary(str => str, str => null as string);
        for (int i = 0; i < optionals.Length; i++)
        {
            if (flagNames.Contains(optionals[i])) flagResults[optionals[i]] = true;
            else if (stringNames.Contains(optionals[i])) 
            {
                if (i + 1 < optionals.Length) stringResults[optionals[i]] = optionals[++i];
                else return false;
            }
            else return false;
        }
        return true;
    }
}
