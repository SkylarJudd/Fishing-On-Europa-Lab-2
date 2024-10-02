using System;
using System.Collections.Generic;

namespace Europa.GameEvents
{
    /// <summary>
    /// Dictionary to manage growth timer events for IGrowable objects.
    /// </summary>
    public static class IGrowableTimedEventDictionary
    {
        private static readonly Dictionary<GrowthObjectBase, Action<int>> SubscribedGrowthTimerEvents = new();

        /// <summary>
        /// Subscribes a growable object to growth timer events.
        /// </summary>
        /// <param name="growable">The growable object.</param>
        /// <param name="onMinuteEvent">The action to perform on each minute event.</param>
        public static void SubscribeGrowthTimerEvent(GrowthObjectBase growable, Action<int> onMinuteEvent)
        {
            SubscribedGrowthTimerEvents.Add(growable, onMinuteEvent);
            TimeHandler.CurrentMinute.ValueChanged += onMinuteEvent;
        }

        /// <summary>
        /// Unsubscribes the growth timer events associated with the growable.
        /// </summary>
        /// <param name="growable">The growable object to unsubscribe.</param>
        public static void UnsubscribeGrowthTimerEvents(GrowthObjectBase growable)
        {
            if (SubscribedGrowthTimerEvents.ContainsKey(growable))
            {
                TimeHandler.CurrentMinute.ValueChanged -= SubscribedGrowthTimerEvents[growable];
                SubscribedGrowthTimerEvents.Remove(growable);
            }
        }
    }
}

