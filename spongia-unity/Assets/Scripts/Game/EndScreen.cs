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

    private float ANIMATION_RATE = 5;
    private float alpha = 0, targetAlpha = 0;

    private List<SpawnedController.Rank> RANKS;

    private long displayedScore = 0, displayedSp = 0;
    private int displayedHits = 0, displayedMisses = 0, displayedAccuracy = 0;

    private long score, sp;
    private int hits, misses, accuracy;

    private const float ANIMATION_DURATION = 1;

    private bool shown = false, animated = false;


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

    // Update is called once per frame
    void Update()
    {
        // If not shown
        if (!shown)
            return;

        // If animating
        if (alpha != targetAlpha) {
            // Animate
            animate();
            // Reset alpha
            canvasGroup.alpha = alpha;
            gamePanelGroup.alpha = 1-alpha;
            playerRenderer.color = new Color32(255, 255, 255, (byte) (255*(1-alpha)));
        } else {
            if (!animated) {
                animated = true;
                StartCoroutine(Animate());
            }
        }
    }

    public void Show(long score, int hits, int misses, int accuracy, long sp) {
        // Activate
        panel.SetActive(true);
        // Animate
        targetAlpha = 1;
        // Set
        this.shown = true;
        this.score = score;
        this.hits = hits;
        this.misses = misses;
        this.accuracy = accuracy;
        this.sp = sp;
        Debug.Log("ACC: " + accuracy);

        // Refresh rank
        RefreshRank(accuracy);
    }

    private IEnumerator Animate() {
        float speed = 1.0f / ANIMATION_DURATION;

        for (float t = 0; t < 1; t += Time.deltaTime * speed) {
            scoreText.text = ((long) Mathf.Lerp(0, score, t)).ToString();
            hitsText.text = ((int) Mathf.Lerp(0, hits, t)).ToString();
            missesText.text = ((int) Mathf.Lerp(0, misses, t)).ToString();
            accuracyText.text = ((int) Mathf.Lerp(0, accuracy, t)) + "%";
            spText.text = ((long) Mathf.Lerp(0, sp, t)).ToString();
            yield return true;
        }
        
        scoreText.text = score.ToString();
        hitsText.text = hits.ToString();
        missesText.text = misses.ToString();
        accuracyText.text = accuracy + "%";
        spText.text = sp.ToString();
    }

    private void RefreshRank(float accuracy) {
        // Index
        int rankIndex = 0;
        // While can move to the upper tier
        while (rankIndex + 1 < RANKS.Count && RANKS[rankIndex + 1].accuracy <= accuracy)
            // Increase
            rankIndex += 1;

        // Activate rank
        RANKS[rankIndex].rank.SetActive(true);
    }

    private void animate() {
        float diff = Time.deltaTime * ANIMATION_RATE;
        alpha = alpha < targetAlpha ?
        alpha + diff > targetAlpha ? targetAlpha : alpha + diff :
        alpha - diff < targetAlpha ? targetAlpha : alpha - diff;
    }
}
