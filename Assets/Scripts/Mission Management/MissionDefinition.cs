using UnityEngine;

public enum MissionType
{
    Collect
}

[CreateAssetMenu(fileName = "Mission", menuName = "Game/Mission")]
public class MissionDefinition : ScriptableObject
{
    public string missionName;
    [TextArea] public string description;

    public MissionType missionType;

    public CollectibleType targetType;
    public int targetAmount;

    public float expReward;
}