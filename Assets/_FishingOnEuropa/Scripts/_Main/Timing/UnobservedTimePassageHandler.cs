using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Europa.GameEvents
{
    /// <summary>
    /// Handles objects and events that are triggered over the passage of time.
    /// This system assists with time passage for objects that are not currently loaded in.
    /// </summary>
    public static class UnobservedTimePassageHandler
    {
        private static readonly Dictionary<string, ulong> zoneLastUnloadedTime = new();
        private static readonly HashSet<string> currentlyLoadedZones = new();

        static UnobservedTimePassageHandler()
        {
            // when scene is uloaded update the zoneLastUnloadedTime
            SceneManager.sceneUnloaded += (scene) =>
            {
                string zoneName = scene.name;
                if (currentlyLoadedZones.Contains(zoneName))
                {
                    UnloadZone(zoneName);
                }
            };
        }




        /// <summary>
        /// Loads a zone into the currently loaded zones.
        /// </summary>
        /// <param name="zoneName">The area name of the zone.</param>
        public static void LoadZone(string zoneName)
        {
            currentlyLoadedZones.Add(zoneName);
        }

        /// <summary>
        /// Unloads a zone.
        /// </summary>
        /// <param name="zoneName">The area name of the zone.</param>
        public static void UnloadZone(string zoneName)
        {
            currentlyLoadedZones.Remove(zoneName);

            ulong totalMinutes = TimeHandler.GetTotalMinutes();
            zoneLastUnloadedTime[zoneName] = totalMinutes;
        }

        /// <summary>
        /// Gets the number of hours that have passed since the zone was last unloaded.
        /// </summary>
        /// <param name="zoneName">The area name of the zone.</param>
        /// <returns>The number of hours that have passed.</returns>
        public static int GetHoursSinceZoneUnloaded(string zoneName)
        {
            ulong totalMinutes = TimeHandler.GetTotalMinutes();

            ulong minutesSinceUnloaded = totalMinutes - GetTimeZoneWasUnloaded(zoneName);
            ulong hoursSinceUnloaded = minutesSinceUnloaded / TimeConstants.MINUTES_IN_HOUR;
            return (int)hoursSinceUnloaded;
        }

        /// <summary>
        /// Gets the number of daily events that have passed in a particular zone since it was last unloaded.
        /// </summary>
        /// <param name="zoneName">The area name of the zone.</param>
        /// <param name="times">Array of event times in hours.</param>
        /// <returns>The number of daily events that have occurred.</returns>
        public static int GetDailyEventsSinceZoneUnloaded(string zoneName, int[] times)
        {
            ulong totalMinutes = TimeHandler.GetTotalMinutes();

            ulong minutesSinceUnloaded = totalMinutes - GetTimeZoneWasUnloaded(zoneName);
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

        /// <summary>
        /// Gets when the zone was last unloaded, 
        /// if the zone has never been unloaded it will return 0 
        /// (as if it was uloaded at the start of the game).
        /// </summary>
        /// <param name="zoneName">The name of the zone.</param>
        /// <returns>Minutes the zone has been unloaded for.</returns>
        private static ulong GetTimeZoneWasUnloaded(string zoneName)
        {
            if (zoneLastUnloadedTime.TryGetValue(zoneName, out ulong time))
            {
                return time;
            }

            return 0;
        }
    }
}

