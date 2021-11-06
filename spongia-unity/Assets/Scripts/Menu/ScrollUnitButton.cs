using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollUnitButton : MonoBehaviour
{
    public static string selectedMapID;

    public void Select()
    {
        selectedMapID = this.name;
    }
}
