using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameQuitScript : MonoBehaviour
{
    private AsyncOperation sceneLoader;

    void Start()
    {
        sceneLoader = SceneManager.LoadSceneAsync("MenuScene");
        sceneLoader.allowSceneActivation = false;
    }

    public void loadMainScene()
    {
        float time = Time.realtimeSinceStartup;
        sceneLoader.allowSceneActivation = true;
    }
}
