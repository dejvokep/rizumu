using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollController : MonoBehaviour
{
    public SFXPlayer sfxPlayer;
    public MapInfoProcessor mapInfoProcessor;

    public float cellHeight = 100f;

    private int selectionIndex;
    private int maxIndex;

    private float deltaScroll;
    public float scrollDelay = 2f;
    [SerializeField]
    private float currentScrollSpeed;
    public float maxScrollSpeed = 20f;
    public float keyboardScrollScale = 10f;

    private bool isActive;

    private bool _isScrolling = false;

    private float lastScrolling = 0f;

    public float fadeTimeOut = 1f;


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
            
            deltaScroll += Mathf.Sign(currentScrollSpeed)*(Mathf.Abs(currentScrollSpeed) - scrollDelay)*Time.deltaTime;
        }

        bool isInteractedWith = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.mouseScrollDelta.y != 0;
        if (isInteractedWith && _isScrolling == false)
            fade(false);
        else if (lastScrolling > fadeTimeOut)
            fade(true);
        
        _isScrolling = isInteractedWith;
        if (!isInteractedWith)
            lastScrolling += Time.deltaTime;
        else
            lastScrolling = 0f;


        if(Mathf.Abs(deltaScroll) >= maxIndex/2)
            deltaScroll = Mathf.Sign(currentScrollSpeed)*Mathf.CeilToInt(maxIndex/2);

        if((int)Input.mouseScrollDelta.y != 0)
        {
            sfxPlayer.Hover();
            scroll(Mathf.Abs(Input.mouseScrollDelta.y) < maxIndex/2 ? (int)Input.mouseScrollDelta.y : (int)(maxIndex/2));
            setSelectedMapID();
        }
        else if (Mathf.Abs(deltaScroll) >= 1)
        {
            sfxPlayer.Hover();
            print(deltaScroll);
            print((int)(Mathf.FloorToInt(Mathf.Abs(deltaScroll))*Mathf.Sign(deltaScroll)));
            scroll((int)(Mathf.FloorToInt(Mathf.Abs(deltaScroll))*Mathf.Sign(deltaScroll)));
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

        _isScrolling = true;
    }


    public bool isScrolling()
    {
        return _isScrolling;
    }

    private void fade(bool fadeIn)
    {
        if (fadeIn)
            mapInfoProcessor.fadeIn();
        else
            mapInfoProcessor.fadeOut();
    }
}
