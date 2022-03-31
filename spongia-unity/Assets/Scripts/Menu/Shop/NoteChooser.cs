using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Newtonsoft.Json;
public class NoteChooser : MonoBehaviour
{
    public List<Skins> skinList;
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

    public void NotePlayer()
    {
        Dictionary<string, Dictionary<string, bool>> skinDetails2 = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, bool>>>(File.ReadAllText(Application.persistentDataPath+"/skinInfo.json"));
        Dictionary<string, bool> skinDetails = skinDetails2["note_skins"];
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
            foreach (Skins file in skinList)
            {


                SkinSelect = Instantiate(Template, new Vector2(0, 0), Quaternion.identity);
                SkinSelect.transform.SetParent(Scroll.transform);
                foreach (Transform child in SkinSelect.transform)
                {
                    if (child.name == "Button") child.GetComponent<Button>().image.sprite = file.skin;
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
