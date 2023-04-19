using HowMuchLeft.ConsoleUI.Calculations;
using HowMuchLeft.ConsoleUI.Enums;
using HowMuchLeft.ConsoleUI.Extensions;

namespace HowMuchLeft.ConsoleUI.Renderer;
public static class ConsoleRenderer
{
    /// <summary>
    ///     Draws a sentence with the end of break time
    /// </summary>
    /// <param name="endTime"></param>
    public static void DrawEndOfBreak(DateTime endTime, Boolean calculatedEndTime = true)
    {
        String prefix = calculatedEndTime ? "Voraussichtlicher" : "Geplanter";
        Console.Write($"\nDie {PhaseKind.Work.GetDescription()} wurde {DateTime.Now:HH:mm}h wieder aufgenommen. {prefix} Feierabend um");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($" {endTime:HH:mm}h.\n");
        Console.ResetColor();
    }

    /// <summary>
    ///     Draws a sentence with the starting time
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <param name="calculatedEndTime"></param>
    public static void DrawStartWork(DateTime startTime, DateTime endTime, Boolean calculatedEndTime = true)
    {
        Console.Write($"Start der {PhaseKind.Work.GetDescription()}:");
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write($"{startTime:HH:mm}h");
        Console.ResetColor();

        String prefix = calculatedEndTime ? "Voraussichtlicher" : "Geplanter";
        Console.Write($". {prefix} Feierabend um");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($" {endTime:HH:mm}h");
        Console.ResetColor();
        Console.Write(".\n");
    }

    /// <summary>
    ///     Checks if there breaks have already been made and draws the total break time.
    /// </summary>
    /// <param name="totalBreakTime"></param>
    /// <param name="isWorkingTime"></param>
    public static void DrawBreakingTimeAfterStartup(TimeSpan totalBreakTime, Boolean isWorkingTime)
    {
        if (totalBreakTime > TimeSpan.Zero)
        {
            String workingPhase = isWorkingTime ? PhaseKind.Work.GetDescription() : PhaseKind.Break.GetDescription();

            Console.WriteLine($"Es wurden bereits Pausen von {totalBreakTime.TotalMinutes}min Länge gemacht. Status: {workingPhase}");
        }
    }

    /// <summary>
    ///  Draws a notification, that the break has already been exceeded
    /// </summary>
    /// <param name="breakTimeExeeded"></param>
    public static void DrawExceededBreakTime(Boolean breakTimeExeeded, Double breakTime)
    {
        if (!breakTimeExeeded)
        {
            Console.WriteLine($"Die Pause dauert bereits {breakTime} Minuten an!");
        }
    }

    /// <summary>
    ///     Draws a progressbar. It shows also the current percentage and the remaining hours or the overtime.
    /// </summary>
    /// <param name="workTime"></param>
    /// <param name="remainingTime"></param>
    public static void DrawProgressbar(Double workTime, TimeSpan remainingTime)
    {
        const Int32 SECONDS_HOURS_FACTOR = 3600;
        const Int32 PERCENT = 100;
        const Int32 MAX_OF_WORKTIME_IN_PERCENT = 125;

        Console.CursorVisible = false;
        Double progress = ProgressCalculations.CalculateProgress(workTime, remainingTime, SECONDS_HOURS_FACTOR, PERCENT, MAX_OF_WORKTIME_IN_PERCENT);

        const Int32 WIDTH_OF_PROGRESSBAR = 40;
        Int32 completedWidth = (Int32)Math.Floor(progress / PERCENT * WIDTH_OF_PROGRESSBAR);
        Int32 remainingWidth = WIDTH_OF_PROGRESSBAR - completedWidth < 0
                                ? 0
                                : WIDTH_OF_PROGRESSBAR - completedWidth;

        Console.Write("[");

        if (completedWidth > WIDTH_OF_PROGRESSBAR)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{new String('=', WIDTH_OF_PROGRESSBAR)}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{new String('=', completedWidth - WIDTH_OF_PROGRESSBAR)}");
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
        Console.Write($" {progress:0.0}%");
        Console.Write($" ({remainingTime:h\\:mm\\h})\r");

        Console.ResetColor();

        Console.CursorLeft = 0;
        Console.CursorVisible = true;
    }

    
}
