using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public GameObject quitPopUp;
    public navigationManager navigation;
    private bool isPopUpEnabled = false;

    public void doExitGame()
    {
        Debug.Log("Exiting Game");
        Application.Quit();
    }

    public void toggleEsacpeEnabled()
    {
        MenuEventManager.triggerToggleEscape();
        isPopUpEnabled = !isPopUpEnabled;
    }

    void Update()
    {
        if (navigation.getCurrentMenu() == gameObject && Input.GetKeyDown(KeyCode.Escape))
        {
            quitPopUp.SetActive(!isPopUpEnabled);
            toggleEsacpeEnabled();
        }
    }
}
