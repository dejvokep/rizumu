using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeOpacity : MonoBehaviour
{
    
    public GameObject currentGameObject;
    public float alpha;
    //Get current material
    public Material currentMat;

    // // Start is called before the first frame update
  

    public void ChangeAlpha()
    {
    currentGameObject = transform.gameObject;
    currentMat = currentGameObject.GetComponent<Renderer>().material;

    Renderer oldColor = currentGameObject.GetComponent<Renderer>();
    Color c = oldColor.material.color;
    c.a = 0;
    

    // public Image ButtonBackground;
    //
    // Color color = ButtonBackground.color;
    // color.a = transparency;
    // ButtonBackground.color = color;
 
}

// public Button myButton;
// // Use this for initialization
// void Start () {
//     myButton.image.color = new Color(255f,255f,255f,.5f);
// }

// // Update is called once per frame
// void Update () {

// }
            
}