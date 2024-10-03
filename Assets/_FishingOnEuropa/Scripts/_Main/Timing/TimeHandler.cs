using System;
using System.IO;
using System.Timers;
using UnityEngine;
using static Europa.GameEvents.TimeConstants;
namespace Europa.GameEvents
{

    /// <summary>
    /// Handles the timed events for things.  
    /// <para>Skylar: each in-game minute I think should be around 0.4 seconds. 75-hour day-night cycle for Europa. </para>
    /// </summary>
    public static class TimeHandler
    {

        private static readonly string TEMP_FOLDER_PATH =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "temp");
        private static readonly string TEMP_FILE_PATH =
            Path.Combine(TEMP_FOLDER_PATH, "time.txt");

        // Observables for the times
        public static Observable<SplitTime> CurrentTime { private set; get; } = new Observable<SplitTime>(new SplitTime(0, 0, 0));
        public static Observable<int> CurrentDay { get; private set; } = new Observable<int>(0);
        public static Observable<int> CurrentHour { get; private set; } = new Observable<int>(0);
        public static Observable<int> CurrentMinute { get; private set; } = new Observable<int>(0);

        /// <summary>
        /// Constructor for the TimeHandler static class.
        /// Starts the timer for the minute interval.
        /// </summary>
        static TimeHandler()
        {
            SaveManager.OnInitialised += () =>
            {
                // we are ready to go
                Start();
            };

           
        }

        private static void Start()
        {
            // Load the serialisedMinute and set up the observer values 
            totalMinutes = LoadSerialisedMinute();
            ProcessSerialisedMinute();

            // Start the timer for the minute intervals.
            minuteTimer = new Timer(MINUTES_IN_TICK);
            minuteTimer.Start();
            minuteTimer.Elapsed += OnMinuteElapsed; // Event that is called every in game minute.
        }

        // Timer system for the minute interval.
        private static Timer minuteTimer;

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

            // temp save
            SaveSerialisedMinute(totalMinutes);
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
            string time = File.ReadAllText(TEMP_FILE_PATH);
            return ulong.Parse(time);
        }
        /// <summary>
        /// Saves the serialised minutes.
        /// </summary>
        /// <param name="totalMinutes">The total minutes since starting the game to save.</param>
        /// <exception cref="NotImplementedException"></exception>
        private static void SaveSerialisedMinute(ulong totalMinutes)
        {
            // Save the serialised time to the save file. TODO: Implement this.
            File.WriteAllText(TEMP_FILE_PATH, totalMinutes.ToString());
        }
    }
}

