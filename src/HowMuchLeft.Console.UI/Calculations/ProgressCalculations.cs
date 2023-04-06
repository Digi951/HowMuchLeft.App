namespace HowMuchLeft.ConsoleUI.Calculations;
public static class ProgressCalculations
{
    public static Double CalculateProgress(Double workTime, TimeSpan remainingTime, Int32 secondsHoursFactor, Int32 percent
        , Int32 maxValuePercent)
    {
        Double progress = percent - (remainingTime.TotalSeconds / (workTime * secondsHoursFactor)) * percent;

        if (progress < 0) { progress = 0; }
        progress = Math.Min(maxValuePercent, progress);
        return progress;
    }
}
