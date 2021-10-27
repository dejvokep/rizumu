using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public Dropdown resolutionDropdown;

    UnityEngine.Resolution[] resolutions;


    public Image mask; //Brightness Adjustment


    public AudioMixer audioMixer;


    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution (int resolutionIndex)
    {
        UnityEngine.Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }


    // Fulscreen Toggle
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }


    public void UpdateBrightness(float value)
    {
        mask.color = new Color32((byte) 0, (byte) 0, (byte) 0, (byte) (100-value));
    }


    public void SetVolume(float value)
    {
        audioMixer.SetFloat("volume", value);
    }


    public void SetQuality(int qualityIndex)
    {
        UnityEngine.QualitySettings.SetQualityLevel(qualityIndex);
    }
}
