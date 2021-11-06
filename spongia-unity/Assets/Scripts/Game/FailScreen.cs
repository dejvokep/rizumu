using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailScreen : MonoBehaviour
{

    public GameObject gamePanel, player, panel;
    
    private CanvasGroup canvasGroup, gamePanelGroup;
    private SpriteRenderer playerRenderer;

    private const float FADE_DURATION = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = panel.GetComponent<CanvasGroup>();
        gamePanelGroup = gamePanel.GetComponent<CanvasGroup>();
        playerRenderer = player.GetComponent<SpriteRenderer>();
    }

    public void Show() {
        // Activate
        panel.SetActive(true);
        // Animate
        StartCoroutine(Fade(true));
    }

    public void Hide() {
        // Animate
        StartCoroutine(Fade(false));
    }

    private IEnumerator Fade(bool fadeIn) {
        // Speed
        float speed = 1f / FADE_DURATION;

        for (float t = 0; t < 1; t += Time.deltaTime * speed) {
            // Current alpha
            float alpha = Mathf.Lerp(fadeIn ? 0 : 1, fadeIn ? 1 : 0, t);
            // Reset alpha
            canvasGroup.alpha = alpha;
            gamePanelGroup.alpha = 1-alpha;
            playerRenderer.color = new Color32(255, 255, 255, (byte) (255*(1-alpha)));
            yield return true;
        }

        // Deactivate
        if (!fadeIn)
            panel.SetActive(false);
    }
}
