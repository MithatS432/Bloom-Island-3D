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
        mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }

    void SetEffectsVolume(float value)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    }
}