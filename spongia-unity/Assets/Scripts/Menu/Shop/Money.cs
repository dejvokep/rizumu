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
        Dictionary<string, dynamic> data = JsonConvert.DeserializeObject<Dictionary<string,dynamic >>(File.ReadAllText(Application.persistentDataPath+"/userdata.json"));

        textObj.GetComponent<Text>().text = data["sp"];
    }

    public void updateFinasec()
    {
        Dictionary<string, dynamic> data = JsonConvert.DeserializeObject<Dictionary<string,dynamic >>(File.ReadAllText(Application.persistentDataPath+"/userdata.json"));

        textObj.GetComponent<Text>().text = data["sp"];
    }

}
