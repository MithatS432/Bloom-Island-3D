using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;

    public Button buyHouseButton;
    public Button buyLumberButton;
    public Button buyStoneMineButton;

    private int houseWoodCost = 300, houseStoneCost = 300;
    private int lumberWoodCost = 500, lumberStoneCost = 100;
    private int mineWoodCost = 150, mineStoneCost = 450;

    public AudioClip buySound;

    public void BuyHouse()
    {
        if (player.woodCount >= houseWoodCost && player.stoneCount >= houseStoneCost)
        {
            ProcessPurchase(houseWoodCost, houseStoneCost, buyHouseButton);
        }
    }

    public void BuyLumber()
    {
        if (player.woodCount >= lumberWoodCost && player.stoneCount >= lumberStoneCost)
        {
            ProcessPurchase(lumberWoodCost, lumberStoneCost, buyLumberButton);
        }
    }

    public void BuyStoneMine()
    {
        if (player.woodCount >= mineWoodCost && player.stoneCount >= mineStoneCost)
        {
            ProcessPurchase(mineWoodCost, mineStoneCost, buyStoneMineButton);
        }
    }

    private void ProcessPurchase(int woodCost, int stoneCost, Button clickedButton)
    {
        player.woodCount -= woodCost;
        player.stoneCount -= stoneCost;

        player.RefreshResourceUI();

        if (player.audioSource != null && buySound != null)
        {
            player.audioSource.PlayOneShot(buySound);
        }

        clickedButton.interactable = false;
    }
}