using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;
    [SerializeField] private PlayerMovement player;

    [Header("Mission Database")]
    public List<MissionDefinition> missionDatabase;

    private MissionRuntime activeMission;

    [Header("UI")]
    public TextMeshProUGUI missionText;
    public Image expBar;

    [Header("XP")]
    public float maxExp = 100;
    private float currentExp;

    [Header("Mission Complete Effects")]
    public AudioClip missionCompleteSound;
    public GameObject missionCompleteEffectPrefab;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        PickRandomMission();
        UpdateExpUI();
    }

    void PickRandomMission()
    {
        if (missionDatabase == null || missionDatabase.Count == 0)
        {
            Debug.LogWarning("Mission Database boş!");
            return;
        }

        var mission = missionDatabase[Random.Range(0, missionDatabase.Count)];
        activeMission = new MissionRuntime(mission);

        UpdateMissionUI();
    }

    void UpdateMissionUI()
    {
        if (missionText == null || activeMission == null) return;

        missionText.text =
            $"Collect {activeMission.definition.targetAmount} {activeMission.definition.targetType} " +
            $"({activeMission.progress}/{activeMission.definition.targetAmount})";
    }

    void UpdateExpUI()
    {
        if (expBar != null)
            expBar.fillAmount = currentExp / maxExp;
    }

    public void ReportCollect(CollectibleType type, int amount)
    {
        if (activeMission == null) return;

        if (type != activeMission.definition.targetType)
            return;

        activeMission.AddProgress(amount);

        UpdateMissionUI();

        if (activeMission.IsComplete())
            CompleteMission();
    }

    public void CompleteMission()
    {
        currentExp += activeMission.definition.expReward;

        if (currentExp >= maxExp)
        {
            currentExp -= maxExp;
            Collectible[] collectibles = Object.FindObjectsByType<Collectible>(FindObjectsSortMode.None);
            foreach (Collectible c in collectibles)
            {
                c.collectDuration = Mathf.Max(1f, c.collectDuration - 1f);
                c.collectAmount += 5;
            }
            PlayMissionCompleteEffects();
        }

        UpdateExpUI();
        PickRandomMission();
    }
    void PlayMissionCompleteEffects()
    {
        if (player == null) return;

        if (missionCompleteSound != null)
        {
            player.audioSource.PlayOneShot(missionCompleteSound);
        }

        if (missionCompleteEffectPrefab != null)
        {
            GameObject effect = Instantiate(
                missionCompleteEffectPrefab,
                player.transform.position,
                Quaternion.identity
            );

            Destroy(effect, 2f);
        }
    }
}