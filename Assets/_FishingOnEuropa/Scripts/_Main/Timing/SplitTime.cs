using static Europa.GameEvents.TimeConstants;
namespace Europa.GameEvents
{
    /// <summary>
    /// Contains the time in a readable format.
    /// </summary>
    public struct SplitTime
    {
        public int Day;
        public int Hour;
        public int Minute;

        /// <summary>
        /// Constructor for the SplitTime struct.
        /// </summary>
        /// <param name="day">Days</param>
        /// <param name="hour">Hours</param>
        /// <param name="minute">Minutes</param>
        public SplitTime(int day, int hour, int minute)
        {
            Day = day;
            Hour = hour;
            Minute = minute;
        }

        /// <summary>
        /// Generates a SplitTime from the total minutes that have passed.
        /// </summary>
        /// <param name="totalMinutesSinceLifetime">How many total minutes?</param>
        /// <returns>The clean and easy to read time data constructed from the total minutes.</returns>
        public static SplitTime FromTotalMinutes(ulong totalMinutesSinceLifetime)
        {
            const int minutesInDay = MINUTES_IN_HOUR * HOURS_IN_DAY;
            int day = (int)(totalMinutesSinceLifetime / minutesInDay);
            int hour = (int)((totalMinutesSinceLifetime % minutesInDay) / MINUTES_IN_HOUR);
            int minute = (int)(totalMinutesSinceLifetime % MINUTES_IN_HOUR);

            return new(day, hour, minute);
        }


        /// <summary>
        /// Overloads the + operator to add two SplitTime instances.
        /// </summary>
        /// <param name="a">The first SplitTime instance.</param>
        /// <param name="b">The second SplitTime instance.</param>
        /// <returns>The result of adding the two SplitTime instances.</returns>
        public static SplitTime operator +(SplitTime a, SplitTime b)
        {
            ulong totalMinutes = a.ToTotalMinutes() + b.ToTotalMinutes();
            return FromTotalMinutes(totalMinutes);
        }

        /// <summary>
        /// Overloads the - operator to subtract one SplitTime instance from another.
        /// </summary>
        /// <param name="a">The first SplitTime instance.</param>
        /// <param name="b">The second SplitTime instance.</param>
        /// <returns>The result of subtracting the second SplitTime instance from the first.</returns>
        public static SplitTime operator -(SplitTime a, SplitTime b)
        {
            ulong totalMinutesA = a.ToTotalMinutes();
            ulong totalMinutesB = b.ToTotalMinutes();
            return FromTotalMinutes(totalMinutesA > totalMinutesB ? totalMinutesA - totalMinutesB : 0);
        }

        /// <summary>
        /// Converts the SplitTime into total minutes.
        /// </summary>
        /// <param name="time">The time to convert.</param>
        /// <returns>The total minutes that have passed.</returns>
        public readonly ulong ToTotalMinutes()
        {
            return (ulong)(Day * MINUTES_IN_HOUR * HOURS_IN_DAY + Hour * MINUTES_IN_HOUR + Minute);
        }
    }
}

