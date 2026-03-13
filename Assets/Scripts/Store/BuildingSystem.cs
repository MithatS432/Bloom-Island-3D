using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem Instance;

    public GameObject currentBuilding;
    private Camera mainCamera;

    [Header("Placed Buildings")]
    public List<GameObject> placedBuildings = new List<GameObject>();

    void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (currentBuilding == null) return;

        bool inputReceived = false;
        Vector2 screenPos = Vector2.zero;

        // PC
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            screenPos = Mouse.current.position.ReadValue();
            inputReceived = true;
        }
        // Mobile
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            int touchId = (int)Touchscreen.current.primaryTouch.touchId.ReadValue();

            if (EventSystem.current.IsPointerOverGameObject(touchId)) return;

            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
            inputReceived = true;
        }

        if (!inputReceived) return;

        Ray ray = mainCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                GameObject newBuilding = Instantiate(currentBuilding, hit.point, Quaternion.identity);

                MakeOneTimeUse(newBuilding);

                placedBuildings.Add(newBuilding);

                currentBuilding = null;
            }
        }
    }

    void MakeOneTimeUse(GameObject building)
    {
        Collider col = building.GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        OneTimeInteractable interactable = building.GetComponent<OneTimeInteractable>();
        if (interactable == null)
        {
            interactable = building.AddComponent<OneTimeInteractable>();
        }

        interactable.Setup();
    }

    public void StartBuilding(GameObject prefab)
    {
        currentBuilding = prefab;
    }
}