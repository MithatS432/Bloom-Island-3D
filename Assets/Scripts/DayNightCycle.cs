using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [Range(0, 24)]
    public float currentTime = 12f;
    public float dayLengthInMinutes = 2f;

    [Header("References")]
    public Light sun;
    public Material daySkybox;
    public Material nightSkybox;

    [Header("Light Settings")]
    public float maxSunIntensity = 1f;
    public float minSunIntensity = 0f;

    [Header("Skybox Transition")]
    [Range(0f, 1f)]
    public float skyboxBlend = 0f;

    void Update()
    {
        UpdateTime();
        UpdateSun();
        UpdateSkybox();
    }

    void UpdateTime()
    {
        float timeSpeed = 24f / (dayLengthInMinutes * 60f);
        currentTime += Time.deltaTime * timeSpeed;

        if (currentTime >= 24f)
            currentTime = 0f;
    }

    void UpdateSun()
    {
        float sunRotation = (currentTime / 24f) * 360f - 90f;
        sun.transform.rotation = Quaternion.Euler(sunRotation, 170f, 0f);

        float intensityMultiplier = Mathf.Clamp01(Mathf.Cos(currentTime / 24f * Mathf.PI * 2f) * 0.5f + 0.5f);
        sun.intensity = Mathf.Lerp(minSunIntensity, maxSunIntensity, intensityMultiplier);

        UpdateAmbient(intensityMultiplier);

        skyboxBlend = intensityMultiplier;
    }

    void UpdateAmbient(float intensityMultiplier)
    {
        Color dayColor = new Color(0.8f, 0.8f, 0.8f);
        Color nightColor = new Color(0.05f, 0.05f, 0.1f);

        RenderSettings.ambientLight = Color.Lerp(nightColor, dayColor, intensityMultiplier);
    }

    void UpdateSkybox()
    {
        if (daySkybox != null && nightSkybox != null)
        {
            RenderSettings.skybox.Lerp(nightSkybox, daySkybox, skyboxBlend);

            if (RenderSettings.skybox.HasProperty("_Exposure"))
                RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(0.2f, 1f, skyboxBlend));

            DynamicGI.UpdateEnvironment();
        }
    }
}