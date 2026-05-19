using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private Coroutine bindRoutine;
    private bool isBound;

    private void OnEnable()
    {
        bindRoutine = StartCoroutine(BindWhenReady());
    }

    private void OnDisable()
    {
        if (bindRoutine != null)
        {
            StopCoroutine(bindRoutine);
            bindRoutine = null;
        }

        Unbind();
    }

    private IEnumerator BindWhenReady()
    {
        while (AudioManager.Instance == null)
            yield return null;

        Bind();
        bindRoutine = null;
    }

    private void Bind()
    {
        if (isBound)
            return;

        if (musicSlider == null || sfxSlider == null)
        {
            Debug.LogError("[AudioSettingsUI] Music or SFX slider is not assigned.");
            return;
        }

        AudioManager.OnVolumesChanged += RefreshSliders;

        musicSlider.onValueChanged.RemoveListener(AudioManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.RemoveListener(AudioManager.Instance.SetSFXVolume);
        musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);

        RefreshSliders(AudioManager.Instance.GetMusicVolume(), AudioManager.Instance.GetSFXVolume());
        isBound = true;
    }

    private void Unbind()
    {
        AudioManager.OnVolumesChanged -= RefreshSliders;

        if (!isBound || AudioManager.Instance == null)
        {
            isBound = false;
            return;
        }

        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(AudioManager.Instance.SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(AudioManager.Instance.SetSFXVolume);

        isBound = false;
    }

    private void RefreshSliders(float musicVolume, float sfxVolume)
    {
        if (musicSlider != null)
            musicSlider.SetValueWithoutNotify(musicVolume);

        if (sfxSlider != null)
            sfxSlider.SetValueWithoutNotify(sfxVolume);
    }
}
