using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButtonsController : MonoBehaviour
{
    public GameObject objectWithAnimator;
    private Animator animator;

    void Start()
    {
        animator = objectWithAnimator.GetComponent<Animator>();
    }
    void Update()
    {
        if (animator.GetBool("open") == true && Input.GetKeyDown(KeyCode.Escape))
        {
            animator.SetBool("open", false);
        }
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
}
