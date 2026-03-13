using UnityEngine;

public class BuildButton : MonoBehaviour
{
    public GameObject buildingPrefab;

    public void Build()
    {
        BuildingSystem.Instance.StartBuilding(buildingPrefab);
    }
}