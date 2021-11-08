using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class UserDataReader : MonoBehaviour
{
    public static SpawnedController.JSONBase userData;

    void Awake()
    {
        LoadJsonData();
    }

    private void LoadJsonData()
    {
        try
        {
            string jsonString = File.ReadAllText(Application.persistentDataPath + "/userdata.json");
            userData = JsonConvert.DeserializeObject<SpawnedController.JSONBase>(jsonString);
        }
        catch (FileNotFoundException)
        {
            return;
        }        
    }
}
