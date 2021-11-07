using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class UserData
{
    public long sp {get; set;}
    public Dictionary<string, SpawnedController.JSONHighscore> highscores {get; set;}

    public void LoadFromJson(string a_Json)
    {
        JsonUtility.FromJsonOverwrite(a_Json, this);
    }
}
