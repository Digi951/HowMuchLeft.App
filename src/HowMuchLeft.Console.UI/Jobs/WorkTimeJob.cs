using HowMuchLeft.ConsoleUI.Calculations;
using HowMuchLeft.ConsoleUI.Configuration;
using HowMuchLeft.ConsoleUI.Enums;
using HowMuchLeft.ConsoleUI.Extensions;
using HowMuchLeft.ConsoleUI.Models;
using HowMuchLeft.ConsoleUI.Renderer;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace HowMuchLeft.ConsoleUI.Jobs;
public sealed class WorkTimeJob
{
    private readonly IConfiguration _config;
    private readonly Double _workTime;
    private readonly Double _breakTime;
    private readonly Double _necessaryBreakAfterTime;
    private DateTime _startTime = DateTime.MinValue;
    private Boolean _isWorkingTime;
    private List<DateTime> _breakTimes;

    public WorkTimeJob(IConfiguration config, CommandLineOptions options )
    {
        _config = config;
        var settings = _config.GetSection("WorkTimeSets").Get<WorkTimeModel>();

        _breakTime = GetValueOrDefault(settings?.BreakTime, $"Invalid value for {nameof(settings.BreakTime)}. Expected value in format '30.0'.");
        _necessaryBreakAfterTime = GetValueOrDefault(settings?.NecessaryBreakAfterTime, $"Invalid value for {nameof(settings.WorkTime)}. Expected value in format '8.0'.");
        _startTime = GetDateTimeValueOrDefault(options.StartTime, "HH:mm", $"Invalid value for {nameof(options.StartTime)}. Expected value in format '08:00'.");
        _workTime = GetValueOrDefault(options?.WorkTime, $"Invalid value for {nameof(options.WorkTime)}"); 
        _breakTimes = options?.BreakTimes?.ToDateTimeList() ?? new List<DateTime>();
    }       

    public void Run()
    {
        _startTime = _startTime != DateTime.MinValue ? _startTime : DateTime.Now;
        _isWorkingTime = true;
        TimeSpan workTime = TimeSpan.FromHours(_workTime);
        TimeSpan totalBreakTime = TimeCalculations.CalculateTotalBreakTime(_breakTimes, ref _isWorkingTime);
        DateTime endTime = TimeCalculations.CalculateEndTime(workTime, _startTime, _breakTime, _necessaryBreakAfterTime, totalBreakTime);

        if (totalBreakTime > TimeSpan.Zero)
        {
            Console.WriteLine($"Es wurden bereits Pausen von {totalBreakTime.TotalMinutes}min Länge gemacht. Status: {(_isWorkingTime ? PhaseKind.Work.GetDescription() : PhaseKind.Break.GetDescription())}");
        }

        ConsoleRenderer.DrawStartWork(_startTime, endTime);

        TimeSpan timeElapsed = DateTime.Now - _startTime;
        
        Boolean breakTimeExeeded = false;

        while (timeElapsed.TotalHours < 10)
        {
            if (_isWorkingTime)
            {
                TimeSpan remainingTime = endTime - DateTime.Now;
                ConsoleRenderer.DrawProgressbar(_workTime, remainingTime);
                breakTimeExeeded = false;
            }

            WaitForKeyboardInput(ref totalBreakTime, ref workTime, ref endTime);

            DateTime lastStartBreakTime = _breakTimes.LastOrDefault();
            Double currentBreakTime = (DateTime.Now - lastStartBreakTime).TotalMinutes;

            if (!_isWorkingTime && lastStartBreakTime != default && currentBreakTime > _breakTime)
            {
                Console.Beep();
                if (!breakTimeExeeded)
                {
                    Console.WriteLine($"Die Pause dauert bereits {_breakTime} Minuten an!");
                }
                breakTimeExeeded = true;
            }

            Thread.Sleep(1000);
        }
    }

    private static DateTime GetDateTimeValueOrDefault(String? value, String format, String errorMessage)
    {
        DateTime result = default;

        if (value != null && !DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
        {
            throw new ArgumentException(errorMessage);
        }

        return result;
    }

    private static Double GetValueOrDefault(String? value, String? errorMessage)
    {
        Double result = default;

        if (value != null && !Double.TryParse(value, out result))
        {
            throw new ArgumentException(errorMessage);
        }

        return result;
    }

    private void WaitForKeyboardInput(ref TimeSpan totalBreakTime, ref TimeSpan workTime, ref DateTime endTime)
    {
        if (Console.KeyAvailable)
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Spacebar)
            {
                if (_isWorkingTime)
                {
                    // start break
                    _isWorkingTime = false;
                    _breakTimes.Add(DateTime.Now);
                    Console.WriteLine($"\nStart der {PhaseKind.Break.GetDescription()}: {DateTime.Now:HH:mm}h.");
                }
                else
                {
                    // end break
                    _breakTimes.Add(DateTime.Now);
                    totalBreakTime = TimeCalculations.CalculateTotalBreakTime(_breakTimes, ref _isWorkingTime);
                    endTime = TimeCalculations.CalculateEndTime(workTime, _startTime, _breakTime, _necessaryBreakAfterTime, totalBreakTime);
                    ConsoleRenderer.DrawEndOfBreak(endTime);
                }
            }
        }
    }  
}
