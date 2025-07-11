using System;
using UnityEngine;

/// <summary>
/// Current test class for store opening/closing
/// </summary>
public class TimeManager : MonoBehaviour, IManageable
{
    [SerializeField] private int openingHour;
    [SerializeField] private int closingHour;
    [SerializeField] private float dayDurationInMinutes;

    private bool isRunning = false;

    private TimeSpan currentTime;
    private TimeSpan previousTime;
    private TimeSpan openTime;
    private TimeSpan closeTime;
    
    private float timePassed = 0f;
    private float secondsPerDay;
    private float progress = 0f;

    private long durationTicks;

    public event Action OnTimeChanged;
    public TimeSpan CurrentTime
    {
        get => currentTime;
    }
    public bool IsRunning
    {
        get => isRunning;
    }

    #region Manager Lifespan

    public void Initialise()
    {
        openTime = TimeSpan.FromHours(openingHour);
        closeTime = TimeSpan.FromHours(closingHour);
        durationTicks = (closeTime - openTime).Ticks;
        secondsPerDay = dayDurationInMinutes * 60f;

        TimeEvents.OnGetManager += GetManager;
    }

    public void Terminate()
    {
        EndShift();

        TimeEvents.OnGetManager -= GetManager;
    }

    private TimeManager GetManager()
    {
        return this;
    }

    #endregion

    #region Shift Logic

    public void StartShift()
    {
        timePassed = 0f;
        currentTime = openTime;
        progress = 0f;
        isRunning = true;

        TimeEvents.ShiftStarted();
        Logger.Info("Shift Started!");
    }

    public void EndShift()
    {
        isRunning = false;
        currentTime = closeTime;

        TimeEvents.ShiftEnded();
        Logger.Info("Shift Ended!");
    }

    #endregion

    #region Time Runtime

    private void Update()
    {
        if (!isRunning)
        {
            return;
        }

        UpdateGameTime();
    }

    private void UpdateGameTime()
    {
        timePassed += Time.deltaTime;
        progress = Mathf.Clamp01(timePassed / secondsPerDay);
        long current_ticks = (long)(durationTicks * progress);
        currentTime = new TimeSpan(openTime.Ticks + current_ticks);

        // Smart notifier
        if (currentTime.Minutes != previousTime.Minutes)
        {
            OnTimeChanged?.Invoke();
            previousTime = currentTime;
        }

        // Time has complete
        if (progress >= 1f)
        {
            EndShift();
        }
    }

    #endregion
}