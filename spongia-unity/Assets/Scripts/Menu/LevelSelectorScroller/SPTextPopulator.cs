using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SPTextPopulator : MonoBehaviour
{
    public Text spText;
    void Start()
    {
        try
        {
            spText.text = $"SP: {UserDataReader.userData.sp.ToString()}";
        }
        catch (NullReferenceException e)
        {
            print(e);
            return;
        }
    }
}
