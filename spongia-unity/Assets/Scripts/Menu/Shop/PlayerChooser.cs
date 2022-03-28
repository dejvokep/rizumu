using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChooser : MonoBehaviour
{
    public List<Skins> skinList;
    public GameObject Scroll;
    public List<GameObject> Children;
    public GameObject Template;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChoosePlayer()
    {


        int a = 0;
        GameObject SkinSelect;
        foreach(Skins file in skinList)
        {
            
            
            SkinSelect = Instantiate(Template, new Vector2(0, 0), Quaternion.identity);
            SkinSelect.transform.SetParent(Scroll.transform);
            foreach(Transform child in SkinSelect.transform)
            {
                Children.Add(child.gameObject);
            }

            Children[0].GetComponent<Button>().image.sprite = file.skin;
            Children[1].GetComponent<Text>().text = file.cost.ToString();
            Children[2].GetComponent<Text>().text = file.name;
            SkinSelect.transform.localScale = new Vector2(1, 1);
            SkinSelect.transform.localPosition = new Vector3(0, 0 + a*350, 0);
            a++;
            Children.Clear();
        }
        
    }
}
