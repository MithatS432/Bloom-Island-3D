using UnityEngine;
using UnityEngine.UI;

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

    void Start()
    {
        if (collectBarUI != null && collectBarUI.gameObject != null)
        {
            collectBarUI.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!isCollecting || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= collectRadius + distanceTolerance)
        {
            collectTimer = Mathf.Clamp(collectTimer + Time.deltaTime, 0, collectDuration);

            if (collectBarUI != null && collectBarUI.gameObject != null)
            {
                if (!collectBarUI.gameObject.activeSelf)
                    collectBarUI.gameObject.SetActive(true);

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
        else
        {
            isCollecting = false;
            player = null;

            if (collectBarUI != null && collectBarUI.gameObject != null)
            {
                collectBarUI.gameObject.SetActive(false);
            }
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
        if (isCollecting) return;
        player = playerMovement;
        isCollecting = true;

        if (collectBarUI != null && collectBarUI.gameObject != null)
        {
            collectBarUI.gameObject.SetActive(true);
            collectBarUI.fillAmount = collectTimer / collectDuration;
        }

        lastLoopSoundTime = Time.time - loopSoundInterval;
    }

    void FinishCollect()
    {
        isCollecting = false;

        if (collectBarUI != null && collectBarUI.gameObject != null)
            collectBarUI.gameObject.SetActive(false);

        if (collectEffectPrefab != null)
        {
            GameObject effect = Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }

        if (player != null && player.audioSource != null && collectSound != null)
            player.audioSource.PlayOneShot(collectSound);

        Destroy(gameObject);
    }
}