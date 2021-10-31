using System.Collections;
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



    // Video
    private int resolutionIndex;
    private int[] resolution;
    private int qualityIndex;
    private float brightness;
    private bool isFullscreen;
    // Audio
    private float volume;
    // Controls
    // private List<string> primaryControls;
    // private List<string> secondaryControls;


    // Saving data
    private void SaveJsonData()
    {
        SaveSettings sd = new SaveSettings();
        
        PopulateSaveData(sd);

        if (FileManager.WriteToFile("settings.dat", sd.ToJson()))
        {
            Debug.Log("Save successful");
        }
    }

    private void PopulateSaveData(SaveSettings a_SaveData)
    {
        a_SaveData.resolution = resolution;
        a_SaveData.qualityIndex = qualityIndex;
        a_SaveData.brightness = brightness;
        a_SaveData.isFullscreen = isFullscreen;

        a_SaveData.volume = volume;

        // a_SaveData.primaryControls = primaryControls;
        // a_SaveData.secondaryControls = secondaryControls;
    }
    

    private void LoadJsonData()
    {
        if (FileManager.LoadFromFile("settings.dat", out var json))
        {
            SaveSettings sd = new SaveSettings();
            sd.LoadFromJson(json);

            LoadFromSaveData(sd);
            
            Debug.Log("Load complete");
        }
        else
        {
            SaveSettings sd = new SaveSettings();
            sd.resolution = new int[]{Screen.currentResolution.width, Screen.currentResolution.height};  // If resolution not in save

            LoadFromSaveData(sd);
        }
    }

    private void LoadFromSaveData(SaveSettings a_SaveData)
    {
        resolution = a_SaveData.resolution;
        qualityIndex = a_SaveData.qualityIndex;
        brightness = a_SaveData.brightness;
        isFullscreen = a_SaveData.isFullscreen;

        volume = a_SaveData.volume;

        // primaryControls = a_SaveData.primaryControls;
        // secondaryControls = a_SaveData.secondaryControls;
    }



    void Start()
    {
        // Resoluion dropdown setup
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);
        
        resolutionIndex = getResolutionIndex(Screen.currentResolution);
        resolutionDropdown.value = resolutionIndex;

        resolutionDropdown.RefreshShownValue();

        // Load Settings
        ResetSettings();
    }

    private int getResolutionIndex(UnityEngine.Resolution resolution)
    {
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == resolution.width &&
                resolutions[i].height == resolution.height)
            {
                return i;
            }
        }
        return resolutions.Length - 1;
    }


    public void ApplySettings()
    {
        SaveJsonData();
    }

    public void ResetSettings()
    {
        LoadJsonData();


        // Resolution
        Screen.SetResolution(resolution[0], resolution[1], Screen.fullScreen);

        // Quality
        UnityEngine.QualitySettings.SetQualityLevel(qualityIndex);

        // Brightness
        brightnessMask.color = new Color32((byte) 0, (byte) 0, (byte) 0, (byte) (100 - brightness));

        // Fulscreen
        Screen.fullScreen = isFullscreen;

        // Volume
        audioMixer.SetFloat("volume", volume);


        resolutionIndex = getResolutionIndex(Screen.currentResolution); resolutionDropdown.value = resolutionIndex;
        graphicsDropdown.value = qualityIndex;
        brightnessSlider.value = brightness;
        fullscreenToggle.isOn = isFullscreen;

        volumeSlider.value = volume;
    }


    // Resolution Dropdown
    public void SetResolution (int index)
    {
        resolutionIndex = index;
        resolution = new int[]{resolutions[index].width, resolutions[index].height};

        Screen.SetResolution(resolution[0], resolution[1], Screen.fullScreen);
    }

    // Graphics (Quality) Dropdown
    public void SetQuality(int index)
    {
        qualityIndex = index;

        UnityEngine.QualitySettings.SetQualityLevel(qualityIndex);
    }

    // Brightness Slider
    public void UpdateBrightness(float value)
    {
        brightness = value;

        brightnessMask.color = new Color32((byte) 0, (byte) 0, (byte) 0, (byte) (100 - brightness));
    }

    // Fullscreen Toggle
    public void SetFullscreen(bool value)
    {
        isFullscreen = value;

        Screen.fullScreen = isFullscreen;
    }

    // Volume Slider
    public void SetVolume(float value)
    {
        volume = value;

        audioMixer.SetFloat("volume", volume);
    }
}