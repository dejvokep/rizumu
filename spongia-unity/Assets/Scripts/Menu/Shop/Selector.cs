using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
public class Selector : MonoBehaviour
{
    public Button playerPreview;
    
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
            foreach (Transform child in transform)
            {
                if (child.name == "Button")
                {
                    playerPreview.GetComponent<Image>().sprite = child.gameObject.GetComponent<Button>().image.sprite;
                }
                

            }
            transform.gameObject.tag = "Selected";
            
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
