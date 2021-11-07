using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    
    public Slider mainSlider;
    public float OldTime=0;
    public InputField Spojovnik;
    public InputField ZistiSpeed;
    public float ArivalTime;
    public float Speed;
    void Start()
    {
       Speed=float.Parse(ZistiSpeed.text);
        
    }

    // Update is called once per frame
    void Update()
    {   
        
        
        transform.Translate(((((mainSlider.value)-OldTime))*Speed), 0f, 0f);
    
            
        OldTime = mainSlider.value;
            
            
    }
}
