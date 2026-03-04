using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSliderController : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider musicSlider;
    public Slider effectsSlider;

    void Start()
    {
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        effectsSlider.onValueChanged.AddListener(SetEffectsVolume);
    }

    void SetMusicVolume(float value)
    {
        float dB = Mathf.Lerp(-80f, 0f, value);
        mixer.SetFloat("MusicVolume", dB);
    }

    void SetEffectsVolume(float value)
    {
        float dB = Mathf.Lerp(-80f, 0f, value);
        mixer.SetFloat("SFXVolume", dB);
    }
}