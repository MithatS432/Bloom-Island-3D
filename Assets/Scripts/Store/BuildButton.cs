using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class InventoryButton : MonoBehaviour
{
    public GameObject buildingPrefab;
    public GameObject inventoryUI;

    private Button myButton;
    private CanvasGroup canvasGroup;
    private bool used = false;

    void Awake()
    {
        myButton = GetComponent<Button>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SelectBuilding()
    {
        if (used || buildingPrefab == null) return;

        if (BuildingSystem.Instance != null)
        {
            BuildingSystem.Instance.SetCurrentBuilding(buildingPrefab, this);

            if (inventoryUI != null)
                inventoryUI.SetActive(false);
        }
    }

    public void MarkAsUsed()
    {
        used = true;

        if (myButton != null)
        {
            myButton.interactable = false;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.5f;
            canvasGroup.blocksRaycasts = false;
        }
    }
}