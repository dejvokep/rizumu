using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollerWidthCalculator : MonoBehaviour
{    
    public static float gridWidth = 0;
    private bool setUpComplete = false;
    public IEnumerator SetGridWidth() {
        yield return new WaitForEndOfFrame();
        gridWidth = transform.GetComponent<RectTransform>().sizeDelta.x;
    }

    void Update()
    {
        if (gridWidth == 0)
        {
            StartCoroutine(SetGridWidth());
            return;
        }

        if (setUpComplete)
        {
            this.GetComponent<ScrollerWidthCalculator>().enabled = false;
        }

        Debug.Log(gridWidth);
        setUpComplete = true;
    }
}
