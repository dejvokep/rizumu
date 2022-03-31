using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class navigationManager : MonoBehaviour
{
    public SFXPlayer sfxPlayer;

    [Space(3)]
    [Header("Menu Windows")]
    public GameObject mainMenu;
    public GameObject levelSelectorMenu;
    public GameObject shopMenu;
    public GameObject settingsMenu;
    public GameObject infoMenu;


    [Space(3)]
    [Header("Black Image")]
    public GameObject windowTransitioner;

    [Space]
    [Header("Transition Configuration")]
    public float transitionSpeed = 1;
    [SerializeField]
    private int transitionFaze = -1;
    [SerializeField]
    private float transitionProgress;
    

    [Space(3)]
    public GameObject defaultMenu;
    public bool limitedMode = false;



    private GameObject lastMenu;
    private GameObject currentMenu;


    private bool isEscapeEnabled = true;
    
    void Awake()
    {
        GameObject[] menuArray = {mainMenu, levelSelectorMenu, shopMenu, settingsMenu, settingsMenu, infoMenu};

        if (defaultMenu == null)
            defaultMenu = mainMenu;

        foreach (GameObject menu in menuArray)
        {
            try
            {
                menu.SetActive(!limitedMode);
            }
            catch (UnassignedReferenceException)
            {
                
            }
        }

        defaultMenu.SetActive(true);

        lastMenu = defaultMenu;
        currentMenu = defaultMenu;

        mainMenu.transform.SetAsFirstSibling();
        currentMenu.transform.SetSiblingIndex(transform.childCount - 3);

        MenuEventManager.triggerChangedMenu();
    }

    private void toggleEsacpeEnabled()
    {
        if (isEscapeEnabled)
            isEscapeEnabled = false;
        else
            isEscapeEnabled = true;
    }

    void OnEnable()
    {
        MenuEventManager.toggleEscapeEvent += toggleEsacpeEnabled;
    }
    void OnDisable()
    {
        MenuEventManager.toggleEscapeEvent -= toggleEsacpeEnabled;
    }

    void Update()
    {
        if (transitionFaze != -1)
        {
            transitionProgress += transitionSpeed*Time.deltaTime;

            if (transitionProgress >= 1)
            {
                if (transitionFaze == 0)
                {
                    currentMenu.transform.SetSiblingIndex(transform.childCount - 3);

                    transitionFaze = 1;
                    transitionProgress = 0f;

                    lastMenu.transform.SetAsFirstSibling();

                    windowTransitioner.GetComponent<RawImage>().color = new Color32((byte)0, (byte)0, (byte)0, (byte)255);

                    MenuEventManager.triggerChangedMenu();
                }
                else
                {
                    transitionFaze = -1;
                    transitionProgress = 0f;

                    windowTransitioner.GetComponent<RawImage>().color = new Color32((byte)0, (byte)0, (byte)0, (byte)0);
                }

                return;
            }


            float newAlpha = Mathf.Abs(transitionFaze - transitionProgress)*255;
            windowTransitioner.GetComponent<RawImage>().color = new Color32((byte)0, (byte)0, (byte)0, (byte)newAlpha);
        }


        else if (Input.GetKeyDown(KeyCode.Escape) && isEscapeEnabled && currentMenu != mainMenu)
        {
            if (currentMenu == settingsMenu)
                this.GetComponent<SettingsMenu>().loadSavedSettings();

            transitionMenu(mainMenu);
            sfxPlayer.ClickBack();
        }
    }
    
    public void transitionMenu(GameObject menuWindow)
    {
        if (transitionFaze != -1)
            return;
        
        if (mainMenu.GetComponent<ExitGame>().isEnabled())
            mainMenu.GetComponent<ExitGame>().toggleEsacpeEnabled();
        
        transitionFaze = 0;
        transitionProgress = 0f;

        lastMenu = currentMenu;
        currentMenu = menuWindow;
    }

    public GameObject getCurrentMenu()
    {
        return currentMenu;
    }

    public bool isTransitioning()
    {
        return transitionFaze != -1;
    }
}
