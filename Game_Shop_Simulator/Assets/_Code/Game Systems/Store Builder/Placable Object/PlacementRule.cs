using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all valid orientations for a given object and checks if a given normal is valid
/// </summary>
public class PlacementRule
{
    private readonly List<Vector3> placeOrientations = new();
    private readonly float angleTolerance;

    public PlacementRule(BuildOrientation orientation, float tolerance = 25f)
    {
        // Enum has floor chosen
        if ((orientation & BuildOrientation.Floor) != 0)
        {
            // Used for up angle only
            placeOrientations.Add(Vector3.up);
        }

        // Enum has ceiling chosen
        if ((orientation & BuildOrientation.Ceiling) != 0)
        {
            // Used for down angle only
            placeOrientations.Add(Vector3.down);        
        }

        // Enum has wall chosen
        if ((orientation & BuildOrientation.Wall) != 0)
        {
            // Define right and left rules (x axis)
            placeOrientations.Add(Vector3.right);       
            placeOrientations.Add(-Vector3.right);
            
            // Define forward and back rules (z axis)
            placeOrientations.Add(Vector3.forward);     
            placeOrientations.Add(-Vector3.forward);
        }

        // Calculate tolerance angle for checks (angle to radian)
        angleTolerance = Mathf.Cos(tolerance * Mathf.Deg2Rad);
    }

    /// <summary>
    /// Checks if given surface normal is valid against the PlaceableObjects' orientation rules
    /// </summary>
    /// <param name="normal"></param>
    /// <returns>Boolean, if normal is valid</returns>
    public bool IsPlacementValid(Vector3 normal)
    {
        foreach (Vector3 rule in placeOrientations)
        {
            float dot = Vector3.Dot(normal.normalized, rule.normalized);

            // Product must be 1 or greater than angle tolerance
            if (dot >= angleTolerance)
            {
                // Normal is close enough
                return true;
            }
        }
        // Not a valid normal
        return false;
    }
}

[System.Flags]
/// <summary>
/// Multiple choice enum for defining an objects valid orientation
/// </summary>
public enum BuildOrientation
{
    None = 0,
    Floor = 1 << 0,
    Ceiling = 1 << 1,
    Wall = 1 << 2
}