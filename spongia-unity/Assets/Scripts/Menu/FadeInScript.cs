using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInScript : MonoBehaviour
{
    private RawImage windowTransitionerImage;
    public float progress = 2f;
    public float speed = 1f;
    private bool isFading = true;

    void Start()
    {
        windowTransitionerImage = transform.GetComponent<RawImage>();
        
        windowTransitionerImage.color = new Color32((byte)0, (byte)0, (byte)0, (byte)255);

        windowTransitionerImage.raycastTarget = true;
    }

    void Update()
    {
        if (isFading)
        {
            progress -= Time.deltaTime*speed;

            if (progress > 1f)
                return;

            float alpha;
            if (progress <= 0f)
            {
                alpha = 0;
                isFading = false;
                windowTransitionerImage.raycastTarget = false;
            }
            else
                alpha = 255*progress;
            
            
            if (progress <= 0.5f && windowTransitionerImage.raycastTarget)
                windowTransitionerImage.raycastTarget = false;
            
            windowTransitionerImage.color = new Color32((byte)0, (byte)0, (byte)0, (byte)alpha);
        }
    }
}
