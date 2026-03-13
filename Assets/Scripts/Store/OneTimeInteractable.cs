using UnityEngine;

public class OneTimeInteractable : MonoBehaviour
{
    private bool isUsed = false;
    private Collider col;
    private Renderer[] renderers;

    void Awake()
    {
        col = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void Setup()
    {
        isUsed = false;
        SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isUsed) return;

        if (other.CompareTag("Player"))
        {
            Use();
        }
    }

    void Use()
    {
        if (isUsed) return;

        isUsed = true;

        Debug.Log($"{gameObject.name} kullanıldı!");

        PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();
        if (player != null)
        {
        }

        SetActive(false);

        Destroy(gameObject, 0.5f);
    }

    void SetActive(bool active)
    {
        foreach (var r in renderers)
        {
            r.enabled = active;
        }

        if (col != null)
            col.enabled = active;
    }
}