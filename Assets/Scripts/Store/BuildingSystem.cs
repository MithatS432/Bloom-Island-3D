using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem Instance;

    public GameObject currentBuilding;
    private InventoryButton currentInvButton;
    private GameObject previewObject;

    private float lastSelectTime;

    public Color canPlaceColor = new Color(0, 1, 0, 0.5f);
    public Color cannotPlaceColor = new Color(1, 0, 0, 0.5f);

    public AudioSource audioSource;
    public AudioClip placeBuildingSound;


    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (currentBuilding == null) return;

        UpdatePreviewPosition();

        if (IsPointerOverUI()) return;

        if (Time.time - lastSelectTime < 0.2f) return;

        bool isClicked = Mouse.current?.leftButton.wasPressedThisFrame ?? false;
        bool isTouched = Touchscreen.current?.primaryTouch.press.wasPressedThisFrame ?? false;

        if (isClicked || isTouched)
        {
            TryPlaceBuilding();
        }
    }

    private void UpdatePreviewPosition()
    {
        if (previewObject == null) return;

        Vector2 screenPos = GetInputPosition();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            previewObject.transform.position = hit.point;

            bool isOnGround = hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground");

            int obstacleMask = LayerMask.GetMask("Building", "Environment");
            bool isOccupied = Physics.CheckSphere(hit.point, 1.0f, obstacleMask);

            bool canPlace = isOnGround && !isOccupied;

            UpdatePreviewColor(canPlace);
        }
    }

    private void TryPlaceBuilding()
    {
        Vector2 screenPos = GetInputPosition();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            int obstacleMask = LayerMask.GetMask("Building", "Environment");
            bool isOccupied = Physics.CheckSphere(hit.point, 1.0f, obstacleMask);

            if (!isOccupied)
            {
                PlaceBuilding(hit.point);
            }
            else
            {
                Debug.Log("<color=red>Burası dolu!</color>");
            }
        }
    }

    private void PlaceBuilding(Vector3 position)
    {
        GameObject newBuilding = Instantiate(currentBuilding, position, Quaternion.identity);
        newBuilding.layer = LayerMask.NameToLayer("Building");

        if (audioSource != null && placeBuildingSound != null)
        {
            audioSource.PlayOneShot(placeBuildingSound);
        }

        if (currentInvButton != null)
            currentInvButton.MarkAsUsed();

        ClearPreview();
    }

    public void SetCurrentBuilding(GameObject prefab, InventoryButton invBtn)
    {
        currentBuilding = prefab;
        currentInvButton = invBtn;
        lastSelectTime = Time.time;

        if (previewObject != null) Destroy(previewObject);

        previewObject = Instantiate(prefab);

        foreach (var col in previewObject.GetComponentsInChildren<Collider>()) col.enabled = false;

        SetLayerRecursive(previewObject, LayerMask.NameToLayer("Preview"));

        SetPreviewTransparency(previewObject);
    }

    private void UpdatePreviewColor(bool canPlace)
    {
        Color targetColor = canPlace ? canPlaceColor : cannotPlaceColor;
        foreach (Renderer r in previewObject.GetComponentsInChildren<Renderer>())
        {
            r.material.color = targetColor;
        }
    }

    private void SetPreviewTransparency(GameObject obj)
    {
        foreach (Renderer r in obj.GetComponentsInChildren<Renderer>())
        {
            Material mat = new Material(r.material);

            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = 3000;

            r.material = mat;
        }
    }

    private void SetLayerRecursive(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursive(child.gameObject, newLayer);
        }
    }

    private Vector2 GetInputPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            return Touchscreen.current.primaryTouch.position.ReadValue();
        return Mouse.current.position.ReadValue();
    }

    private void ClearPreview()
    {
        currentBuilding = null;
        currentInvButton = null;
        if (previewObject != null) Destroy(previewObject);
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        if (EventSystem.current.IsPointerOverGameObject()) return true;
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return true;
        return false;
    }
}