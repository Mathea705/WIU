using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public string    name;
        public AudioClip clip;
        [Range(0f, 1f)]   public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch  = 1f;
    }

    [SerializeField] private Sound[] sfx;
    [SerializeField] private Sound[] music;
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float musicVolume  = 1f;
    [SerializeField, Range(0f, 1f)] private float sfxVolume    = 1f;

    private AudioSource _sfxSource;
    private AudioSource _musicSource;
    private AudioSource _layerSource;

    private Dictionary<string, Sound> _sfxMap;
    private Dictionary<string, Sound> _musicMap;

    private string _currentMusic;
    private string _currentLayer;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _sfxSource               = gameObject.AddComponent<AudioSource>();
        _musicSource             = gameObject.AddComponent<AudioSource>();
        _musicSource.loop        = true;
        _musicSource.playOnAwake = false;
        _layerSource             = gameObject.AddComponent<AudioSource>();
        _layerSource.loop        = true;
        _layerSource.playOnAwake = false;

        _sfxMap   = BuildMap(sfx);
        _musicMap = BuildMap(music);
    }

    private static Dictionary<string, Sound> BuildMap(Sound[] arr)
    {
        var map = new Dictionary<string, Sound>();
        if (arr == null) return map;
        foreach (Sound s in arr)
            if (!string.IsNullOrEmpty(s.name) && s.clip != null)
                map[s.name] = s;
        return map;
    }

    // ── SFX ─────────────────────────────────────────────────────────────────

    /// Play a one-shot sound effect.
    public void Play(string soundName)
    {
        if (!_sfxMap.TryGetValue(soundName, out Sound s))
        {
            Debug.LogWarning($"AudioManager: SFX '{soundName}' not found.");
            return;
        }
        _sfxSource.pitch = s.pitch;
        _sfxSource.PlayOneShot(s.clip, s.volume * sfxVolume * masterVolume);
    }

    // ── Music ────────────────────────────────────────────────────────────────

    /// Start looping a music track. Does nothing if it's already playing.
    public void PlayMusic(string soundName)
    {
        if (_currentMusic == soundName && _musicSource.isPlaying) return;

        if (!_musicMap.TryGetValue(soundName, out Sound s))
        {
            Debug.LogWarning($"AudioManager: Music '{soundName}' not found.");
            return;
        }

        _currentMusic       = soundName;
        _musicSource.clip   = s.clip;
        _musicSource.volume = s.volume * musicVolume * masterVolume;
        _musicSource.pitch  = s.pitch;
        _musicSource.Play();
    }

    /// Stop the current music track.
    public void StopMusic()
    {
        _currentMusic = null;
        _musicSource.Stop();
    }

    // ── Layer (second music channel, plays on top of main music) ─────────────

    /// Play a looping track on the layer channel without touching the main music.
    public void PlayLayer(string soundName)
    {
        if (_currentLayer == soundName && _layerSource.isPlaying) return;

        if (!_musicMap.TryGetValue(soundName, out Sound s))
        {
            Debug.LogWarning($"AudioManager: Layer '{soundName}' not found.");
            return;
        }

        _currentLayer       = soundName;
        _layerSource.clip   = s.clip;
        _layerSource.volume = s.volume * musicVolume * masterVolume;
        _layerSource.pitch  = s.pitch;
        _layerSource.Play();
    }

    /// Stop the layer channel.
    public void StopLayer()
    {
        _currentLayer = null;
        _layerSource.Stop();
    }

    // ── Volume control ───────────────────────────────────────────────────────

    public void SetMasterVolume(float v)
    {
        masterVolume = Mathf.Clamp01(v);
        RefreshMusicVolume();
    }

    public void SetMusicVolume(float v)
    {
        musicVolume = Mathf.Clamp01(v);
        RefreshMusicVolume();
    }

    public void SetSfxVolume(float v) => sfxVolume = Mathf.Clamp01(v);

    private void RefreshMusicVolume()
    {
        if (_currentMusic != null && _musicMap.TryGetValue(_currentMusic, out Sound s))
            _musicSource.volume = s.volume * musicVolume * masterVolume;
    }
}
