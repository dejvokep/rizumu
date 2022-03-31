using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class Activate : MonoBehaviour
{
    public GameObject GameObject;
    public GameObject Scroll;
    public GameObject NoteScroll;
    public GameObject ParticleScroll;
    // Start is called before the first frame update
    void Awake()
    {
        // Dictionary<string, string> equipedSkins = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Application.persistentDataPath+"/activeSkins.json"));
        //
        // foreach (Transform child in Scroll.transform)
        // {
        //
        //     if (child.name == equipedSkins["player_skins"])
        //     {
        //         Scroll.GetComponent<ShopScrollController>().setActivedCell(child.gameObject);
        //     }
        // }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Button()
    {
        if (GameObject.active)
        {
            GameObject.SetActive(false);
        }
        else
        {
            Dictionary<string, string> equipedSkins = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Application.persistentDataPath+"/activeSkins.json"));
            
            foreach (Transform child in Scroll.transform)
            {

                if (child.name == equipedSkins["player_skins"])
                {
                    Scroll.GetComponent<ShopScrollController>().setActivedCell(child.gameObject);
                }
            }
            
            
            NoteScroll.SetActive(false);
            ParticleScroll.SetActive(false);
            GameObject.SetActive(true); 
        }
    }
}
