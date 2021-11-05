using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollerUnitGridLayout : MonoBehaviour
{    
    private float GridWidth = 0;
    public IEnumerator SetGridWidth() {
        yield return new WaitForEndOfFrame();
        GridWidth = transform.GetComponent<RectTransform>().sizeDelta.x;
    }

    void Start()
    {
        StartCoroutine(SetGridWidth());

        Debug.Log(GridWidth);
    }
}
