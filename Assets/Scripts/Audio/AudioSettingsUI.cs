using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        // Потом переделать на получение из SAVE
        musicSlider.value = AudioManager.Instance.GetMusicVolume();
        sfxSlider.value = AudioManager.Instance.GetSFXVolume();

        musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
    }
}