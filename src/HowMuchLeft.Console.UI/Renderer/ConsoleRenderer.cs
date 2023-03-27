using HowMuchLeft.ConsoleUI.Enums;
using HowMuchLeft.ConsoleUI.Extensions;

namespace HowMuchLeft.ConsoleUI.Renderer;
public static class ConsoleRenderer
{
    public static void DrawEndOfBreak(DateTime endTime)
    {
        Console.Write($"\nDie {PhaseKind.Work.GetDescription()} wurde {DateTime.Now:HH:mm}h wieder aufgenommen. Voraussichtlicher Feierabend um");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($" {endTime:HH:mm}h.\n");
        Console.ResetColor();
    }

    public static void DrawStartWork(DateTime startTime, DateTime endTime)
    {
        Console.Write($"Start der {PhaseKind.Work.GetDescription()}:");
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write($"{startTime:HH:mm}h");
        Console.ResetColor();
        Console.Write($". Voraussichtlicher Feierabend um");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($" {endTime:HH:mm}h");
        Console.ResetColor();
        Console.Write(".\n");
    }

    public static void DrawProgressbar(Double workTime, TimeSpan remainingTime)
    {
        const Int32 SECONDS_TO_HOURS = 3600;
        const Int32 PERCENT = 100;
        const Int32 MAX_OF_WORKTIME_IN_PERCENT = 125;

        Console.CursorVisible = false;
        Double progress = PERCENT - (remainingTime.TotalSeconds / (workTime * SECONDS_TO_HOURS)) * PERCENT;

        if (progress < 0) { progress = 0; }
        progress = Math.Min(MAX_OF_WORKTIME_IN_PERCENT, progress);

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
        Console.Write($" {progress:0.0}%\r");
        Console.ResetColor();
        Console.CursorLeft = 0;
        Console.CursorVisible = true;
    }
}
