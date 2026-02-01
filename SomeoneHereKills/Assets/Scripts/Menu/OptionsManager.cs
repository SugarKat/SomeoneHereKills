using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using System.Collections.Generic;

public class OptionsManager : MonoBehaviour
{
    [Header("UI")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider ambienceSlider;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    [Header("Audio")]
    public AudioMixer audioMixer;

    private Resolution[] resolutions;

    private float tempMasterVol;
    private float tempMusicVol;
    private float tempSfxVol;
    private float tempAmbienceVol;
    private int tempResolutionIndex;
    private bool tempFullscreen;

    private const string MASTER = "MasterVolume";
    private const string MUSIC = "MusicVolume";
    private const string SFX = "SFXVolume";
    private const string AMBIENCE = "AmbienceVolume";
    private const string RES = "ResolutionIndex";
    private const string FS = "Fullscreen";

    private void Start()
    {
        LoadSettings();
        SetupResolutions();
        RefreshUI();
        ApplySettings();
    }

    void LoadSettings()
    {
        tempMasterVol = PlayerPrefs.GetFloat(MASTER, 1f);
        tempMusicVol = PlayerPrefs.GetFloat(MUSIC, 1f);
        tempSfxVol = PlayerPrefs.GetFloat(SFX, 1f);
        tempAmbienceVol = PlayerPrefs.GetFloat(AMBIENCE, 1f);
        tempResolutionIndex = PlayerPrefs.GetInt(RES, 0);
        tempFullscreen = PlayerPrefs.GetInt(FS, 1) == 1;
    }

    void RefreshUI()
    {
        resolutionDropdown.value = tempResolutionIndex;
        fullscreenToggle.isOn = tempFullscreen;
        masterSlider.value = tempMasterVol;
        musicSlider.value = tempMusicVol;
        sfxSlider.value = tempSfxVol;
        ambienceSlider.value = tempAmbienceVol;
    }

    void SetupResolutions()
    {
        List<Resolution> unique = new List<Resolution>();
        List<string> options = new List<string>();

        foreach (Resolution r in Screen.resolutions)
        {
            if (unique.Exists(x => x.width == r.width && x.height == r.height))
                continue;

            unique.Add(r);
            options.Add($"{r.width} x {r.height}");
        }

        resolutions = unique.ToArray();
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
    }

    public void OnApply()
    {
        tempMasterVol = masterSlider.value;
        tempMusicVol = musicSlider.value;
        tempSfxVol = sfxSlider.value;
        tempAmbienceVol = ambienceSlider.value;
        tempResolutionIndex = resolutionDropdown.value;
        tempFullscreen = fullscreenToggle.isOn;

        ApplySettings();
        SaveSettings();

        Debug.Log("Options Applied");
    }

    void ApplySettings()
    {
        // Audio (0 = mute, 1 = max)
        SetVolume01(MASTER, tempMasterVol);
        SetVolume01(MUSIC, tempMusicVol);
        SetVolume01(SFX, tempSfxVol);
        SetVolume01(AMBIENCE, tempAmbienceVol);

        // Fullscreen / Resolution
        Screen.fullScreen = tempFullscreen;

        if (tempResolutionIndex >= 0 && tempResolutionIndex < resolutions.Length)
        {
            Resolution r = resolutions[tempResolutionIndex];
            Screen.SetResolution(r.width, r.height, tempFullscreen);
        }
    }

    void SaveSettings()
    {
        PlayerPrefs.SetFloat(MASTER, tempMasterVol);
        PlayerPrefs.SetFloat(MUSIC, tempMusicVol);
        PlayerPrefs.SetFloat(SFX, tempSfxVol);
        PlayerPrefs.SetFloat(AMBIENCE, tempAmbienceVol);
        PlayerPrefs.SetInt(RES, tempResolutionIndex);
        PlayerPrefs.SetInt(FS, tempFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnCancel()
    {
        RefreshUI();
        Debug.Log("Changes Reverted");
    }

    const float MIN_DB = -80f;
    const float MAX_DB = 0f;

    void SetVolume01(string param, float v01)
    {
        if (audioMixer == null) return;

        if (v01 <= 0.0001f)
        {
            audioMixer.SetFloat(param, MIN_DB);
            return;
        }

        float db = Mathf.Log10(v01) * 20f;
        db = Mathf.Clamp(db, MIN_DB, MAX_DB);
        audioMixer.SetFloat(param, db);
    }

}
