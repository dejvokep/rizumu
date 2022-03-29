using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using static SpawnedController.Rank;

public class EndScreen : MonoBehaviour
{
    public GameObject rankD, rankS, rankA, rankB, rankC;
    public GameObject gamePanel, player;

    public Text scoreText, hitsText, missesText, accuracyText, spText;

    public GameObject panel;
    public Animator animator;

    private CanvasGroup canvasGroup, gamePanelGroup;
    private SpriteRenderer playerRenderer;

    private List<SpawnedController.Rank> RANKS;

    private long score, sp;
    private int hits, misses, accuracy;

    private AudioSource source;
    public AudioMixerGroup mixer;
    public AudioClip ticking, rankShowUp;

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

        // Add audio source
        source = gameObject.AddComponent<AudioSource>();
        // Set mixer
        source.outputAudioMixerGroup = mixer;
        source.loop = true;
        source.clip = ticking;
        //Show(100, 100, 50, 99, 1);
    }

    public void Show(long score, int hits, int misses, int accuracy, long sp) {
        // Deactivate ranks
        foreach (SpawnedController.Rank rank in RANKS)
            rank.rank.SetActive(false);
        // Activate
        panel.SetActive(true);
        // Set
        this.score = score;
        this.hits = hits;
        this.misses = misses;
        this.accuracy = accuracy;
        this.sp = sp;

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
        source.Play();

        for (float t = 0; t < 1; t += Time.deltaTime * speed) {
            // Calculate values
            scoreText.text = "Score: " + ((long) Mathf.Lerp(0, score, t)).ToString();
            hitsText.text = "Hits/misses: " + ((int) Mathf.Lerp(0, hits, t)).ToString() + "/" + ((int) Mathf.Lerp(0, misses, t)).ToString();
            accuracyText.text = "Accuracy: " + ((int) Mathf.Lerp(0, accuracy, t)) + "%";
            spText.text = "Earned: " + ((long) Mathf.Lerp(0, sp, t)).ToString() + " pts";
            yield return true;
        }
        
        // Set final values
        scoreText.text = "Score: " + score.ToString();
        hitsText.text = "Hits/misses: " + hits.ToString() + "/" + misses.ToString();
        accuracyText.text = "Accuracy: " + accuracy + "%";
        spText.text = "Earned: " + sp.ToString() + " pts";
        source.Stop();
        
        // Activate rank
        Invoke("ActivateRank", 0.5F);
    }

    private void ActivateRank() {
        // Index
        int rankIndex = 0;
        // While can move to the upper tier
        while (rankIndex + 1 < RANKS.Count && RANKS[rankIndex + 1].accuracy <= accuracy)
            // Increase
            rankIndex += 1;

        source.PlayOneShot(rankShowUp);
        // Play animation
        RANKS[rankIndex].rank.GetComponent<Animator>().Play("Rank_Popup", -1, 0F);
        // Activate rank
        RANKS[rankIndex].rank.SetActive(true);
    }
}
