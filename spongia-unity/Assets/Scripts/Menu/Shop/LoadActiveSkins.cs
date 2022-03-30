using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
public class LoadActiveSkins : MonoBehaviour
{
    // Start is called before the first frame update
    public string skinType;
    
    void Awake()
    {
        Dictionary<string, string> equipedSkins = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Application.persistentDataPath+"/activeSkins.json"));
        transform.GetComponent<Button>().image.sprite = Resources.Load<Sprite>("Skins/"+skinType+"/"+equipedSkins[skinType]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
