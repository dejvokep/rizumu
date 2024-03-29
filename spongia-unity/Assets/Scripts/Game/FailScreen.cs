using System.Collections;
using System.Collections.Generic;
using System;
using static Sector;
using UnityEngine;
using UnityEngine.EventSystems;

public class FailScreen : MonoBehaviour
{

    // Constants
    private const float FADE_DURATION = 0.5f;

    // UI elements
    public GameObject gamePanel, player, panel;

    // Internals
    private CanvasGroup canvasGroup, gamePanelGroup;
    private SpriteRenderer playerRenderer;
    private SpawnedController controller;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = panel.GetComponent<CanvasGroup>();
        gamePanelGroup = gamePanel.GetComponent<CanvasGroup>();
        playerRenderer = player.GetComponent<SpriteRenderer>();
        controller = gameObject.GetComponent<SpawnedController>();
    }

    // Shows the panel
    public void Show() {
        // Activate
        panel.SetActive(true);
        // Animate
        StartCoroutine(Fade(true));
    }

    // Hides the panel
    public void Hide() {
        // Animate
        StartCoroutine(Fade(false));
        EventSystem.current.SetSelectedGameObject(null);
    }
    
    // Fades the panel (and other game components)
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
            foreach (Sector sector in Enum.GetValues(typeof(Sector)))
                controller.sectors[sector].Fade(1-alpha);
            yield return true;
        }

        // Deactivate
        if (!fadeIn)
            panel.SetActive(false);
    }
}
