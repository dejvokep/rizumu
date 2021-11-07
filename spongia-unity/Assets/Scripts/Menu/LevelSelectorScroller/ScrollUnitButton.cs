using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrollUnitButton : MonoBehaviour
{
    public static string selectedMapID;

    public void Select()
    {
        if (selectedMapID == this.name)
        {
            SpawnedController.songID = selectedMapID;
            SceneManager.LoadScene("GameScene");
        }  
        else
            selectedMapID = this.name;
    }
}
