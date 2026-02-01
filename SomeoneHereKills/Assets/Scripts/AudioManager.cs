using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    public AudioMixer mixer;

    public string masterParam = "MasterVolume";
    public string musicParam = "MusicVolume";
    public string sfxParam = "SFXVolume";
    public string ambienceParam = "AmbienceVolume";

    [Header("Mixer Groups (drag from MainMixer)")]
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup ambienceGroup;

    [Header("Music")]
    public AudioSource musicA;
    public AudioSource musicB;
    [Range(0.1f, 5f)] public float musicFadeSeconds = 1.0f;

    [Header("Ambience")]
    public AudioSource ambienceSource;

    [Header("SFX Pool")]
    public int sfxPoolSize = 10;

    private readonly List<AudioSource> sfxPool = new List<AudioSource>();
    private int sfxIndex = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicA == null) musicA = CreateChildSource("MusicA", musicGroup, loop: true);
        if (musicB == null) musicB = CreateChildSource("MusicB", musicGroup, loop: true);

        if (ambienceSource == null) ambienceSource = CreateChildSource("Ambience", ambienceGroup, loop: true);

        if (sfxPool.Count == 0)
        {
            for (int i = 0; i < sfxPoolSize; i++)
                sfxPool.Add(CreateChildSource($"SFX_{i}", sfxGroup, loop: false));
        }
    }

    AudioSource CreateChildSource(string name, AudioMixerGroup group, bool loop)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);

        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = loop;
        src.outputAudioMixerGroup = group;
        src.spatialBlend = 0f;
        return src;
    }

    public void SetMaster(float v01) => SetMixerParam(masterParam, v01);
    public void SetMusic(float v01) => SetMixerParam(musicParam, v01);
    public void SetSFX(float v01) => SetMixerParam(sfxParam, v01);
    public void SetAmbience(float v01) => SetMixerParam(ambienceParam, v01);

    void SetMixerParam(string param, float v01)
    {
        if (mixer == null || string.IsNullOrEmpty(param)) return;
        v01 = Mathf.Clamp(v01, 0.0001f, 1f);
        mixer.SetFloat(param, Mathf.Log10(v01) * 20f);
    }

    public void PlaySFXOneShot(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        var src = sfxPool[sfxIndex];
        sfxIndex = (sfxIndex + 1) % sfxPool.Count;

        src.volume = Mathf.Clamp01(volume);
        src.pitch = 1f;
        src.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip, float volume = 1f, bool fade = true)
    {
        if (clip == null) return;

        AudioSource from = (musicA.isPlaying) ? musicA : musicB;
        AudioSource to = (from == musicA) ? musicB : musicA;

        to.clip = clip;
        to.loop = true;
        to.volume = 0f;
        to.Play();

        if (fade) StartCoroutine(CrossFade(from, to, Mathf.Clamp01(volume), musicFadeSeconds));
        else
        {
            if (from.isPlaying) from.Stop();
            to.volume = Mathf.Clamp01(volume);
        }
    }

    public void PlayAmbience(AudioClip clip, float volume = 1f)
    {
        if (ambienceSource == null || clip == null) return;
        ambienceSource.clip = clip;
        ambienceSource.loop = true;
        ambienceSource.volume = Mathf.Clamp01(volume);
        ambienceSource.Play();
    }

    public void StopAmbience()
    {
        if (ambienceSource != null) ambienceSource.Stop();
    }

    IEnumerator CrossFade(AudioSource from, AudioSource to, float targetToVol, float seconds)
    {
        float t = 0f;
        float fromStart = from != null ? from.volume : 0f;

        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            float a = seconds <= 0f ? 1f : Mathf.Clamp01(t / seconds);

            if (from != null) from.volume = Mathf.Lerp(fromStart, 0f, a);
            if (to != null) to.volume = Mathf.Lerp(0f, targetToVol, a);

            yield return null;
        }

        if (from != null) { from.Stop(); from.volume = fromStart; }
        if (to != null) { to.volume = targetToVol; }
    }
    public void PlayMusicPersistant(AudioClip clip, float volume = 1f, bool fade = true)
    {
        if (clip == null) return;

        if ((musicA != null && musicA.isPlaying && musicA.clip == clip) ||
            (musicB != null && musicB.isPlaying && musicB.clip == clip))
            return;

        PlayMusic(clip, volume, fade);
    }
}