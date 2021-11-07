using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveSettings
{
    // Video
    public int resolutionWidth;
    public int resolutionHeight;
    public int qualityIndex;
    public float brightness;
    public bool isFullscreen;
    // Audio
    public float volume;
    // Controls
    public string keyNE;
    public string keySE;
    public string keySW;
    public string keyNW;

    public void Default()
    {
        resolutionWidth = 1920;
        resolutionHeight = 1080;
        qualityIndex = 5;
        brightness = 80f;
        isFullscreen = true;

        volume = -40f;

        keyNE = "F";
        keySE = "K";
        keySW = "J";
        keyNW = "D";
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string a_Json)
    {
        JsonUtility.FromJsonOverwrite(a_Json, this);
    }
}