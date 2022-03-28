using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScript : MonoBehaviour
{
    public Slider _progressBar;
    
    void Start()
    {
        var scene = SceneManager.LoadSceneAsync("MenuScene");
        scene.allowSceneActivation = false;

        do {
            _progressBar.value = scene.progress;
        } while (scene.progress < 0.9f);

        scene.allowSceneActivation = true;
    }
}
