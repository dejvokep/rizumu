using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fullscreen : MonoBehaviour
{
    public void FullscreenToggle(bool value)
    {
        Screen.fullScreen = value;
        Debug.Log("Fullscreen: " + value);
    }
}
