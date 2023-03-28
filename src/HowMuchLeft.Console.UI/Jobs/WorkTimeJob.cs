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
    private readonly double _workTime;
    private readonly double _breakTime;
    private readonly double _necessaryBreakAfterTime;
    private DateTime _startTime = DateTime.MinValue;
    private bool _isWorkingTime;
    private List<DateTime> _breakTimes;

    public WorkTimeJob(IConfiguration config, CommandLineOptions options )
    {
        _config = config;
        var settings = _config.GetSection("WorkTimeSets").Get<WorkTimeModel>();
        if (!Double.TryParse(settings?.WorkTime, out _workTime)) { throw new ArgumentException("Invalid value for WorkTime"); }
        if (!Double.TryParse(settings?.BreakTime, out _breakTime)) { throw new ArgumentException("Invalid value for BreakTime"); }
        if (!Double.TryParse(settings?.NecessaryBreakAfterTime, out _necessaryBreakAfterTime)) { throw new ArgumentException("Invalid value for NecessaryBreakAfterTime"); }

        if (options.StartTime != null && !DateTime.TryParseExact(options.StartTime, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _startTime)) { throw new ArgumentException("Invalid value for WorkTime"); }

        _breakTimes = options.BreakTimes.ToDateTimeList() ?? new List<DateTime>();
    }

    public void Run()
    {
        _startTime = _startTime != DateTime.MinValue ? _startTime : DateTime.Now;
        _isWorkingTime = true;
        var workTime = TimeSpan.FromHours(_workTime);
        var totalBreakTime = CalculateTotalBreakTime();
        DateTime endTime = CalculateEndTime(workTime, totalBreakTime);

        if (totalBreakTime > TimeSpan.Zero)
        {
            Console.WriteLine($"Es wurden bereits Pausen von {totalBreakTime.TotalMinutes}min Länge gemacht. Status: {(_isWorkingTime ? PhaseKind.Work.GetDescription() : PhaseKind.Break.GetDescription())}");
        }

        ConsoleRenderer.DrawStartWork(_startTime, endTime);

        TimeSpan timeElapsed = DateTime.Now - _startTime;

        // Runs if the workingday is short than 10 hours
        while (timeElapsed.TotalHours < 10)
        {
            if (_isWorkingTime)
            {
                TimeSpan remainingTime = endTime - DateTime.Now;
                ConsoleRenderer.DrawProgressbar(_workTime, remainingTime);
            }

            ProcessKeyboardInput(ref totalBreakTime, ref workTime, ref endTime);

            Thread.Sleep(1000);
        }
    }   

    private void ProcessKeyboardInput(ref TimeSpan totalBreakTime, ref TimeSpan workTime, ref DateTime endTime)
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
                    totalBreakTime = CalculateTotalBreakTime();
                    endTime = CalculateEndTime(workTime, totalBreakTime);
                    ConsoleRenderer.DrawEndOfBreak(endTime);
                }
            }
        }
    }    

    private TimeSpan CalculateTotalBreakTime()
    {
        if (_breakTimes is null || !_breakTimes.Any()) { return TimeSpan.Zero; }

        var result = TimeSpan.Zero;

        for (int i = 0; i < _breakTimes.Count - 1; i += 2)
        {
            result += _breakTimes[i + 1] - _breakTimes[i];
        }

        _isWorkingTime = _breakTimes.Count % 2 == 0;

        return result;
    }

    private DateTime CalculateEndTime(TimeSpan workTime, TimeSpan totalBreakTime)
    {      
        if (workTime < TimeSpan.FromHours(_necessaryBreakAfterTime))
        {
            return _startTime + workTime;
        }

        if (workTime > TimeSpan.FromHours(_necessaryBreakAfterTime) &&
                totalBreakTime < TimeSpan.FromMinutes(_breakTime))
        {
            return _startTime + workTime + TimeSpan.FromMinutes(_breakTime);
        }

        return _startTime + workTime + totalBreakTime; 
    } 
}
