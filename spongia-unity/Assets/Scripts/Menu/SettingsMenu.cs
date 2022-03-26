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
    [Space]
    [Space]
    public GameObject controlButtonsHolder;

    private GameObject currentKey;

    
    // Keyboard keys by sector

    private int resolutionIndex;
    private SaveSettings curSettings = new SaveSettings();


    // Saving data
    private string saveFileName = "settings.json";

    public static float audioVolume = -30;

    public void SaveJsonData()
    {
        updateKeys();
        audioVolume = curSettings.volume;
        
        if (FileManager.WriteToFile(saveFileName, curSettings.ToJson()))
        {
            // Debug.Log("Save successful");
        }
    }
    
    public void LoadJsonData()
    {
        if (FileManager.LoadFromFile(saveFileName, out var json))
        {
            curSettings.Default();

            curSettings.LoadFromJson(json);

            // Debug.Log("Load complete");
        }
        else
        {
            curSettings.Default();

            SaveJsonData();
        }        
    }



    void Start()
    {
        if (!File.Exists(Application.persistentDataPath + "/settings.json"))
        {
            curSettings.Default();
            SaveJsonData();
        }
        
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
        DefaultSettings();
        LoadJsonData();

        // Update Settings
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
        audioVolume = curSettings.volume;


        resolutionIndex = getResolutionIndex(); resolutionDropdown.value = resolutionIndex;
        graphicsDropdown.value = curSettings.qualityIndex;
        brightnessSlider.value = curSettings.brightness;
        fullscreenToggle.isOn = curSettings.isFullscreen;

        volumeSlider.value = curSettings.volume;
        
        updateKeys();

        // KeyControls
        UpdateAllControlButtons();
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


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && current != null)
        {
            currentKey.GetComponent<Image>().color = new Color32((byte) 255, (byte) 255, (byte) 255, (byte) (255));
            currentKey = null;
        }
    }

    void OnGUI()
    {
        if (currentKey != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                if (e.keyCode != KeyCode.Escape)
                {
                    curSettings.keyboardKeys[GetSectorIndexBySectorName(currentKey.name)] = e.keyCode.ToString().ToLower();
                    Debug.Log(indexToSector(GetSectorIndexBySectorName(currentKey.name)));
                    Debug.Log(curSettings.keyboardKeys[GetSectorIndexBySectorName(currentKey.name)]);
                    controlButtonsHolder.transform.GetChild(GetSectorIndexBySectorName(currentKey.name)).GetChild(0).GetComponent<Text>().text = e.keyCode.ToString().ToUpper();

                    currentKey.GetComponent<Image>().color = new Color32((byte) 255, (byte) 255, (byte) 255, (byte) (255));
                    currentKey = null;
                }
            }
        }
    }
    
    public void SwitchControlsButtonHandler(GameObject clicked)
    {
        if (currentKey == null)
        {
            currentKey = clicked;

            currentKey.GetComponent<Image>().color = new Color32((byte) 255, (byte) 100, (byte) 100, (byte) 255);
        }
    }

    private void UpdateAllControlButtons()
    {
        for (int i = 0; i < 4; i++)
        {
            controlButtonsHolder.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = curSettings.keyboardKeys[i].ToUpper();
        }
    }

    private int GetSectorIndexBySectorName(string name)
    {
        switch (name)
        {
            default:
                return -1;
            case "NE":
                return 0;
            case "SE":
                return 1;
            case "SW":
                return 2;
            case "NW":
                return 3;
        }
    }

    private Sector indexToSector(int index)
    {
        switch (index)
        {
            default:
                return Sector.NORTH_EAST;
            case 0:
                return Sector.NORTH_EAST;
            case 1:
                return Sector.SOUTH_EAST;
            case 2:
                return Sector.SOUTH_WEST;
            case 3:
                return Sector.NORTH_WEST;
        }
    }

    private void updateKeys()
    {
        SpawnedController.keyboardKeys = new Dictionary<Sector, string>();
        SpawnedController.keyboardKeys[Sector.NORTH_EAST] = curSettings.keyboardKeys[0].ToLower();
        SpawnedController.keyboardKeys[Sector.SOUTH_EAST] = curSettings.keyboardKeys[1].ToLower();
        SpawnedController.keyboardKeys[Sector.SOUTH_WEST] = curSettings.keyboardKeys[2].ToLower();
        SpawnedController.keyboardKeys[Sector.NORTH_WEST] = curSettings.keyboardKeys[3].ToLower();
    }
}