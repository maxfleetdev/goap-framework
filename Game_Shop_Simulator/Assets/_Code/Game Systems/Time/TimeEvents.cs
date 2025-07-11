using System;

/// <summary>
/// Events related to in-game time and store opening/closing
/// </summary>
public static class TimeEvents
{
    public static event Func<TimeManager> OnGetManager;
    
    /// <summary>
    /// Invoked when the store has opened
    /// </summary>
    public static event Action OnShiftStart;
    
    /// <summary>
    /// Invoked when the store has closed
    /// </summary>
    public static event Action OnShiftEnd;

    public static TimeManager GetManager()
    {
        if (OnGetManager != null)
        {
            return OnGetManager?.Invoke();
        }
        return null;
    }

    /// <summary>
    /// Call by TimeManager when store has opened
    /// </summary>
    public static void ShiftStarted()
    {
        OnShiftStart?.Invoke();
    }

    /// <summary>
    /// Call by TimeManager when store has closed
    /// </summary>
    public static void ShiftEnded()
    {
        OnShiftEnd?.Invoke();
    }
}
