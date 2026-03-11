using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum CollectibleType { Wood, Stone }

public class Collectible : MonoBehaviour
{
    public CollectibleType type;
    public float collectDuration = 10f;
    public Image collectBarUI;

    public AudioClip collectSound;
    public AudioClip woodSound;
    public AudioClip stoneSound;

    public GameObject collectEffectPrefab;

    [HideInInspector] public bool isCollecting = false;

    private float collectTimer = 0f;
    private PlayerMovement player;
    private float lastLoopSoundTime = 0f;
    private float loopSoundInterval = 1f;
    [SerializeField] private float collectRadius = 3f;
    [SerializeField] private float distanceTolerance = 0.2f;

    [SerializeField] private float respawnTime = 20f;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private GameObject meshObject;

    private void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void Start()
    {
        if (collectBarUI != null)
        {
            collectBarUI.gameObject.SetActive(false);
            collectBarUI.fillAmount = 0f;
        }
    }

    void Update()
    {
        if (!isCollecting || player == null)
            return;

        if (!player.HasStamina())
        {
            StopCollecting();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > collectRadius + distanceTolerance)
        {
            StopCollecting();
            return;
        }

        collectTimer += Time.deltaTime;

        if (collectBarUI != null)
        {
            collectBarUI.fillAmount = Mathf.Clamp01(collectTimer / collectDuration);
        }

        if (Time.time - lastLoopSoundTime >= loopSoundInterval)
        {
            PlayLoopSound();
            lastLoopSoundTime = Time.time;
        }

        if (collectTimer >= collectDuration)
        {
            FinishCollect();
        }
    }

    void PlayLoopSound()
    {
        if (player != null && player.audioSource != null)
        {
            if (type == CollectibleType.Wood && woodSound != null)
                player.audioSource.PlayOneShot(woodSound);
            else if (type == CollectibleType.Stone && stoneSound != null)
                player.audioSource.PlayOneShot(stoneSound);
        }
    }

    public void StartCollect(PlayerMovement playerMovement)
    {
        if (isCollecting || !playerMovement.HasStamina())
            return;

        player = playerMovement;
        isCollecting = true;
        collectTimer = 0f;

        if (collectBarUI != null)
        {
            collectBarUI.gameObject.SetActive(true);
            collectBarUI.fillAmount = 0f;
        }

        lastLoopSoundTime = Time.time - loopSoundInterval;
    }

    void StopCollecting()
    {
        isCollecting = false;
        player = null;
        collectTimer = 0f;

        if (collectBarUI != null)
        {
            collectBarUI.gameObject.SetActive(false);
            collectBarUI.fillAmount = 0f;
        }
    }

    void FinishCollect()
    {
        isCollecting = false;

        if (collectBarUI != null)
        {
            collectBarUI.gameObject.SetActive(false);
        }

        if (collectEffectPrefab != null)
        {
            GameObject effect = Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }

        if (player != null && player.audioSource != null && collectSound != null)
            player.audioSource.PlayOneShot(collectSound);


        player = null;
        collectTimer = 0f;

        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (var r in renderers)
            r.enabled = false;

        foreach (var c in colliders)
            c.enabled = false;

        if (collectBarUI != null)
            collectBarUI.gameObject.SetActive(false);

        yield return new WaitForSeconds(respawnTime);

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        foreach (var r in renderers)
            r.enabled = true;

        foreach (var c in colliders)
            c.enabled = true;

        collectTimer = 0f;
        isCollecting = false;
        player = null;
    }
}