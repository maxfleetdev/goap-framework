using UnityEngine;

[DisallowMultipleComponent]
public class AIProfileProvider : MonoBehaviour
{
    [SerializeField] private AIProfileData profileData;

    public AIProfileData ProfileData
    {
        get => profileData;
        set => profileData = value;
    }
}