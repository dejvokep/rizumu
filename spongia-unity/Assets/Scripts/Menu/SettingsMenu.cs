using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    UnityEngine.Resolution[] resolutions;

    
    public Image brightnessMask; // Brightness Adjustment
    [Space]
    public AudioMixer audioMixer;
    [Space]
    [Space]
    public Dropdown resolutionDropdown;
    public Dropdown graphicsDropdown;
    public Slider brightnessSlider;
    public Toggle fullscreenToggle;
    [Space]
    public Slider volumeSlider;


    private int resolutionIndex;
    private SaveSettings curSettings = new SaveSettings();


    // Saving data
    private string saveFileName = "settings.dat";
    public void SaveJsonData()
    {
        if (FileManager.WriteToFile(saveFileName, curSettings.ToJson()))
        {
            Debug.Log("Save successful");
        }
    }
    
    public void LoadJsonData()
    {
        if (FileManager.LoadFromFile(saveFileName, out var json))
        {
            curSettings.LoadFromJson(json);

            Debug.Log("Load complete");
        }
        else
        {
            curSettings.Default();

            SaveJsonData();
        }        
    }



    void Start()
    {
        // Resoluion dropdown setup
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + "  ";
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);

        // Load Settings
        LoadJsonData();
        UpdateAllSettings();
    }

    private int getResolutionIndex()
    {
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == curSettings.resolutionWidth &&
                resolutions[i].height == curSettings.resolutionHeight)
            {
                return i;
            }
        }
        return resolutions.Length - 1;
    }

    public void UpdateAllSettings()
    {
        // Resolution
        Screen.SetResolution(curSettings.resolutionWidth, curSettings.resolutionHeight, Screen.fullScreen);

        // Quality
        UnityEngine.QualitySettings.SetQualityLevel(curSettings.qualityIndex);

        // Brightness
        brightnessMask.color = new Color32((byte) 0, (byte) 0, (byte) 0, (byte) (100f - curSettings.brightness));

        // Fulscreen
        Screen.fullScreen = curSettings.isFullscreen;

        // Volume
        audioMixer.SetFloat("volume", curSettings.volume);


        resolutionIndex = getResolutionIndex(); resolutionDropdown.value = resolutionIndex;
        graphicsDropdown.value = curSettings.qualityIndex;
        brightnessSlider.value = curSettings.brightness;
        fullscreenToggle.isOn = curSettings.isFullscreen;

        volumeSlider.value = curSettings.volume;
    }

    public void DefaultSettings()
    {
        curSettings.Default();
    }


    // Resolution Dropdown
    public void SetResolution (int index)
    {
        resolutionIndex = index;
        curSettings.resolutionWidth = resolutions[index].width;
        curSettings.resolutionHeight = resolutions[index].height;

        Screen.SetResolution(curSettings.resolutionWidth, curSettings.resolutionHeight, Screen.fullScreen);
    }

    // Graphics (Quality) Dropdown
    public void SetQuality(int index)
    {
        curSettings.qualityIndex = index;

        UnityEngine.QualitySettings.SetQualityLevel(curSettings.qualityIndex);
    }

    // Brightness Slider
    public void UpdateBrightness(float value)
    {
        curSettings.brightness = value;

        brightnessMask.color = new Color32((byte) 0, (byte) 0, (byte) 0, (byte) (100 - curSettings.brightness));
    }

    // Fullscreen Toggle
    public void SetFullscreen(bool value)
    {
        curSettings.isFullscreen = value;

        Screen.fullScreen = curSettings.isFullscreen;
    }

    // Volume Slider
    public void SetVolume(float value)
    {
        curSettings.volume = value;

        audioMixer.SetFloat("volume", curSettings.volume);
    }
}