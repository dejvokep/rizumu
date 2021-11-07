using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataReader : MonoBehaviour
{
    public static UserData userData;

    void Awake()
    {
        LoadJsonData();
    }

    private void LoadJsonData()
    {
        if (FileManager.LoadFromFile("userdata.json", out var json))
        {
            userData.LoadFromJson(json);

            Debug.Log("userdata load complete");
        }
        else
        {
            userData = null;
            Debug.Log("userdata.json not found");
        }        
    }
}
