using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelectorController : MonoBehaviour
{
    private int mapIndex = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        updateScrollView();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            scrollUp();

        if (Input.GetKeyDown(KeyCode.DownArrow))
            scrollDown();
    }


    // Scroller movement
    public void scrollUp()
    {
        if (mapIndex + 1 == transform.childCount) //mapIndex can't be bigger than map count
            mapIndex = 0;
        else
            mapIndex += 1;

        updateScrollView();
    }

    public void scrollDown()
    {
        if (mapIndex == 0) //mapIndex can't be smaller than 0
            mapIndex = transform.childCount - 1;
        else
            mapIndex -= 1;

        updateScrollView();
    }

    private void updateScrollView() // transform.GetChild(transform.childCount-1).GetComponent<MapScrollUnitController>().setDown();
    {
        transform.GetChild(normalizeMapPosition(mapIndex+2)).GetComponent<MapScrollUnitController>().setInactive();
        transform.GetChild(normalizeMapPosition(mapIndex-2)).GetComponent<MapScrollUnitController>().setInactive();
        
        transform.GetChild(normalizeMapPosition(mapIndex+1)).GetComponent<MapScrollUnitController>().setUp();
        transform.GetChild(mapIndex).GetComponent<MapScrollUnitController>().setSelected();
        transform.GetChild(normalizeMapPosition(mapIndex-1)).GetComponent<MapScrollUnitController>().setDown();
    }

    private int normalizeMapPosition(int pos)
    {
        if (pos < 0)
            return transform.childCount + pos;
        else if (pos > transform.childCount - 1)
            return pos - transform.childCount;
        else
            return pos;
    }
}
