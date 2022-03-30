using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
public class Buyer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Buy()
    {
        if(transform.tag=="Selected")
        {
            Dictionary<string, Dictionary<string, bool>> skinDetails2 = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, bool>>>(File.ReadAllText(Application.persistentDataPath+"/skinInfo.json"));
            Dictionary<string, bool> skinDetails = skinDetails2["player_skins"];
            if (!skinDetails[transform.name])
            {
                skinDetails[transform.name] = true;
                foreach (Transform child in transform)
                {
                    if (child.name == "Cost")
                    {
                        child.GetComponent<Text>().text = "";
                    }
                    if (child.name == "Button")
                    {
                            
                        Image _image = child.GetComponent<Button>().GetComponent<Image>();
                        Color32 newColor = _image.color;
                        newColor.a = 255;
                        _image.color = newColor;
                    }
                    

                }
            }
            string json = JsonConvert.SerializeObject(skinDetails2, Formatting.Indented);
            File.WriteAllText(Application.persistentDataPath+"/skinInfo.json", json);
        }
    }
}
