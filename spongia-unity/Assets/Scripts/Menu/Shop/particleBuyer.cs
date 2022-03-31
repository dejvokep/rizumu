using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
public class particleBuyer : MonoBehaviour
{
    public Text NoMoney;
    public Text textObj;
    public GameObject NotEnoughMoney;
    public string skinType;
    public GameObject particlePreview;
    // Update is called once per frame
    
    public void Buy()
    {
        if(transform.tag=="Selected")
        {
            int Cost = 0;
            string json;
            Dictionary<string, Dictionary<string, bool>> skinDetails2 = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, bool>>>(File.ReadAllText(Application.persistentDataPath+"/skinInfo.json"));
            Dictionary<string, bool> skinDetails = skinDetails2["particle_skins"];
            Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string,object >>(File.ReadAllText(Application.persistentDataPath+"/userdata.json"));

            if (!skinDetails[transform.name])
            {
                foreach (Transform child in transform)
                {
                    if (child.name == "Cost")
                    {
                        Cost =  Int32.Parse(child.GetComponent<Text>().text);
                        
                    }

                }

                if (Cost > Int32.Parse(data["sp"].ToString()))
                {
                    NotEnoughMoney.GetComponent<NotEnoughMoney>().StartMessage(NoMoney);
                    return;
                    
                }
                skinDetails[transform.name] = true;
                foreach (Transform child in transform)
                {
                    if (child.name == "Cost")
                    {
                        
                        
                        Dictionary<string, string> equipedSkins = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Application.persistentDataPath+"/activeSkins.json"));
                        equipedSkins[skinType] = transform.name;
                        
                        Particles[][] particlesFolder = new[] {Resources.LoadAll<Particles>("Skins/particle_skins")};
                        foreach (Particles[] particle in particlesFolder)
                        {
                            foreach (Particles particleData in particle)
                            {
                                print(transform.name);
                                if (particleData.name == transform.name)
                                {
                                    equipedSkins["particle_skin_color"] =
                                        particleData.collor1 + "," + particleData.collor2;
                                }
                            }
                        }
                        
                        json = JsonConvert.SerializeObject(equipedSkins, Formatting.Indented);
                        File.WriteAllText(Application.persistentDataPath+"/activeSkins.json", json); 
                                                
                        data["sp"] = Int32.Parse(data["sp"].ToString()) - Int32.Parse(child.GetComponent<Text>().text);
                        child.GetComponent<Text>().text = "";
                        json= JsonConvert.SerializeObject(data, Formatting.Indented);
                        File.WriteAllText(Application.persistentDataPath+"/userdata.json",json);
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
            json = JsonConvert.SerializeObject(skinDetails2, Formatting.Indented);
            File.WriteAllText(Application.persistentDataPath+"/skinInfo.json", json);
            textObj.GetComponent<Money>().updateFinasec();
            
        }
        
    }
}
