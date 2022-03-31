using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Text;

public class particleChooser : MonoBehaviour
{
    public List<Particles> skinList;
    public GameObject Scroll;
    
    public GameObject Template;
    // Start is called before the first frame update
    void Awake()
    {
        // NotePlayer();
        // transform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public int[] hexToRgb(int bigint) {
        
        int r = (bigint >> 16) & 255;
        int g = (bigint >> 8) & 255;
        int b = bigint & 255;

        return new [] {r, g, b};
    }

    public void NotePlayer()
    {
        print(Application.persistentDataPath+"/skinInfo.json");
        Dictionary<string, Dictionary<string, bool>> skinDetails2 = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, bool>>>(File.ReadAllText(Application.persistentDataPath+"/skinInfo.json"));
        Dictionary<string, bool> skinDetails = skinDetails2["particle_skins"];
        string name = "none";
        if (Scroll.transform.childCount > 0)
        {
            name = Scroll.transform.GetChild(0).name;
        }
        
        if (name == "PlayerSkin(Clone)")
        {
            foreach (Transform child in Scroll.transform)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {


            int a = 0;
            GameObject SkinSelect;
            foreach (Particles file in skinList)
            {


                SkinSelect = Instantiate(Template, new Vector2(0, 0), Quaternion.identity);
                SkinSelect.transform.SetParent(Scroll.transform);
                foreach (Transform child in SkinSelect.transform)
                {
                    //if (child.name == "Button") child.GetComponent<Button>().image.sprite = file.skin;
                    if (child.name == "Cost")
                    {
                        if (!skinDetails[file.name])
                        {
                            child.GetComponent<Text>().text = file.cost.ToString(); 
                        }
                        else
                        {
                            child.GetComponent<Text>().text = ""; 
                        }
                    }

                    if (child.name == "Name")
                    {
                        child.GetComponent<Text>().text = file.name;
                        SkinSelect.name = file.name;
                    }
                    if (child.name == "Particles")
                    {
                        foreach (Transform smallChild in child)
                        {
                            
                            int[] colorArray = hexToRgb(Int32.Parse(file.collor1)); 
                            Color32 color = new Color32((byte)colorArray[0], (byte)colorArray[1], (byte)colorArray[2],255);
                            colorArray = hexToRgb(Int32.Parse(file.collor2)); 
                            Color32 color2 = new Color32((byte)colorArray[0], (byte)colorArray[1], (byte)colorArray[2],255);
                            ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient(color,color2);
                            var settings = smallChild.GetComponent<ParticleSystem>().main;
                            settings.startColor = gradient;
                            print(smallChild.name);
                            print(gradient);
                            print(color);
                            print(color2);
                            
                        }
                    }

                }
                
                if (skinDetails[file.name])
                {
                    foreach (Transform child in SkinSelect.transform)
                    {
                        if (child.name == "Button")
                        {
                            
                            Image _image = child.GetComponent<Button>().GetComponent<Image>();
                            Color32 newColor = _image.color;
                            newColor.a = 255;
                            _image.color = newColor;
                        }
                            
                        

                    }
                }
                SkinSelect.transform.localScale = new Vector2(1, 1);
                SkinSelect.transform.localPosition = new Vector3(0, 0 + a * 350, 0);
                a++;

            }

        }
        transform.gameObject.SetActive(false);
    }
}
