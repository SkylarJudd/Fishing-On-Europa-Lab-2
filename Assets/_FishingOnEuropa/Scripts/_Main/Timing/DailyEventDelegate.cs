namespace Europa.GameEvents
{
    /// <summary>
    /// The delegate for the growth event. Bool for was observed followed by the cycles.
    /// </summary>
    /// <param name="wasObserved">Was the player able to see this event happen?</param>
    /// <param name="cycles">If the player was unable to see the event happen, how many times did it happen without the player seeing?</param>
    public delegate void DailyEventDelegate(bool wasObserved, int cycles = 1);
}

