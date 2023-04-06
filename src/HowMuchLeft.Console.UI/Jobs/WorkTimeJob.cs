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
        TimeSpan workTime = TimeSpan.FromHours(_workTime);
        TimeSpan totalBreakTime = TimeCalculations.CalculateTotalBreakTime(_breakTimes, ref _isWorkingTime);
        DateTime endTime = TimeCalculations.CalculateEndTime(workTime, _startTime, _breakTime, _necessaryBreakAfterTime, totalBreakTime);

        if (totalBreakTime > TimeSpan.Zero)
        {
            Console.WriteLine($"Es wurden bereits Pausen von {totalBreakTime.TotalMinutes}min Länge gemacht. Status: {(_isWorkingTime ? PhaseKind.Work.GetDescription() : PhaseKind.Break.GetDescription())}");
        }

        ConsoleRenderer.DrawStartWork(_startTime, endTime);

        TimeSpan timeElapsed = DateTime.Now - _startTime;
        
        while (timeElapsed.TotalHours < 10)
        {
            if (_isWorkingTime)
            {
                TimeSpan remainingTime = endTime - DateTime.Now;
                ConsoleRenderer.DrawProgressbar(_workTime, remainingTime);
            }            

            WaitForKeyboardInput(ref totalBreakTime, ref workTime, ref endTime);

            DateTime lastStartBreakTime = _breakTimes.LastOrDefault();
            Double currentBreakTime = (DateTime.Now - lastStartBreakTime).TotalMinutes;

            if (!_isWorkingTime && lastStartBreakTime != default && currentBreakTime > _breakTime)
            {
                Console.Beep();
                Console.WriteLine($"Die Pause dauert bereits {currentBreakTime} Minuten an!");
            }

            Thread.Sleep(1000);
        }
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
