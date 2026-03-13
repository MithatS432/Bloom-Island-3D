using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;

    public Button buyHouseButton;
    public Button buyLumberButton;
    public Button buyStoneMineButton;

    public GameObject housePrefab;
    public GameObject lumberPrefab;
    public GameObject stoneMinePrefab;

    public GameObject houseInventoryButton;
    public GameObject lumberInventoryButton;
    public GameObject mineInventoryButton;

    private int houseWoodCost = 300, houseStoneCost = 300;
    private int lumberWoodCost = 500, lumberStoneCost = 100;
    private int mineWoodCost = 150, mineStoneCost = 450;

    public AudioClip buySound;

    public void BuyHouse()
    {
        if (player.woodCount >= houseWoodCost && player.stoneCount >= houseStoneCost)
        {
            player.woodCount -= houseWoodCost;
            player.stoneCount -= houseStoneCost;
            player.RefreshResourceUI();

            InventoryManager.Instance.UnlockHouse();
            houseInventoryButton.SetActive(true);

            if (player.audioSource != null && buySound != null)
                player.audioSource.PlayOneShot(buySound);

            buyHouseButton.interactable = false;
        }
    }

    public void BuyLumber()
    {
        if (player.woodCount >= lumberWoodCost && player.stoneCount >= lumberStoneCost)
        {
            player.woodCount -= lumberWoodCost;
            player.stoneCount -= lumberStoneCost;
            player.RefreshResourceUI();

            InventoryManager.Instance.UnlockLumber();
            lumberInventoryButton.SetActive(true);

            if (player.audioSource != null && buySound != null)
                player.audioSource.PlayOneShot(buySound);

            buyLumberButton.interactable = false;
        }
    }

    public void BuyStoneMine()
    {
        if (player.woodCount >= mineWoodCost && player.stoneCount >= mineStoneCost)
        {
            player.woodCount -= mineWoodCost;
            player.stoneCount -= mineStoneCost;
            player.RefreshResourceUI();

            InventoryManager.Instance.UnlockMine();
            mineInventoryButton.SetActive(true);

            if (player.audioSource != null && buySound != null)
                player.audioSource.PlayOneShot(buySound);

            buyStoneMineButton.interactable = false;
        }
    }
}