using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activate : MonoBehaviour
{
    public GameObject GameObject;
    // Start is called before the first frame update
    void Start()
    {
        
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
            GameObject.SetActive(true); 
        }
    }
}
