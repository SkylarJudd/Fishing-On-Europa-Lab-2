using System;
using System.Collections.Generic;

namespace Europa.GameEvents
{
    /// <summary>
    /// All of the possible daily time events that can be triggered during the day. Is enum flags.
    /// </summary>
    [System.Flags]
    public enum DailyEvents
    {
        Midnight = 1 << 0, // 0
        Sunrise = 1 << 1,  // 18
                           // Eclipse Start?
        Noon = 1 << 2,     // 37
                           // Eclipse End?
        Sunset = 1 << 3    // 56
    }
}