using System;
using System.Collections.Generic;
using UnityEngine;

namespace Europa.GameEvents
{
    /// <summary>
    /// Handles the timed event's for things such as farming and hybrids. 
    /// The morning event is called at the start of each day.
    /// <see cref="DailyEvents"/> to add more events.
    /// - Modified a shit ton by: Jayden (cozitime)
    /// </summary>
    public static class GameEvents
    {
        // not set up yet.. Need to check with Skylar on this
        internal static void UpdateTimeTemp(Observable<int> currentHour, Observable<int> currentMinute, Observable<int> currentDay)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// All of the different growth events that can be triggered.
        /// Each index corresponds to a different individual option in the <see cref="DailyEvents"/> flag enum.
        /// </summary>
        private static List<DailyEventDelegate>[] dailyEvents;

        /// <summary>
        /// Constructor for the GameEvents static class.
        /// </summary>
        static GameEvents()
        {
            ClearDailyEvents();
        }

        /// <summary>
        /// Clears out all of the daily events.
        /// Is called before things are subscribed. 
        /// </summary>
        public static void ClearDailyEvents()
        {
            int length = GetDailyEventCount();
            dailyEvents = new List<DailyEventDelegate>[length];
            for (int i = 0; i < length; i++)
            {
                dailyEvents[i] = new List<DailyEventDelegate>();
            }
        }
        /// <summary>
        /// Invokes all of the subscribed actions for the specified daily event(s).
        /// </summary>
        /// <param name="times">What time events are being invoked? Is a flag enum so the actions can be invoked for multiple times at once.</param>
        /// <param name="wasObserved">Did the player observe the event? False if things happened while the player is in a different scene.</param>
        /// <param name="cycles">How many cycles is passing, 1 by default. Should always be 1 if the player observes the event.</param>
        public static void InvokeDailyEvent(DailyEvents times, bool wasObserved, int cycles = 1)
        {
            int timeEventIndexes = times.ToArrayIndexes().Count;
            for (int e = 0; e < timeEventIndexes; e++)
            {
                for (int i = 0; i < dailyEvents[e].Count; i++)
                {
                    dailyEvents[e][i](wasObserved, cycles);
                }
            }
        }

        /// <summary>
        /// Subscribes the event to the specified daily event(s).
        /// </summary>
        /// <param name="times">The times we wish to subscribe the event for.</param>
        /// <param name="growthEvent">The <see cref="DailyEventDelegate"/> action we wish to subscribed to the times.</param>
        public static DailyEventDelegate SubscribeDailyEvent(DailyEvents times, DailyEventDelegate growthEvent)
        {
            List<int> indexes = times.ToArrayIndexes();
            int length = indexes.Count;
            for (int i = 0; i < length; i++)
            {
                dailyEvents[i].Add(growthEvent);
            }

            return growthEvent;
        }

        /// <summary>
        /// Removes the growth event from the specified daily event(s).
        /// </summary>
        /// <param name="times">The times we wish to remove the event for.</param>
        /// <param name="growthEvent">The subscribed growth event we wish to remove.</param>
        public static void UnsubscribeDailyEvent(DailyEvents times, DailyEventDelegate growthEvent)
        {
            List<int> indexes = times.ToArrayIndexes();
            int length = indexes.Count;
            for (int i = 0; i < length; i++)
            {
                dailyEvents[i].Remove(growthEvent);
            }
        }

        /// <summary>
        /// Removes the every instance of this subscribed action from all of the daily events.
        /// </summary>
        /// <param name="growthEvent"></param>
        public static void UnsubscribeDailyEvent(DailyEventDelegate growthEvent)
        {
            int length = GetDailyEventCount();
            for (int i = 0; i < length; i++)
            {
                dailyEvents[i].Remove(growthEvent);
            }
        }

        /// <summary>
        /// Get's the total number of individual entries within the <see cref="DailyEvents"/> flag enum.
        /// </summary>
        /// <returns>How many individual entires are there within the <see cref="DailyEvents"/> flag enum.</returns>
        private static int GetDailyEventCount() => Enum.GetValues(typeof(DailyEvents)).Length;

        /// <summary>
        /// Converts the specified <see cref="DailyEvents"/> flags into a list of corresponding indexes.
        /// Each index represents a bit position that is set in the <paramref name="times"/> flags.
        /// </summary>
        /// <param name="times">The <see cref="DailyEvents"/> flags to convert to indexes.</param>
        /// <returns>A list of indexes where each index corresponds to a set bit in the <paramref name="times"/> flags.</returns>
        private static List<int> ToArrayIndexes(this DailyEvents times)
        {
            var indexes = new List<int>();  // List to store the resulting indexes
            int index = 0; // Current bit position being checked

            // Loop until all bits in the 'times' flags have been processed
            while (times != 0)
            {
                // Check if the current bit position is set in the 'times' flags
                if ((times & (DailyEvents)(1 << index)) != 0)
                {
                    indexes.Add(index); // Add the current bit position to the list of indexes
                    times &= ~(DailyEvents)(1 << index);  // Clear the bit that was just processed
                }
                index++; // Move to the next bit position
            }

            return indexes; // Return the list of indexes
        }
    }
}

