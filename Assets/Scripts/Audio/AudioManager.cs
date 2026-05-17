using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
    [Header("Source")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource UISource;
    [SerializeField] private AudioSource SFXSource;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip clickClip;

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
        LoadVolumes();
    }

    #region Music

    public void SetMusicVolume(float value)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }

    public float GetMusicVolume()
    {
        return 1f;
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
        mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    }

    public float GetSFXVolume()
    {
        return 1f;
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        SFXSource.PlayOneShot(clip, volume);
    }
    #endregion

    private void LoadVolumes()
    {
        SetMusicVolume(GetMusicVolume());
        SetSFXVolume(GetSFXVolume());
    }

    public void PlayClick()
    {
        UISource.PlayOneShot(clickClip);
    }

}