using UnityEngine;

public static class PositionSnap
{
    /// <summary>
    /// Converts a deseried snapping point (0.25, 0.1 etc) to an int multiplier in SnapPosition
    /// </summary>
    /// <param name="raw_multiplier"></param>
    /// <returns>Snapping Amount</returns>
    public static int GetSnapAmount(float raw_multiplier)
    {
        return (int)(1 / raw_multiplier);
    }

    /// <summary>
    /// Snaps a given position to the nearest amount
    /// </summary>
    /// <param name="position"></param>
    /// <param name="snap_amount"></param>
    /// <param name="snap_y"></param>
    /// <returns></returns>
    public static Vector3 Snap(Vector3 position, int snap_amount, bool snap_y = false)
    {
        Vector3 snapped_position = position;

        // Snap x and z values (ignore y)
        snapped_position.x = Mathf.Round(position.x * snap_amount) / snap_amount;
        snapped_position.z = Mathf.Round(position.z * snap_amount) / snap_amount;
        
        if (snap_y)
        {
            snapped_position.y = Mathf.Round(position.z * snap_amount) / snap_amount;
        }

        return snapped_position;
    }
}