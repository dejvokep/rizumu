using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SongInfo
{
    public string song_name;
    public string song_author;
    public int difficulty;

    public void Default()
    {
        song_name = "MISSING SONG NAME";
        song_author = "MISSING SONG AUTHOR";
        difficulty = 0;
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