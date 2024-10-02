using System.Collections.Generic;

namespace Europa.GameEvents
{
    /// <summary>
    /// Extension methods for all things <see cref="DailyEvents"/> related.
    /// </summary>
    public static class DailyEventsExtensions
    {
        /// <summary>
        /// Converts the specified <see cref="DailyEvents"/> flags into a list of corresponding hours.
        /// </summary>
        private static readonly Dictionary<DailyEvents, int> eventToHoursMap = new()
            {
                { DailyEvents.Midnight,  0}, // Midnight at 0
                { DailyEvents.Sunrise,  18},  // Sunrise at 18
                { DailyEvents.Noon, 37},    // Noon at 37
                { DailyEvents.Sunset, 56}   // Sunset at 56
            };

        /// <summary>
        /// Converts the specified <see cref="DailyEvents"/> flags into an array of corresponding hours.
        /// </summary>
        /// <param name="times">The <see cref="DailyEvents"/> flags to convert to hours.</param>
        /// <returns>An array of hours where each hour corresponds to a set bit in the <paramref name="times"/> flags.</returns>
        public static int[] GetHoursForDailyEvents(this DailyEvents times)
        {
            // Count the number of set bits in 'times'
            int count = 0;
            for (DailyEvents time = DailyEvents.Midnight; time <= DailyEvents.Sunset; time = (DailyEvents)((int)time << 1))
            {
                if ((times & time) != 0)
                {
                    count++;
                }
            }  

            // Create an array with the exact size needed
            int[] hours = new int[count];
            int index = 0;

            // Populate the array with the corresponding hours
            for (DailyEvents time = DailyEvents.Midnight; time <= DailyEvents.Sunset; time = (DailyEvents)((int)time << 1))
            {
                if ((times & time) != 0 && eventToHoursMap.TryGetValue(time, out var eventHour))
                {
                    hours[index++] = eventHour;
                }
            }

            return hours;
        }

    }



}