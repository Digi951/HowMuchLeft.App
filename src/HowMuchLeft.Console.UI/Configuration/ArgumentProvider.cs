using CommandLine;

namespace HowMuchLeft.ConsoleUI.Configuration;
public class ArgumentProvider
{
    private readonly String[] _args;
    private readonly Parser _parser;
    private CommandLineOptions _options = new();

    public ArgumentProvider(String[] args)
    {
        _args = args;
        _parser = new Parser(config => config.HelpWriter = Console.Out);
        ReadArguments();
    }

    public String StartTime { get; set; }

    public List<String> BreakTimes { get; set; }

    private void ReadArguments()
    {
        var result = _parser.ParseArguments<CommandLineOptions>(_args);

        result.WithParsed(o =>
        {
            StartTime = o.StartTime;
            BreakTimes = String.IsNullOrEmpty(o.BreakTimes) ? new List<String>() : o.BreakTimes.Split(',').ToList();
            _options = o;
        });

        result.WithNotParsed(errors =>
        {
            Console.WriteLine("Fehler bei der Verarbeitung der Argumente:");
            Console.WriteLine($"{String.Join(Environment.NewLine, errors)}");
        });

        // If the --help option was used, wait for a key press and exit the program.
        if (_args.Length > 0 && _args.First() == "--help")
        {
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
