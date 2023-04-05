namespace HowMuchLeft.ConsoleUI.Calculations;

public static class TimeCalculations
{
    public static TimeSpan CalculateTotalBreakTime(List<DateTime> breakTimes, ref Boolean isWorkingTime)
    {
        if (breakTimes is null || !breakTimes.Any()) { return TimeSpan.Zero; }

        var result = TimeSpan.Zero;

        for (int i = 0; i < breakTimes.Count - 1; i += 2)
        {
            result += breakTimes[i + 1] - breakTimes[i];
        }

        isWorkingTime = breakTimes.Count % 2 == 0;

        return result;
    }

    public static DateTime CalculateEndTime(TimeSpan workTime, DateTime startTime, Double breakTime, Double necessaryBreakAfterTime, TimeSpan totalBreakTime)
    {
        if (workTime < TimeSpan.FromHours(necessaryBreakAfterTime))
        {
            return startTime + workTime;
        }

        if (workTime > TimeSpan.FromHours(necessaryBreakAfterTime) &&
                totalBreakTime < TimeSpan.FromMinutes(breakTime))
        {
            return startTime + workTime + TimeSpan.FromMinutes(breakTime);
        }

        return startTime + workTime + totalBreakTime;
    }
}
