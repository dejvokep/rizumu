using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
public class particleBuyer : MonoBehaviour
{
    public Text NotEnoughMoney;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Text textObj;
    // Update is called once per frame
    public IEnumerator FadeTextToFullAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }
    public IEnumerator FadeTextToZeroAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }
    IEnumerator ShowMessage ()
    {
        StartCoroutine(FadeTextToFullAlpha(1f, NotEnoughMoney));
        yield return new WaitForSeconds(3);
        StartCoroutine(FadeTextToZeroAlpha(1f, NotEnoughMoney));

    }
    public void Buy()
    {
        if(transform.tag=="Selected")
        {
            int Cost = 0;
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
                    StartCoroutine(ShowMessage());
                    return;
                    
                }
                skinDetails[transform.name] = true;
                foreach (Transform child in transform)
                {
                    if (child.name == "Cost")
                    {
                        data["sp"] = Int32.Parse(data["sp"].ToString()) - Int32.Parse(child.GetComponent<Text>().text);
                        child.GetComponent<Text>().text = "";
                        string json2 = JsonConvert.SerializeObject(data, Formatting.Indented);
                        File.WriteAllText(Application.persistentDataPath+"/userdata.json",json2 );
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
            textObj.GetComponent<Money>().updateFinasec();
            
        }
        
    }
}
