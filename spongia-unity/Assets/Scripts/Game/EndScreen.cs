using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SpawnedController.Rank;

public class EndScreen : MonoBehaviour
{
    public GameObject rankD, rankS, rankA, rankB, rankC;
    public GameObject gamePanel, player;

    public Text scoreText, hitsText, missesText, accuracyText, spText;

    public GameObject panel;
    private CanvasGroup canvasGroup, gamePanelGroup;
    private SpriteRenderer playerRenderer;

    private List<SpawnedController.Rank> RANKS;

    private long score, sp;
    private int hits, misses, accuracy;

    private const float FADE_DURATION = 0.5f, ANIMATION_DURATION = 1;

    // Start is called before the first frame update
    void Start()
    {
        RANKS = new List<SpawnedController.Rank>{
            new SpawnedController.Rank(0, rankD),
            new SpawnedController.Rank(50, rankC),
            new SpawnedController.Rank(65, rankB),
            new SpawnedController.Rank(85, rankA),
            new SpawnedController.Rank(95, rankS)
        };

        canvasGroup = panel.GetComponent<CanvasGroup>();
        gamePanelGroup = gamePanel.GetComponent<CanvasGroup>();
        playerRenderer = player.GetComponent<SpriteRenderer>();
    }

    public void Show(long score, int hits, int misses, int accuracy, long sp) {
        // Activate
        panel.SetActive(true);
        // Set
        this.score = score;
        this.hits = hits;
        this.misses = misses;
        this.accuracy = accuracy;
        this.sp = sp;

        // Activate rank
        ActivateRank(accuracy);
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

        // If fading in
        if (fadeIn)
            // Animate numbers
            StartCoroutine(Animate());
        else
            // Deactivate
            panel.SetActive(false);
    }

    private IEnumerator Animate() {
        // Speed
        float speed = 1.0f / ANIMATION_DURATION;

        for (float t = 0; t < 1; t += Time.deltaTime * speed) {
            // Calculate values
            scoreText.text = ((long) Mathf.Lerp(0, score, t)).ToString();
            hitsText.text = ((int) Mathf.Lerp(0, hits, t)).ToString();
            missesText.text = ((int) Mathf.Lerp(0, misses, t)).ToString();
            accuracyText.text = ((int) Mathf.Lerp(0, accuracy, t)) + "%";
            spText.text = ((long) Mathf.Lerp(0, sp, t)).ToString();
            yield return true;
        }
        
        // Set final values
        scoreText.text = score.ToString();
        hitsText.text = hits.ToString();
        missesText.text = misses.ToString();
        accuracyText.text = accuracy + "%";
        spText.text = sp.ToString();
    }

    private void ActivateRank(float accuracy) {
        // Index
        int rankIndex = 0;
        // While can move to the upper tier
        while (rankIndex + 1 < RANKS.Count && RANKS[rankIndex + 1].accuracy <= accuracy)
            // Increase
            rankIndex += 1;

        // Activate rank
        RANKS[rankIndex].rank.SetActive(true);
    }
}
