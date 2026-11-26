using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameAudio : MonoBehaviour
{
    public enum Track { Menu, Battle, Boss, Victory, GameOver }

    [SerializeField] private AudioClip[] music;
    [SerializeField] private AudioClip[] effects = System.Array.Empty<AudioClip>();
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.35f;
    [SerializeField, Range(0f, 1f)] private float effectsVolume = 0.45f;

    private const string MusicPreference = "Audio.Music";
    private const string EffectsPreference = "Audio.Effects";
    private static GameAudio instance;
    private readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
    private readonly Dictionary<string, float> lastPlayed = new Dictionary<string, float>();
    private AudioSource musicSource;
    private AudioSource effectsSource;
    private Track currentTrack;

    private void Awake()
    {
        instance = this;
        musicSource = CreateSource();
        effectsSource = CreateSource();
        musicSource.volume = musicVolume;
        effectsSource.volume = effectsVolume;
        musicSource.mute = PlayerPrefs.GetInt(MusicPreference, 1) == 0;
        effectsSource.mute = PlayerPrefs.GetInt(EffectsPreference, 1) == 0;

        foreach (AudioClip clip in effects)
        {
            if (clip != null) clips[clip.name] = clip;
        }
    }

    private AudioSource CreateSource()
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        return source;
    }

    private void Update()
    {
        bool inCombat = currentTrack == Track.Battle || currentTrack == Track.Boss;
        float target = inCombat && Time.timeScale == 0f ? musicVolume * 0.4f : musicVolume;
        musicSource.volume = Mathf.MoveTowards(musicSource.volume, target, Time.unscaledDeltaTime);
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    public static void PlayMusic(Track track)
    {
        if (instance == null) return;
        int index = (int)track;
        if (instance.music == null || index >= instance.music.Length) return;

        AudioClip clip = instance.music[index];
        AudioSource source = instance.musicSource;
        if (source.clip == clip && source.isPlaying) return;

        instance.currentTrack = track;
        source.clip = clip;
        source.loop = track == Track.Menu || track == Track.Battle || track == Track.Boss;
        if (clip != null) source.Play();
    }

    public static void Play(string cue, float volume = 1f, float interval = 0f)
    {
        if (instance == null || instance.effectsSource.mute) return;
        if (!instance.clips.TryGetValue(cue, out AudioClip clip)) return;

        float now = Time.unscaledTime;
        if (interval > 0f)
        {
            if (instance.lastPlayed.TryGetValue(cue, out float last) && now - last < interval) return;
            instance.lastPlayed[cue] = now;
        }

        instance.effectsSource.PlayOneShot(clip, volume);
    }

    public void BindSettings(GameObject panel)
    {
        if (panel == null) return;

        foreach (Transform row in panel.GetComponentsInChildren<Transform>(true))
        {
            if (row.name == "Music") BindToggle(row, musicSource, MusicPreference);
            if (row.name == "SoundsRow") BindToggle(row, effectsSource, EffectsPreference);
        }
    }

    private static void BindToggle(Transform row, AudioSource source, string preference)
    {
        Toggle toggle = row.GetComponentInChildren<Toggle>(true);
        if (toggle == null) return;

        toggle.SetIsOnWithoutNotify(!source.mute);
        toggle.onValueChanged.AddListener(enabled =>
        {
            source.mute = !enabled;
            PlayerPrefs.SetInt(preference, enabled ? 1 : 0);
            PlayerPrefs.Save();
            Play("ui_click");
        });
    }

    public void BindButtonHover()
    {
        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            ButtonAudioHover hover = button.gameObject.AddComponent<ButtonAudioHover>();
            hover.Button = button;
        }
    }
}
