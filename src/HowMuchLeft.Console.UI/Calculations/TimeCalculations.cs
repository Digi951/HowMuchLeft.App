namespace HowMuchLeft.ConsoleUI.Calculations;

public static class TimeCalculations
{
    /// <summary>
    ///     Calculates the total break time during the entire day
    /// </summary>
    /// <param name="breakTimes"></param>
    /// <param name="isWorkingTime"></param>
    /// <returns></returns>
    public static TimeSpan CalculateTotalBreakTime(List<DateTime> breakTimes, ref Boolean isWorkingTime)
    {
        if (breakTimes is null || !breakTimes.Any()) { return TimeSpan.Zero; }

        var result = TimeSpan.Zero;

        for (Int32 i = 0; i < breakTimes.Count - 1; i += 2)
        {
            result += breakTimes[i + 1] - breakTimes[i];
        }

        isWorkingTime = breakTimes.Count % 2 == 0;

        return result;
    }

    /// <summary>
    ///     Calculates the end of work time
    /// </summary>
    /// <param name="workTime"></param>
    /// <param name="startTime"></param>
    /// <param name="breakTime"></param>
    /// <param name="necessaryBreakAfterTime"></param>
    /// <param name="totalBreakTime"></param>
    /// <returns></returns>
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

    /// <summary>
    ///     Calculates the total work time for a day
    /// </summary>
    /// <param name="hours"></param>
    /// <returns></returns>
    public static TimeSpan CalculateWorkTime(Double hours)
    {
        return TimeSpan.FromHours(hours);
    }

    /// <summary>
    ///     Calculates the total work time for a day
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static TimeSpan CalculateWorkTime(DateTime start, DateTime end)
    {
        return (TimeSpan)(end - start)!;
    }
}
