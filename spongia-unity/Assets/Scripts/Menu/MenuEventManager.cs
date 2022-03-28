using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuEventManager : MonoBehaviour
{
    public static event Action toggleEscapeEvent;


    public static void triggerToggleEscape()
    {
        toggleEscapeEvent?.Invoke();
    }
}
