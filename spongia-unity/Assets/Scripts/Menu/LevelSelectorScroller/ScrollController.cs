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
    public float mouseScrollScale = 50f;
    public float keyboardScrollScale = 10f;

    private bool isActive;


    private void checkIfActive()
    {
        isActive = transform.parent.GetSiblingIndex() == (transform.parent.parent.childCount - 3);
    }

    void OnEnable()
    {
        MenuEventManager.changedMenuEvent += checkIfActive;
    }
    void OnDisable()
    {
        MenuEventManager.changedMenuEvent -= checkIfActive;
    }


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

        checkIfActive();
    }

    void Update()
    {
        if (!isActive)
            return;



        if (EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.CompareTag("menuMapUnit"))
            EventSystem.current.SetSelectedGameObject(transform.GetChild(selectionIndex).gameObject);



        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            transform.GetChild(selectionIndex).GetComponent<ScrollUnitButton>().Select();


        if (Input.GetKeyDown(KeyCode.UpArrow))
            scroll(1);
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            scroll(-1);
        else if (Input.GetKey(KeyCode.UpArrow))
            deltaScroll += Time.deltaTime*keyboardScrollScale;
        else if (Input.GetKey(KeyCode.DownArrow))
            deltaScroll -= Time.deltaTime*keyboardScrollScale;
        else if (Input.mouseScrollDelta.y != 0)
            deltaScroll += Time.deltaTime*Input.mouseScrollDelta.y*mouseScrollScale;


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

    public void setActivedCell(GameObject cell)
    {
        int cellIndex = cell.transform.GetSiblingIndex();

        scroll(-(cellIndex - selectionIndex));
    }
}
