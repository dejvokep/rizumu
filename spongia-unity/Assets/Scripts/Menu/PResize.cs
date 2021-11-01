using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PResize : MonoBehaviour
{

    public int _band;
    public float _startScale, _scaleMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3((PulsingObject._freqBand[_band] * _scaleMultiplier) + _startScale, (PulsingObject._freqBand[_band] * _scaleMultiplier)+ _startScale, transform.localScale.z);
    }
}