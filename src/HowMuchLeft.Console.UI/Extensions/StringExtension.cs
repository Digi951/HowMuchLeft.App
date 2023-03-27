using System.Globalization;

namespace HowMuchLeft.ConsoleUI.Extensions;
public static class StringExtensions
{
    public static List<DateTime>? ToDateTimeList(this String input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        String[] timeStrings = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
        List<DateTime> timeList = new();

        foreach (String timeString in timeStrings)
        {
            if (DateTime.TryParseExact(timeString.Trim(), "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time))
            {
                timeList.Add(time);
            }
            else
            {
                throw new ArgumentException($"Invalid time format: {timeString}");
            }
        }

        return timeList;
    }
}
