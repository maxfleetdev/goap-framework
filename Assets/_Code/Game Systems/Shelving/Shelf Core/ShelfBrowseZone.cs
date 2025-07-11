using System.Collections.Generic;
using UnityEngine;
using GSS.AI;

/// <summary>
/// Class which dynamically assigns/de-assigns AIAgents to a list of browsing positions
/// </summary>
public class ShelfBrowseZone : MonoBehaviour
{
    // Holds the browsing transforms
    [SerializeField] private List<Transform> browsingSpots = new();

    // Holds a position (key) and an assigned agent (value)
    private Dictionary<Transform, AIAgent> browseMap = new();

    #region Initialisation

    public void Initialise()
    {
        browseMap.Clear();

        // load browse map
        foreach (Transform spot in browsingSpots)
        {
            browseMap.Add(spot, null);
        }
    }

    #endregion

    #region Browsing Logic

    /// <summary>
    /// Assigns a spot to a given AIAgent, returns if successful and their assigned browsing spot
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="targetPosition"></param>
    /// <returns>Boolean of availability, out Vector3 of position</returns>
    public bool TryAssignSpot(AIAgent agent, out Vector3 targetPosition)
    {
        foreach (var spot in browsingSpots)
        {
            if (browseMap[spot] == null)
            {
                browseMap[spot] = agent;
                targetPosition = spot.position;
                return true;
            }
        }

        targetPosition = Vector3.zero;
        return false;
    }

    /// <summary>
    /// Releases a spot with a given AIAgent
    /// </summary>
    /// <param name="agent"></param>
    public void ReleaseSpot(AIAgent agent)
    {
        if (agent == null)
        {
            return;
        }

        foreach (var kvp in browseMap)
        {
            if (kvp.Value == agent)
            {
                browseMap[kvp.Key] = null;
                break;
            }
        }
    }

    /// <summary>
    /// Returns if any spots are avaiable
    /// </summary>
    /// <returns>Boolean of availability</returns>
    public bool IsSpotAvailable()
    {
        foreach (var spot in browsingSpots)
        {
            if (browseMap[spot] == null)
                return true;
        }
        return false;
    }

    #endregion

    #region Gizmos
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Draw positions in Editor 
        if (browseMap.Count == 0 && browsingSpots.Count > 0)
        {
            foreach(var spot in browsingSpots)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(spot.position, 0.1f);
            }

            return;
        }

        // Draw availability in game
        foreach (var spot in browsingSpots)
        {
            Gizmos.color = browseMap[spot] == null ? Color.green : Color.red;
            Gizmos.DrawSphere(spot.position, 0.1f);
        }
    }
#endif
    #endregion
}