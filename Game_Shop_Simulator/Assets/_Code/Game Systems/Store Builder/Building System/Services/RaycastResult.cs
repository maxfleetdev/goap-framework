using UnityEngine;

public struct RaycastResult
{
    public bool Hit;
    public Vector3 Point;
    public Vector3 Normal;
    public string HitTag;

    public int ObjectID;
    public Transform HitTransform;

    /// <summary>
    /// RaycastResult which takes multiple casted Raycast parameters
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="point"></param>
    /// <param name="normal"></param>
    /// <param name="hitTag"></param>
    public RaycastResult(bool hit, Vector3 point, Vector3 normal, string hitTag)
    {
        Hit = hit;
        Point = point;
        Normal = normal;
        HitTag = hitTag;

        ObjectID = -1;
        HitTransform = null;
    }

    /// <summary>
    /// RaycastResult which takes a casted ObjectID and it's Transform
    /// </summary>
    /// <param name="objectID"></param>
    /// <param name="hitTransform"></param>
    public RaycastResult(int objectID, Transform hitTransform)
    {
        Hit = true;
        Point = Vector3.zero;
        Normal = Vector3.zero;
        HitTag = hitTransform != null ? hitTransform.tag : "";

        ObjectID = objectID;
        HitTransform = hitTransform;
    }
}