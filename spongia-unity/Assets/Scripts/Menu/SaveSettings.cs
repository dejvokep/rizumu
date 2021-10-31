using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveSettings
{
    // Video
    public int[] resolution;
    public int qualityIndex = 5;
    public float brightness = 80f;
    public bool isFullscreen = true;
    // Audio
    public float volume = -40f;
    // Controls
    // public List<string> primaryControls;
    // public List<string> secondaryControls;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string a_Json)
    {
        JsonUtility.FromJsonOverwrite(a_Json, this);
    }
}