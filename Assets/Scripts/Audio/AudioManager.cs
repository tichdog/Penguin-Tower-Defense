using System;
using SaveSystem;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public static event Action<float, float> OnVolumesChanged;

    private const float MinVolume = 0.0001f;

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
    [Header("Source")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource UISource;
    [SerializeField] private AudioSource SFXSource;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip clickClip;

    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    [Serializable]
    private class AudioSaveData
    {
        public float musicVolume = 1f;
        public float sfxVolume = 1f;
    }

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        GameDataManager.OnLoaded += HandleSaveLoaded;
    }

    private void Start()
    {
        LoadVolumes();
    }

    private void OnDisable()
    {
        GameDataManager.OnLoaded -= HandleSaveLoaded;
    }

    #region Music

    public void SetMusicVolume(float value)
    {
        musicVolume = ClampVolume(value);
        ApplyMusicVolume();
        SaveVolumes();
        NotifyVolumesChanged();
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource.clip == clip)
            return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    #endregion

    #region SFX

    public void SetSFXVolume(float value)
    {
        sfxVolume = ClampVolume(value);
        ApplySFXVolume();
        SaveVolumes();
        NotifyVolumesChanged();
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        SFXSource.PlayOneShot(clip, volume);
    }
    #endregion

    private void LoadVolumes()
    {
        if (GameDataManager.Instance != null &&
            GameDataManager.Instance.TryGetData(SaveKeys.SettingsAudio, out AudioSaveData data))
        {
            musicVolume = ClampVolume(data.musicVolume);
            sfxVolume = ClampVolume(data.sfxVolume);
        }

        ApplyMusicVolume();
        ApplySFXVolume();
        NotifyVolumesChanged();
    }

    private void SaveVolumes()
    {
        if (GameDataManager.Instance == null)
            return;

        GameDataManager.Instance.SetData(SaveKeys.SettingsAudio, new AudioSaveData
        {
            musicVolume = musicVolume,
            sfxVolume = sfxVolume
        });

        GameDataManager.Instance.Save();
    }

    private void HandleSaveLoaded(int _)
    {
        LoadVolumes();
    }

    private void ApplyMusicVolume()
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20f);
    }

    private void ApplySFXVolume()
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20f);
    }

    private void NotifyVolumesChanged()
    {
        OnVolumesChanged?.Invoke(musicVolume, sfxVolume);
    }

    private static float ClampVolume(float value)
    {
        return Mathf.Clamp(value, MinVolume, 1f);
    }

    public void PlayClick()
    {
        UISource.PlayOneShot(clickClip);
    }

}
