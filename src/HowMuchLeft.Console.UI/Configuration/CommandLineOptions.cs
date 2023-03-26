using CommandLine;

namespace HowMuchLeft.ConsoleUI.Configuration;
public class CommandLineOptions
{
    [Option(shortName: 's', longName: "start", Required = false, HelpText = "Defines the start time hh:mm.")]
    public String? StartTime { get; set; }

    [Option('b', "breaks", Required = false, HelpText = "Takes a comma separated list of start and end times of breaks")]
    public String? BreakTimes { get; set; }
}
