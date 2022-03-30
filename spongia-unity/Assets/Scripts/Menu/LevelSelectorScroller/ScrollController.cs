using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollController : MonoBehaviour
{
    public SFXPlayer sfxPlayer;
    public float cellHeight = 100f;

    private int selectionIndex;
    private int maxIndex;

    private float deltaScroll;
    private float timeScrolling;
    public float scrollDelay = 2f;
    [SerializeField]
    private float currentScrollSpeed;
    public float maxScrollSpeed = 7f;
    public float mouseScrollScale = 50f;
    public float keyboardScrollScale = 7f;

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
        

        scroll(-selectionIndex);  // Select first map

        if (ScrollUnitButton.selectedMapID != transform.GetChild(selectionIndex).name)
            setSelectedMapID();

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

        
        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            currentScrollSpeed = 0;
        }


        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            scroll(1);
            setSelectedMapID();
            currentScrollSpeed = 0;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            scroll(-1);
            setSelectedMapID();
            currentScrollSpeed = 0;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            currentScrollSpeed += keyboardScrollScale*Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            currentScrollSpeed -= keyboardScrollScale*Time.deltaTime;
        }

        if (currentScrollSpeed != 0 && Mathf.Abs(currentScrollSpeed) > scrollDelay)
        {
            if (Mathf.Abs(currentScrollSpeed) > maxScrollSpeed)
                currentScrollSpeed = Mathf.Sign(currentScrollSpeed)*maxScrollSpeed;
            
            deltaScroll += Mathf.Abs(currentScrollSpeed)*(Mathf.Abs(currentScrollSpeed) - scrollDelay)*Time.deltaTime;

            // if((Input.GetKey(KeyCode.UpArrow)) || (Input.GetKey(KeyCode.DownArrow)))
            //     sfxPlayer.Hover();
        }
        

        // if(Input.mouseScrollDelta.y != 0)
        //     sfxPlayer.Hover();


        deltaScroll += Time.deltaTime*Input.mouseScrollDelta.y*mouseScrollScale;

        if(Mathf.Abs(deltaScroll) >= maxIndex/2)
            deltaScroll = Mathf.Sign(currentScrollSpeed)*Mathf.CeilToInt(maxIndex/2);


        if (Mathf.Abs(deltaScroll) >= 1)
        {
            sfxPlayer.Hover();
            scroll(Mathf.FloorToInt(Mathf.Abs(deltaScroll))*(int)Mathf.Sign(deltaScroll));
            setSelectedMapID();
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

        EventSystem.current.SetSelectedGameObject(transform.GetChild(selectionIndex).gameObject);
    }

    private void setSelectedMapID()
    {
        transform.GetChild(selectionIndex).GetComponent<ScrollUnitButton>().Select();
    }

    public void setActivedCell(GameObject cell)
    {
        int cellIndex = cell.transform.GetSiblingIndex();

        scroll(-(cellIndex - selectionIndex));

        setSelectedMapID();
    }
}
