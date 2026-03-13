using UnityEngine;
using System.Collections;

public enum BuildingType { House, Wood, Stone }

public class BuildingResource : MonoBehaviour
{
    public BuildingType buildingType;

    private PlayerMovement player;

    void Start()
    {
        player = Object.FindFirstObjectByType<PlayerMovement>();
        StartCoroutine(ResourceProduction());
    }

    IEnumerator ResourceProduction()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            switch (buildingType)
            {
                case BuildingType.House:
                    int woodAmount = Random.Range(1, 6);
                    int stoneAmount = Random.Range(1, 6);
                    player.UpdateCollectibleUI(CollectibleType.Wood, woodAmount);
                    player.UpdateCollectibleUI(CollectibleType.Stone, stoneAmount);
                    break;

                case BuildingType.Wood:
                    player.UpdateCollectibleUI(CollectibleType.Wood, 20);
                    break;

                case BuildingType.Stone:
                    player.UpdateCollectibleUI(CollectibleType.Stone, 20);
                    break;
            }
        }
    }
}