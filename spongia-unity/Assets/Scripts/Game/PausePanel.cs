using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class PausePanel : MonoBehaviour
{

    private SpawnedController controller;
    private CanvasGroup canvasGroup, gamePanelGroup, targetGroup;
    private SpriteRenderer playerRenderer;

    private float ANIMATION_RATE = 5;

    private float alpha = 0, targetAlpha = 0;
    private bool pendingCountdown = false;
    private const int resumeCountdownLength = 3;
    private float resumeCountdown = 0;

    public GameObject gamePanel, player, resumeCountdownObject, panel;
    private Text resumeCountdownText;

    private bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("SpawnedController").GetComponent<SpawnedController>();
        canvasGroup = panel.GetComponent<CanvasGroup>();
        gamePanelGroup = gamePanel.GetComponent<CanvasGroup>();
        playerRenderer = player.GetComponent<SpriteRenderer>();
        resumeCountdownText = resumeCountdownObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the countdown is pending
        if (pendingCountdown) {
            // Decrease
            resumeCountdown -= Time.deltaTime;
            // If below or 0
            if (resumeCountdown <= 0) {
                // Disable
                resumeCountdownObject.SetActive(false);
                pendingCountdown = false;
                // Resume
                controller.Resume();
            } else {
                // Set the text
                resumeCountdownText.text = "Resuming in " + ((int) Math.Ceiling(resumeCountdown)) + "...";
            }
        }

        // If animating
        if (alpha != targetAlpha) {
            // Animate
            animate();
            // Reset alpha
            canvasGroup.alpha = alpha;
            gamePanelGroup.alpha = 1-alpha;
            playerRenderer.color = new Color32(255, 255, 255, (byte) (255*(1-alpha)));
            // If reached the target
            if (alpha == targetAlpha) {
                // If dissappeared
                if (targetAlpha == 0)
                    // Deactivate
                    panel.SetActive(false);
            }
        }

        // If escape is down
        if (Input.GetKeyDown("escape")) {
            // If was paused
            if (paused) {
                Resume();
            } else {
                Pause();
            }
        }
    }

    public void Hide() {
        targetAlpha = 0;
        paused = false;
    }

    public void Pause() {
        // If paused
        if (paused)
            return;
        // Paused
        paused = true;
        // Open the menu
        panel.SetActive(true);
        // Pause
        controller.Pause();
        // Play sound
        SFXPlayer.Play(SFXPlayer.EffectType.CLICK_CONTINUE);
        // Animate
        targetAlpha = 1;
        // Set
        pendingCountdown = false;
        // Disable
        resumeCountdownObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Resume() {
        // If not paused
        if (!paused)
            return;
        // Resumed
        paused = false;
        // Play sound
        SFXPlayer.Play(SFXPlayer.EffectType.CLICK_CONTINUE);
        // Animate
        targetAlpha = 0;
        // Set awaiting
        pendingCountdown = true;
        // Reset cooldown
        resumeCountdown = (float) resumeCountdownLength;
        // Enable
        resumeCountdownObject.SetActive(true);
        // Set the text
        resumeCountdownText.text = "Resuming in " + ((int) resumeCountdown) + "...";
    }

    private void animate() {
        float diff = Time.deltaTime * ANIMATION_RATE;
        alpha = alpha < targetAlpha ?
        alpha + diff > targetAlpha ? targetAlpha : alpha + diff :
        alpha - diff < targetAlpha ? targetAlpha : alpha - diff;
    }
}
