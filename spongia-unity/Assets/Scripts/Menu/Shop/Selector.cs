using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
public class Selector : MonoBehaviour
{
    public Button playerPreview;
    public GameObject previewScroll;
    public string skinType;
    public GameObject particlePreview;
    // Start is called before the first frame update
    public void Select()
    {
        GameObject parent = transform.parent.gameObject;
        if (transform.CompareTag("Unselected"))
        {
            foreach (Transform child in parent.transform)
            {
                if (child.CompareTag("Selected"))
                {
                    
                    foreach (Transform smallChild in child.transform)
                    {
                        if (smallChild.name == "Selected")
                        {
                            smallChild.gameObject.SetActive(false);
                            child.gameObject.tag = "Unselected";
                           
                        }
                        
                    }
                    
                }
            }

            if (!(skinType == "particle_skins"))
            {
                foreach (Transform child in transform)
                {
                    if (child.name == "Button")
                    {
                        playerPreview.GetComponent<Image>().sprite = child.gameObject.GetComponent<Button>().image.sprite;
                    }
                }
            }

            if(!(previewScroll == null))
            {
                foreach (Transform child in previewScroll.transform)
                {
                    foreach (Transform smallchild in child)
                    {
                        if (smallchild.name == "Button")
                        {
                            smallchild.GetComponent<Image>().sprite = playerPreview.GetComponent<Image>().sprite;
                        }
                    }
                }
            }

            if (skinType == "particle_skins")
            {
                
                foreach (Transform child in particlePreview.transform)
                {
                    foreach (Transform smallchild in transform)
                    {
                        if (smallchild.name == "Particles")
                        {
                            ParticleSystem.MinMaxGradient gradient = smallchild.GetChild(0).GetComponent<ParticleSystem>().main.startColor;
                            var settings = child.transform.GetComponent<ParticleSystem>().main;
                            settings.startColor = gradient;
                        }
                    }
                    
                    
                }
            }

            transform.gameObject.tag = "Selected";
            Dictionary<string, Dictionary<string, bool>> skinDetails2 = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, bool>>>(File.ReadAllText(Application.persistentDataPath+"/skinInfo.json"));
            Dictionary<string, bool> skinDetails = skinDetails2[skinType];
            if (skinDetails2[skinType][transform.name])
            {
                Dictionary<string, string> equipedSkins = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Application.persistentDataPath+"/activeSkins.json"));
                equipedSkins[skinType] = transform.name;
                string json = JsonConvert.SerializeObject(equipedSkins, Formatting.Indented);
                File.WriteAllText(Application.persistentDataPath+"/activeSkins.json", json); 
            }
                
            foreach (Transform Child in transform)
            {
                if (Child.name == "Selected")
                {
                    Child.gameObject.SetActive(true);
                    
                }
            }
            
        }
        
    }
}
