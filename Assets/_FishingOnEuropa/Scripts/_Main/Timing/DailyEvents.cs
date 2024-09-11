namespace Europa.GameEvents
{
    /// <summary>
    /// All of the possible daily time events that can be triggered during the day. Is enum flags.
    /// </summary>
    [System.Flags]
    public enum DailyEvents
    {
        Sunrise = 1 << 0,
        MiddayEclipse = 1 << 2,
        Sunset = 1 << 3,
        Midnight = 1 << 4,
    }
}