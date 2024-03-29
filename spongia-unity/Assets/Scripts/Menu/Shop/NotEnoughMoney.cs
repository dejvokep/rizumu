using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;

public class NotEnoughMoney : MonoBehaviour
{
    public IEnumerator FadeTextToFullAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }
    public IEnumerator FadeTextToZeroAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }
    public IEnumerator ShowMessage (Text NotEnoughMoney)
    {
        StartCoroutine(FadeTextToFullAlpha(1f, NotEnoughMoney));
        yield return new WaitForSeconds(3);
        StartCoroutine(FadeTextToZeroAlpha(1f, NotEnoughMoney));

    }

    public void StartMessage(Text NotEnoughMoney)
    {
        StartCoroutine(ShowMessage(NotEnoughMoney));
    }
}
