using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollController : MonoBehaviour
{
    public float cellHeight = 100f;

    private int selectionIndex;
    private int maxIndex;

    private float deltaScroll;
    public float scrollScale = 0.1f;


    void Start()
    {
        maxIndex = transform.childCount -1;

        if (transform.childCount % 2 == 0)
        {
            selectionIndex = transform.childCount / 2;
            transform.localPosition = new Vector3(0, cellHeight, 0);
        }
        else
            selectionIndex = (transform.childCount - 1) / 2;
        
        EventSystem.current.SetSelectedGameObject(transform.GetChild(selectionIndex).gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            transform.GetChild(selectionIndex).GetComponent<ScrollUnitButton>().Select();


        if (Input.GetKey(KeyCode.UpArrow))
        {
            deltaScroll += Time.deltaTime*scrollScale*0.0001f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            deltaScroll -= Time.deltaTime*scrollScale*0.0001f;
        }
        else if (Input.mouseScrollDelta.y != 0)
        {
            deltaScroll += Time.deltaTime*Input.mouseScrollDelta.y*scrollScale*2;
        }


        if (Mathf.Abs(Input.mouseScrollDelta.y) == 1)
        {
            scroll((int)Mathf.Sign(deltaScroll));
            deltaScroll = 0;
        }
        else if (deltaScroll >= 1 || deltaScroll <= -1)
        {
            scroll(Mathf.FloorToInt(Mathf.Abs(deltaScroll))*(int)Mathf.Sign(deltaScroll));
            deltaScroll = 0f;
        }
    }

    private void scroll(int scrollCount)
    {
        for (int i = 0; i < Mathf.Abs(scrollCount); i++)
        {
            if (scrollCount > 0)
                transform.GetChild(maxIndex).SetAsFirstSibling();
            else
                transform.GetChild(0).SetAsLastSibling();
        }

        Transform cellTransform = transform.GetChild(selectionIndex);

        EventSystem.current.SetSelectedGameObject(cellTransform.gameObject);
        cellTransform.GetComponent<ScrollUnitButton>().Select();
    }
}
