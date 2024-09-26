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
    }

    public EventTypeEnum EventType = EventTypeEnum.DailyEvent;

    [Tooltip("What times does the growth for this object occur?"), ShowIf(nameof(EventTypeEnum), EventTypeEnum.DailyEvent)]
    public DailyEvents TimeOfGrowth = DailyEvents.Sunrise;

    [Tooltip("Max amount of random in-game minute delay after an observed growth event occurs, this is to stagger things such as particle effects.")]
    public int MaxRandomDelay = 0;
}

