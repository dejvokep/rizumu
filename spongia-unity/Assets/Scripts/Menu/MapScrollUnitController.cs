using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScrollUnitController : MonoBehaviour
{
    public void setInactive()
    {
        transform.Find("selected").gameObject.SetActive(false);
        transform.Find("up").gameObject.SetActive(false);
        transform.Find("down").gameObject.SetActive(false);
    }

    public void setSelected()
    {
        transform.Find("selected").gameObject.SetActive(true);
        
        transform.Find("up").gameObject.SetActive(false);
        transform.Find("down").gameObject.SetActive(false);
    }

    public void setUp()
    {
        transform.Find("up").gameObject.SetActive(true);
    }

    public void setDown()
    {
        transform.Find("down").gameObject.SetActive(true);
    }
}
