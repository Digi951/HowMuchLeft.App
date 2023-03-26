using HowMuchLeft.ConsoleUI.Models;
using Microsoft.Extensions.Configuration;

namespace HowMuchLeft.ConsoleUI.Jobs;
public class WorkTimeJob
{
    private readonly IConfiguration _config;
    private readonly double _workTime;
    private readonly double _breakTime;
    private readonly double _necessaryBreakAfterTime;
    private DateTime _startTime;
    private bool _isWorkingTime;
    private List<DateTime> _pauseStartTimes;

    public WorkTimeJob(IConfiguration config)
    {
        _config = config;
        var settings = _config.GetSection("WorkTimeSets").Get<WorkTimeModel>();
        if (!Double.TryParse(settings?.WorkTime, out _workTime)) { throw new ArgumentException("Invalid value for WorkTime"); }
        if (!Double.TryParse(settings?.BreakTime, out _breakTime)) { throw new ArgumentException("Invalid value for BreakTime"); }
        if (!Double.TryParse(settings?.NecessaryBreakAfterTime, out _necessaryBreakAfterTime)) { throw new ArgumentException("Invalid value for NecessaryBreakAfterTime"); } 
    }

    public void Run()
    {
        _startTime = DateTime.Now;
        _pauseStartTimes = new List<DateTime>();
        _isWorkingTime = true;
        var workTime = TimeSpan.FromHours(_workTime);
        var totalBreakTime = TimeSpan.Zero;

        DateTime endTime = _startTime + workTime + totalBreakTime;

        Console.Write($"Start der Arbeit:");
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write($"{_startTime:HH:mm}h");
        Console.ResetColor();
        Console.Write($". Voraussichtlicher Feierabend um");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($" {endTime:HH:mm}h");
        Console.ResetColor();
        Console.Write(".\n");

        while (DateTime.Now < _startTime + workTime)
        {            
            CalculateWorkingDay(workTime, ref totalBreakTime, ref endTime);

            Thread.Sleep(1000);
        }
    }

    private void CalculateWorkingDay(TimeSpan workTime, ref TimeSpan totalBreakTime, ref DateTime endTime)
    {
        if (_isWorkingTime)
        {
            TimeSpan remainingTime = _startTime + workTime - DateTime.Now;
            DrawProgressbar(remainingTime);
        }
        
            ProcessKeyboardInput(ref totalBreakTime, ref workTime, ref endTime);
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
                    _isWorkingTime = false;
                    _pauseStartTimes.Add(DateTime.Now);
                    Console.WriteLine($"\nStart der Pause: {DateTime.Now:HH:mm}h.");
                }
                else
                {
                    EndPause(ref totalBreakTime, ref workTime, ref endTime);
                }
            }
        }
    }

    private void EndPause(ref TimeSpan totalBreakTime, ref TimeSpan workTime, ref DateTime endTime)
    {
        DateTime lastPauseStartTime = _pauseStartTimes.LastOrDefault();
        if (lastPauseStartTime != default)
        {
            var pauseDuration = DateTime.Now - lastPauseStartTime;
            totalBreakTime += pauseDuration;
            _isWorkingTime = true;

            endTime = CalculateEndTime(totalBreakTime, workTime);

            Console.Write($"\nDie Arbeit wurde {DateTime.Now:HH:mm}h wieder aufgenommen. Voraussichtlicher Feierabend um");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($" {endTime:HH:mm}h.\n");
            Console.ResetColor();
        }
    }

    private DateTime CalculateEndTime(TimeSpan totalBreakTime, TimeSpan workTime)
    {   
        return workTime > TimeSpan.FromHours(_necessaryBreakAfterTime)
            ? _startTime + workTime + totalBreakTime
            : _startTime + workTime + TimeSpan.FromMinutes(_breakTime);
    }

    private void DrawProgressbar(TimeSpan remainingTime)
    {
        Console.CursorVisible = false;
        Double progress = 100 - (remainingTime.TotalSeconds / (_workTime * 3600)) * 100;

        if (progress < 0) { progress = 0; }
        progress = Math.Min(125, progress);

        // Erstellen der Fortschrittsanzeige
        Int32 widthOfProgressbar = 40; // Breite des Fortschrittsbalkens
        Int32 completedWidth = (Int32)Math.Floor(progress / 100 * widthOfProgressbar);
        Int32 remainingWidth = widthOfProgressbar - completedWidth < 0 
                                ? 0
                                : widthOfProgressbar - completedWidth;

        Console.Write("[");        

        if (completedWidth > widthOfProgressbar)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{new String('=', widthOfProgressbar)}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{new String('=', completedWidth - widthOfProgressbar)}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{new String('=', completedWidth)}");
        }

        Console.ResetColor();
        Console.Write($"{new String('-', remainingWidth)}]");
        Console.ForegroundColor = progress >= 100.0 
            ? ConsoleColor.Green 
            : ConsoleColor.Red;
        Console.Write($" {progress:0.0}%\r"); 
        Console.ResetColor();
        Console.CursorLeft = 0;
        Console.CursorVisible= true;
    }
}
