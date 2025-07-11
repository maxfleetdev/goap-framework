using GSS.AI;
using UnityEngine;

[CreateAssetMenu(menuName = "GSS/AI Profile")]
public class AIProfileData : ScriptableObject
{
    [SerializeField] private AIPersonalityType personalityType;
    [SerializeField] private int objectID;      // used for pooling

    public AIPersonalityType PersonalityType
    {
        get => personalityType;
        private set => personalityType = value;
    }
    public int ObjectID
    {
        get => objectID;
        private set => objectID = value;
    }

    /// <summary>
    /// Returns a created AIPersonality class with this objects PersonalityType
    /// </summary>
    /// <returns></returns>
    public AIPersonality GetPersonality()
    {
        switch (personalityType)
        {
            case AIPersonalityType.Relaxed:
                return AIPersonality.Relaxed;

            case AIPersonalityType.Normal:
                return AIPersonality.Normal;

            case AIPersonalityType.Impatient:
                return AIPersonality.Impatient;

            default: 
                return null;
        }
    }
}