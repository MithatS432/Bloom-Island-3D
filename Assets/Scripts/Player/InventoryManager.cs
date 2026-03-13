using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public bool houseUnlocked = false;
    public bool lumberUnlocked = false;
    public bool mineUnlocked = false;

    void Awake()
    {
        Instance = this;
    }

    public void UnlockHouse() => houseUnlocked = true;
    public void UnlockLumber() => lumberUnlocked = true;
    public void UnlockMine() => mineUnlocked = true;
}