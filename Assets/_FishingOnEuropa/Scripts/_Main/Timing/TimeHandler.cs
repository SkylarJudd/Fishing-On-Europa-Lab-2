using System;
using System.Timers;

namespace Europa.GameEvents
{
    /// <summary>
    /// Handles the timed events for things.  
    /// <para>Skylar: each in-game minute I think should be around 0.4 seconds. 75-hour day-night cycle for Europa. </para>
    /// </summary>
    public static class TimeHandler
    {
        // const values for the time intervals
        private const double MINUTE_INTERVAL = 400;
        private const int MINUTES_IN_HOUR = 60;
        private const int HOURS_IN_DAY = 75;

        // Observables for the current time
        public static Observable<int> CurrentMinute { get; private set; } = new(0);
        public static Observable<int> CurrentHour { get; private set; } = new(0);
        public static Observable<int> CurrentDay { get; private set; } = new(0);

        // Timer system for the minute interval.
        private static readonly Timer minuteTimer;

        // The current serialised time. Is the total minutes that have passed in game since the very start.
        // Hope it doesn't overflow, I think it would take about 23381681843.6 real life years.
        private static UInt64 serialisedMinute;

        /// <summary>
        /// Constructor for the TimeHandler static class.
        /// Starts the timer for the minute interval.
        /// </summary>
        static TimeHandler()
        {
            // Load the serialisedMinute and set up the observer values 
            // serialisedMinute = LoadSerialisedMinute();
            SetTimeValuesToSerialisedMinutes();

            // Start the timer for the minute intervals.
            minuteTimer = new Timer(MINUTE_INTERVAL);
            minuteTimer.Start();
            minuteTimer.Elapsed += MinuteElapsed; // Event that is called every in game minute.
        }

        /// <summary>
        /// Event that is called when an in game minute has passed.
        /// It does all of the time calculations and updates the serialised time.
        /// </summary>
        private static void MinuteElapsed(object _, ElapsedEventArgs e)
        {
            serialisedMinute++;
            SetTimeValuesToSerialisedMinutes();
        }

        /// <summary>
        /// Sets all of the observable time values to the correct values based on the serialised time.
        /// </summary>
        private static void SetTimeValuesToSerialisedMinutes()
        {
            int minute = (int)(serialisedMinute % MINUTES_IN_HOUR);
            int hour = (int)((serialisedMinute / MINUTES_IN_HOUR) % HOURS_IN_DAY);
            int day = (int)(serialisedMinute / (MINUTES_IN_HOUR * HOURS_IN_DAY));

            UpdateTime(hour, minute, day);
        }

        /// <summary>
        /// Updates the current time to the specified values. Is called every in game minute.
        /// </summary>
        /// <param name="hour">Total hours</param>
        /// <param name="minute">Total minutes</param>
        /// <param name="day">Total days</param>
        private static void UpdateTime(int minute, int hour, int day)
        {
            CurrentHour.Value = hour;
            CurrentMinute.Value = minute;
            CurrentDay.Value = day;
        }

        /// <summary>
        /// Gets the serialised time from the save file.
        /// </summary>
        /// <returns>The total serialised minutes from the save file.</returns>
        private static UInt64 LoadSerialisedMinute()
        {
            // Load the serialised time from the save file. TODO: Implement this.
            throw new NotImplementedException();
        }
        /// <summary>
        /// Saves the serialised minutes.
        /// </summary>
        /// <param name="totalMinutes">The total minutes since starting the game to save.</param>
        /// <exception cref="NotImplementedException"></exception>
        private static void SaveSerialisedMinute(UInt64 totalMinutes)
        {
            // Save the serialised time to the save file. TODO: Implement this.
            throw new NotImplementedException();
        }
    }
}

