using System;
using System.Collections.Generic;
using UnityEngine;

namespace Europa.GameEvents
{
    /// <summary>
    /// Handles the timed events for things such as farming and hybrids.
    /// The morning event is called at the start of each day.
    /// <see cref="DailyEvents"/> to add more events.
    /// - Modified a lot by: Jayden (cozitime)
    /// </summary>
    public static class DailyEventHandler
    {
        /// <summary>
        /// All of the different growth events that can be triggered.
        /// Each index corresponds to a different individual option in the <see cref="DailyEvents"/> flag enum.
        /// </summary>
        private static Dictionary<int, DailyEventDelegate> dailyEvents;

        /// <summary>
        /// Static constructor for the ObservedDailyEventHandler class.
        /// </summary>
        static DailyEventHandler()
        {
            ClearDailyEvents();
            TimeHandler.CurrentHour.ValueChanged += (int hour) => InvokeDailyEvent(hour); // every hour invoke the daily event
        }


        /// <summary>
        /// Clears out all of the daily events.
        /// Is called before things are subscribed.
        /// </summary>
        public static void ClearDailyEvents()
        {
            dailyEvents = new Dictionary<int, DailyEventDelegate>();
        }

        #region Overloads for DailyEvents

        /// <summary>
        /// Invokes all of the subscribed actions for the specified daily event(s).
        /// <see cref="InvokeDailyEvent(int[])"/>
        /// </summary>
        /// <param name="times">What time events are being invoked? Is a flag enum so the actions can be invoked for multiple times at once.</param>
        public static void InvokeDailyEvent(DailyEvents times) => InvokeDailyEvent(times.GetHoursForDailyEvents());

        /// <summary>
        /// Subscribes the event to the specified daily event(s).
        /// <see cref="SubscribeDailyEvent(int[], DailyEventDelegate)"/>
        /// </summary>
        /// <param name="times">The times we wish to subscribe the event for.</param>
        /// <param name="growthEvent">The <see cref="DailyEventDelegate"/> action we wish to subscribe to the times.</param>
        public static DailyEventDelegate SubscribeDailyEvent(DailyEvents times, DailyEventDelegate growthEvent) => SubscribeDailyEvent(times.GetHoursForDailyEvents(), growthEvent);

        /// <summary>
        /// Removes the growth event from the specified daily event(s).
        /// <see cref="UnsubscribeDailyEvent(int[], DailyEventDelegate)"/>
        /// </summary>
        /// <param name="times">The times we wish to remove the event for.</param>
        /// <param name="growthEvent">The subscribed growth event we wish to remove.</param>
        public static void UnsubscribeDailyEvent(DailyEvents times, DailyEventDelegate growthEvent) => UnsubscribeDailyEvent(times.GetHoursForDailyEvents(), growthEvent);

        #endregion

        #region Overloads for int

        /// <summary>
        /// Invokes all of the subscribed actions for the specified daily event(s).
        /// <see cref="InvokeDailyEvent(int[])"/>
        /// </summary>
        /// <param name="time">The time we wish to invoke the event for.</param>
        private static void InvokeDailyEvent(int time) => InvokeDailyEvent(new[] { time });

        /// <summary>
        /// Subscribes the event to a single daily event.
        /// <see cref="SubscribeDailyEvent(int[], DailyEventDelegate)"/>
        /// </summary>
        /// <param name="time">The time we wish to subscribe the event for.</param>
        /// <param name="growthEvent">The <see cref="DailyEventDelegate"/> action we wish to subscribe to the time.</param>
        private static DailyEventDelegate SubscribeDailyEvent(int time, DailyEventDelegate growthEvent) => SubscribeDailyEvent(new[] { time }, growthEvent);

        /// <summary>
        /// Removes the growth event from a single daily event.
        /// <see cref="UnsubscribeDailyEvent(int[], DailyEventDelegate)"/>
        /// </summary>
        /// <param name="time">The time we wish to remove the event for.</param>
        /// <param name="growthEvent">The subscribed growth event we wish to remove.</param>
        private static void UnsubscribeDailyEvent(int time, DailyEventDelegate growthEvent) => UnsubscribeDailyEvent(new[] { time }, growthEvent);

        #endregion

        #region Overloads for int[]

        /// <summary>
        /// Invokes all of the subscribed actions for the specified daily event(s).
        /// </summary>
        /// <param name="times">Array of times (in hours) for which the events are being invoked.</param>
        public static void InvokeDailyEvent(params int[] times)
        {
            int length = times.Length;
            for (int i = 0; i < length; i++)
            {
                if (dailyEvents.TryGetValue(times[i], out var dailyEvent))
                {
                    dailyEvent.Invoke(true, 1);
                }
            }
        }

        /// <summary>
        /// Subscribes the event to the specified daily event(s).
        /// </summary>
        /// <param name="times">The times we wish to subscribe the event for.</param>
        /// <param name="growthEvent">The <see cref="DailyEventDelegate"/> action we wish to subscribe to the times.</param>
        public static DailyEventDelegate SubscribeDailyEvent(int[] times, DailyEventDelegate growthEvent)
        {
            int length = times.Length;
            for (int i = 0; i < length; i++)
            {
                if (!dailyEvents.ContainsKey(times[i]))
                {
                    dailyEvents[times[i]] = growthEvent;
                }
                else
                {
                    dailyEvents[times[i]] += growthEvent;
                }
            }

            return growthEvent;
        }

        /// <summary>
        /// Removes the growth event from the specified daily event(s).
        /// </summary>
        /// <param name="times">The times we wish to remove the event for.</param>
        /// <param name="growthEvent">The subscribed growth event we wish to remove.</param>
        public static void UnsubscribeDailyEvent(int[] times, DailyEventDelegate growthEvent)
        {
            int length = times.Length;
            for (int i = 0; i < length; i++)
            {
                if (dailyEvents.ContainsKey(times[i]))
                {
                    dailyEvents[times[i]] -= growthEvent;
                    if (dailyEvents[times[i]] == null)
                    {
                        dailyEvents.Remove(times[i]);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Removes every instance of this subscribed action from all of the daily events.
        /// </summary>
        /// <param name="growthEvent">The subscribed growth event we wish to remove.</param>
        public static void UnsubscribeDailyEvent(DailyEventDelegate growthEvent)
        {
            foreach (var key in new List<int>(dailyEvents.Keys))
            {
                dailyEvents[key] -= growthEvent;
                if (dailyEvents[key] == null)
                {
                    dailyEvents.Remove(key);
                }
            }
        }
    }
}
