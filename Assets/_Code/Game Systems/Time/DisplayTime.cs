using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Just for displaying time currently - not a good/effecient solution
/// </summary>
public class DisplayTime : MonoBehaviour
{
    private TextMeshProUGUI textGui;
    private TimeManager timeManager;
    private TimeSpan currentTime;

    private void Awake()
    {
        textGui = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StartCoroutine(WaitForManager());
    }

    private void OnDisable()
    {
        if (timeManager != null)
        {
            timeManager.OnTimeChanged -= TimeChanged;
        }
    }

    private System.Collections.IEnumerator WaitForManager()
    {
        while ((timeManager = TimeEvents.GetManager()) == null)
        {
            yield return null;
        }

        timeManager.OnTimeChanged += TimeChanged;
        TimeChanged();
    }

    private void TimeChanged()
    {
        currentTime = timeManager.CurrentTime;
        if (textGui != null)
        {
            textGui.text = $"{currentTime.Hours:D2}:{currentTime.Minutes:D2}";
        }
    }
}