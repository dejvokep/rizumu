using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Brightness : MonoBehaviour
{
    public Image mask;

    // Start is called before the first frame update
    void Start()
    {
        mask = GetComponent<Image>();
    }

    public void UpdateBrightness(float value) {
        mask.color = new Color32((byte) 0, (byte) 0, (byte) 0, (byte) (100-value));
    }
}
