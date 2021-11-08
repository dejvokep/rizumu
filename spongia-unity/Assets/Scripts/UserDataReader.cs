using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataReader : MonoBehaviour
{
    public static UserData userData;

    void Awake()
    {
        userData = new UserData();
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
            Debug.Log("userdata.json not found, creating new file");

            userData.Default();

            string jsonString = userData.ToJson();
            if (FileManager.WriteToFile("userdata.json", jsonString))
            {
                Debug.Log("Save successful");
            }
        }        
    }
}
