using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
public class Money : MonoBehaviour
{
    public Text textObj;

    private void Awake()
    {
        
        
        Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string,object >>(File.ReadAllText(Application.persistentDataPath+"/userdata.json"));

        if (!data.ContainsKey("sp"))
        {
            textObj.text = "Points: 0";
        }
        else
        {
            textObj.text = $"Points: {data["sp"].ToString()}";
        }
        
    }

    public void updateFinasec()
    {
        Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string,object >>(File.ReadAllText(Application.persistentDataPath+"/userdata.json"));
        
        textObj.text = $"Points: {data["sp"].ToString()}";
    }

}
