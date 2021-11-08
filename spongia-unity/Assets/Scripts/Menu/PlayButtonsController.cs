using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonsController : MonoBehaviour
{
    public GameObject objectWithAnimator;
    private Animator animator;

    void Start()
    {
        animator = objectWithAnimator.GetComponent<Animator>();
    }

    public void PlayAnimation()
    {
        if (animator.GetBool("open") == true)
        {
            animator.SetBool("open", false);
        }
        else
        {
            animator.SetBool("open", true);
        }
    }

    public void LoadEditorScene()
    {
        SceneManager.LoadScene("LevelMaker");
    }
}