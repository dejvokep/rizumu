using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;

public class ActivateParticle : MonoBehaviour
{
    public GameObject GameObject;
    public GameObject Scroll;
    public GameObject PlayerScroll;
    public GameObject NoteScroll;

    public GameObject previewScroll;
    public Button playerPreview;
    // Start is called before the first frame update
    void Awake()
    {
        // Dictionary<string, string> equipedSkins = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Application.persistentDataPath+"/activeSkins.json"));
        //
        // foreach (Transform child in Scroll.transform)
        // {
        //
        //     if (child.name == equipedSkins["note_skins"])
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
        
        if (GameObject.active)
        {
            GameObject.SetActive(false);
        }
        else
        {
            Dictionary<string, string> equipedSkins = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Application.persistentDataPath+"/activeSkins.json"));
           
            PlayerScroll.SetActive(false);
            NoteScroll.SetActive(false);
            GameObject.SetActive(true); 

            
            Scroll.GetComponent<ShopScrollController>().setActivedCell(GameObject.Find(equipedSkins["particle_skins"]));
        }
    }
}