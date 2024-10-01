using UnityEngine;
using Europa.GameEvents;
using NaughtyAttributes;

/// <summary>
/// All of the growth settings for a growable object.
/// </summary>
[CreateAssetMenu(fileName = "Growth Data", menuName = "Europa/Growth Settings", order = 1)]
public class GrowthDataSO : ScriptableObject
{
    public enum EventTypeEnum
    {
        DailyEvent,
        DailyEventCustomHour,
        HourlyTimer,
        MinuteTimer,
    }

    public EventTypeEnum EventType = EventTypeEnum.DailyEvent;

    [Tooltip("When does this event occur during the day?"), ShowIf("EventType", "EventTypeEnum.DailyEvent")]
    public DailyEvents DailyEventEnum = DailyEvents.Sunrise;

    [Tooltip("What custom hours during the day does the event occur?"), ShowIf("EventType", "EventTypeEnum.DailyEventCustomHour")]
    public int[] DailyEventHour = new int[] { 6 };

    [Tooltip("How many hours does it take for the event to happen"), ShowIf("EventType", "EventTypeEnum.HourlyTimer")]
    public int HourlyTime = 1;

    [Tooltip("How many minutes does it take for the event to happen"), ShowIf("EventType", "EventTypeEnum.MinuteTimer")]
    public int MinuteTime = 10;
}

