using System;
using System.Timers;
using static Europa.GameEvents.TimeConstants;
namespace Europa.GameEvents
{

    /// <summary>
    /// Handles the timed events for things.  
    /// <para>Skylar: each in-game minute I think should be around 0.4 seconds. 75-hour day-night cycle for Europa. </para>
    /// </summary>
    public static class TimeHandler
    {
        // Observables for the times
        public static Observable<SplitTime> CurrentTime { private set; get; }
        public static Observable<int> CurrentDay { get; private set; }
        public static Observable<int> CurrentHour { get; private set; }
        public static Observable<int> CurrentMinute { get; private set; }

        /// <summary>
        /// Constructor for the TimeHandler static class.
        /// Starts the timer for the minute interval.
        /// </summary>
        static TimeHandler()
        {
            // Load the serialisedMinute and set up the observer values 
            // serialisedMinute = LoadSerialisedMinute();
            ProcessSerialisedMinute();

            // Start the timer for the minute intervals.
            minuteTimer = new Timer(MINUTES_IN_TICK);
            minuteTimer.Start();
            minuteTimer.Elapsed += OnMinuteElapsed; // Event that is called every in game minute.
        }

        // Timer system for the minute interval.
        private static readonly Timer minuteTimer;

        // The current serialised time. Is the total minutes that have passed in game since the very start.
        // Hope it doesn't overflow, I think it would take about 23381681843.6 real life years.
        private static ulong totalMinutes;

        /// <summary>
        /// How many minutes have passed since the start of the game.
        /// </summary>
        /// <returns>Returns the total amount of minutes that have passed since the very start of the game.</returns>
        public static ulong GetTotalMinutes()
        {
            return totalMinutes;
        }

        /// <summary>
        /// Event that is called when an in game minute has passed.
        /// It does all of the time calculations and updates the serialised time.
        /// </summary>
        private static void OnMinuteElapsed(object _, ElapsedEventArgs e)
        {
            totalMinutes++;
            ProcessSerialisedMinute();
        }

        /// <summary>
        /// Sets all of the observable time values to the correct values based on the serialised time.
        /// </summary>
        private static void ProcessSerialisedMinute()
        {
            CurrentTime.Value = SplitTime.FromTotalMinutes(totalMinutes);
            UpdateTimeObservables(CurrentTime);
        }

        /// <summary>
        /// Updates the current time to the specified values. Is called every in game minute.
        /// </summary>
        /// <param name="day">Total days</param>
        /// <param name="hour">Total hours</param>
        /// <param name="minute">Total minutes</param>
        private static void UpdateTimeObservables(SplitTime time)
        {
            CurrentDay.Value = time.Day;
            CurrentHour.Value = time.Hour;
            CurrentMinute.Value = time.Minute;
        }

        /// <summary>
        /// Gets the serialised time from the save file.
        /// </summary>
        /// <returns>The total serialised minutes from the save file.</returns>
        private static ulong LoadSerialisedMinute()
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

