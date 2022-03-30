using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuEventManager : MonoBehaviour
{
    public static event Action toggleEscapeEvent;

    public static event Action changedMenuEvent;


    public static void triggerToggleEscape()
    {
        toggleEscapeEvent?.Invoke();
        print("TOGGLE ESCAPE");
    }

    public static void triggerChangedMenu()
    {
        changedMenuEvent?.Invoke();
    }
}
