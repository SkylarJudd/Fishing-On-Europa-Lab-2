using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Europa.GameEvents
{
    /// <summary>
    /// Handles objects and events that are triggered over the passage of time.
    /// This system assists with time passage for objects that are not currently loaded in.
    /// </summary>
    public static class UnobservedTimePassageCalculator
    {
       
        /// <summary>
        /// Gets the number of minted that have passed since the zone was last unloaded.
        /// </summary>
        /// <param name="timeLastUnloaded">timeLastUnloaded.</param>
        /// <returns>The number of hours that have passed.</returns>
        public static int GetMinutesSinceZoneUnloaded(ulong timeLastUnloaded)
        {
            ulong totalMinutes = TimeHandler.GetTotalMinutes();
            ulong minutesSinceUnloaded = totalMinutes - timeLastUnloaded;
            return (int)minutesSinceUnloaded;
        }

        /// <summary>
        /// Gets the number of hours that have passed since the zone was last unloaded.
        /// </summary>
        /// <param name="timeLastUnloaded">timeLastUnloaded.</param>
        /// <returns>The number of hours that have passed.</returns>
        public static int GetHoursSinceZoneUnloaded(ulong timeLastUnloaded)
        {
            return GetMinutesSinceZoneUnloaded(timeLastUnloaded) / TimeConstants.MINUTES_IN_HOUR;
        }

        /// <summary>
        /// Gets the number of daily events that have passed in a particular zone since it was last unloaded.
        /// </summary>
        /// <param name="timeLastUnloaded">When was this item last unloaded?</param>
        /// <param name="times">Array of event times in hours.</param>
        /// <returns>The number of daily events that have occurred.</returns>
        public static int GetDailyEventsSinceZoneUnloaded(ulong timeLastUnloaded, params int[] times)
        {
            ulong totalMinutes = TimeHandler.GetTotalMinutes();

            ulong minutesSinceUnloaded = totalMinutes - timeLastUnloaded;
            ulong hoursSinceUnloaded = minutesSinceUnloaded / TimeConstants.MINUTES_IN_HOUR;
            ulong daysSinceUnloaded = hoursSinceUnloaded / TimeConstants.HOURS_IN_DAY;
            ulong remainingHours = hoursSinceUnloaded % TimeConstants.HOURS_IN_DAY;

            int eventCount = 0;

            // Count events for full days
            eventCount += (int)daysSinceUnloaded * times.Length;

            // Count events for remaining hours
            foreach (int time in times)
            {
                if (time < (int)remainingHours)
                {
                    eventCount++;
                }
            }

            return eventCount;
        }
    }
}

