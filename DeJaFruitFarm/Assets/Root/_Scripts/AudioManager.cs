using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Background Music")]
    [SerializeField] private AudioClip _menuBackgroundMusic;
    [SerializeField] private AudioClip _gameBackgroundMusic;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip _buttonClickSound;
    [SerializeField] private AudioClip _buttonHoverSound;

    [Header("Actions Sounds")]
    [SerializeField] private AudioClip[] _actionSounds = new AudioClip[6];

    [Header("Music Volume")]
    [SerializeField, Range(0f, 1f)] private float _musicVolume = 0.3f;
    [SerializeField, Range(0f, 1f)] private float _sfxVolume = 0.7f;
    [SerializeField, Range(0f, 1f)] private float _uiVolume = 0.5f;

    private AudioSource _musicSource;
    private AudioSource _sfxSource;
    private AudioSource _uiSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }

        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;
        _musicSource.volume = _musicVolume;
        _musicSource.priority = 0;

        _uiSource = gameObject.AddComponent<AudioSource>();
        _uiSource.volume = _uiVolume;
        _uiSource.priority = 64;

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.volume = _sfxVolume;
        _sfxSource.priority = 128;
    }

    #region Background Music

    public void PlayMenuMusic()
    {
        PlayMusic(_menuBackgroundMusic, _musicVolume * 0.8f);
    }

    public void PlayGameMusic()
    {
        PlayMusic(_gameBackgroundMusic, _musicVolume);
    }

    private void PlayMusic(AudioClip clip, float volume = -1f)
    {
        if (clip == null)
        {
            return;
        }

        if (_musicSource.clip == clip && _musicSource.isPlaying)
        {
            return;
        }

        _musicSource.clip = clip;
        _musicSource.volume = volume >= 0 ? volume : _musicVolume;
        _musicSource.Play();
    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }

    #endregion

    #region UI Sounds
    public void PlayButtonClick()
    {
        PlayUISound(_buttonClickSound);
    }

    public void PlayButtonHover()
    {
        PlayUISound(_buttonHoverSound);
    }

    private void PlayUISound(AudioClip clip)
    {
        if (clip != null)
        {
            _uiSource.PlayOneShot(clip, _uiVolume);
        }
    }
    #endregion

    #region Action Sounds
    public void PlayActionSound(int actionIndex)
    {
        if (actionIndex >= 0 && actionIndex < _actionSounds.Length)
        {
            PlaySFX(_actionSounds[actionIndex]);
        }
        else
        {
            Debug.LogWarning($"Action sound index {actionIndex} out of range!");
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            _sfxSource.PlayOneShot(clip, _sfxVolume);
        }
    }
    #endregion

    #region Volume Control
    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
        _musicSource.volume = _musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
        _sfxSource.volume = _sfxVolume;
    }

    public void SetUIVolume(float volume)
    {
        _uiVolume = Mathf.Clamp01(volume);
        _uiSource.volume = _uiVolume;
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = Mathf.Clamp01(volume);
    }
    #endregion
}
