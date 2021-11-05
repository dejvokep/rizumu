using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System;
using static Sector;

public class SpawnedController : MonoBehaviour
{

    private class Multiplier {
        public int combo;
        public float multiplier;

        public Multiplier(int combo, float multiplier) {
            this.combo = combo;
            this.multiplier = multiplier;
        }
    }

    public class Rank {
        public float accuracy;
        public GameObject rank;

        public Rank(float accuracy, GameObject rank) {
            this.accuracy = accuracy;
            this.rank = rank;
        }
    }

    public static string songName = "tajne_zaznamy";
    // Move speed
    public static float MOVE_SPEED = 2;
    public static float DIAGONAL_MOVE_SPEED = MOVE_SPEED * (float) Prop.SQRT_OF_TWO;
    public static float DIFFICULTY_MULTIPLIER = 1;

    private Color32 SCORE_COLOR_GOOD = new Color32(0, 255, 0, 255);
    private Color32 SCORE_COLOR_AVERAGE = new Color32(0, 255, 255, 255);
    private Color32 SCORE_COLOR_BAD = new Color32(255, 255, 0, 255);

    private static List<Multiplier> MULTIPLIERS = new List<Multiplier>{
        new Multiplier(0, 1.0f),
        new Multiplier(10, 1.1f),
        new Multiplier(30, 1.2f),
        new Multiplier(60, 1.3f),
        new Multiplier(100, 1.4f),
        new Multiplier(150, 1.5f),
        new Multiplier(200, 1.7f),
        new Multiplier(250, 1.9f),
        new Multiplier(300, 2f),
        new Multiplier(400, 2.5f),
        new Multiplier(500, 3f)
    };

    private List<Rank> RANKS;

    private const string MULTIPLIER_FORMAT = "0.0";

    private int multiplierIndex = 0;

    // Music handler
    private MusicHandler musicHandler;
    // Mixer
    public AudioMixerGroup mixer;
    // Audio source
    public AudioSource audioSource;

    public Text scoreText, multiplierText, comboText, accuracyText;
    public Image accuracyIndicator;
    public Image progressBar, hpBar;

    // Prefab to spawn
    public GameObject prefab;
    // Player
    public GameObject player;
    public GameObject scoreEarnedPrefab, canvas, targets;

    public GameObject rankD, rankS, rankA, rankB, rankC;

    private EndScreen endScreen;
    private FailScreen failScreen;

    // Hlavna classa, kde by mal byt cely logic co sa tyka spawnutych kruzkov, whatever.
    // To znamena, metoda ked sa nejaky klikne, ked treba nejaky spawnut, etc.

    // Spawn offsets
    public double xOffset, yOffset;
    private double height, width;

    // Currently active (spawned) props
    private Dictionary<Sector, SectorData> sectors;
    // Keyboard keys by sector
    private Dictionary<Sector, string> keyboardKeys;

    private float currentTime;
    public float playerWidth;

    private long score = 0;

    private float songLength;
    private float multiplier = MULTIPLIERS[0].multiplier;
    private int combo = 0;

    private long maxScore = 0;

    private const long LONG_ANIMATION_RATE = 3000;
    private const float FLOAT_ANIMATION_RATE = 1;

    private long displayedScore = 0;
    private float displayedMultiplier = MULTIPLIERS[0].multiplier;
    private float displayedAccuracy = 0;
    private int displayedHP = 100;

    private int hp = START_HP;

    public bool startedPlaying = false, finishedPlaying = false, failedPlaying = false, paused = false;

    private PausePanel pausePanel;

    public static int FAIL_HP = 30;
    private int rankIndex;

    public static int START_HP = 100, MISS_HP = -30, CORRECT_HP = 20;

    private int misses = 0;



    // Start is called before the first frame update
    void Start()
    {
        RANKS = new List<Rank>{
            new Rank(0, rankD),
            new Rank(50, rankC),
            new Rank(65, rankB),
            new Rank(85, rankA),
            new Rank(95, rankS)
        };
        rankIndex = RANKS.Count - 1;

        playerWidth = GameObject.Find("Player").GetComponent<Renderer>().bounds.size.x / 4;
        endScreen = GetComponent<EndScreen>();
        failScreen = GetComponent<FailScreen>();
        // Create new data
        sectors = new Dictionary<Sector, SectorData>();
        // Iterate
        foreach (Sector sector in Enum.GetValues(typeof(Sector)))
            // Add data
            sectors.Add(sector, new SectorData(this));

        //
        // CONTROLS
        //
        keyboardKeys = new Dictionary<Sector, string>();
        keyboardKeys.Add(Sector.NORTH_WEST, "a");
        keyboardKeys.Add(Sector.NORTH_EAST, "s");
        keyboardKeys.Add(Sector.SOUTH_EAST, "d");
        keyboardKeys.Add(Sector.SOUTH_WEST, "f");

        //
        // OFFSETS
        //
        // Screen dimensions (/2)
        Camera cam = Camera.main;
        height = 1f * cam.orthographicSize;
        width = height * cam.aspect;
        // Convert 45 deg to radians
        double diagonalRadians = Math.PI * 45 / 180.0d;
        // Calculate x offset (horizontal)              a = tg(45) *  b
        xOffset = (Math.Sin(diagonalRadians)/Math.Cos(diagonalRadians)) * height;
        // Calculate y offset (vertical)
        yOffset = height;

        //
        // AUDIO
        //
        // Audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        // Set mixer
        audioSource.outputAudioMixerGroup = mixer;
        // Load handler
        musicHandler = new MusicHandler(this);
        // Set clip
        audioSource.clip = musicHandler.audioClip;
        songLength = audioSource.clip.length;
        // Set current time
        currentTime = (int) (musicHandler.firstSpawn - 1);

        // If lower than 0
        if (currentTime < 0)
            // Start playing when current time is 0
            Invoke("PlayMusic", Math.Abs(currentTime));
        else
            // Play now
            PlayMusic();
    }

    public void Restart() {
        // Stop
        audioSource.Stop();
        musicHandler.Reset();
        audioSource.time = 0;
        // Reset to defaults
        score = 0;
        multiplier = MULTIPLIERS[0].multiplier;
        combo = 0;
        maxScore = 0;
        displayedScore = 0;
        displayedMultiplier = MULTIPLIERS[0].multiplier;
        displayedAccuracy = 0;
        displayedHP = 100;
        hp = START_HP;
        startedPlaying = false;
        finishedPlaying = false;
        failedPlaying = false;
        paused = false;
        rankIndex = RANKS.Count - 1;
        misses = 0;

        // Reset ranks
        for (int index = 0; index < RANKS.Count; index++)
            RANKS[index].rank.SetActive(index == rankIndex);

        // Iterate
        foreach (Sector sector in Enum.GetValues(typeof(Sector)))
            // Reset
            sectors[sector].Reset();
        // Set current time
        currentTime = (int) (musicHandler.firstSpawn - 1);
        // Reset progress
        progressBar.fillAmount = 0;

        // Hide all panels
        endScreen.Hide();
        failScreen.Hide();
        pausePanel.Hide();

        // If lower than 0
        if (currentTime < 0)
            // Start playing when current time is 0
            Invoke("PlayMusic", Math.Abs(currentTime));
        else
            // Play now
            PlayMusic();
    }

    public void Pause() {
        // Paused
        paused = true;
        // Cancel invoke
        CancelInvoke();
        // Pause the music
        audioSource.Pause();
    }

    public void Resume() {
        // Not paused
        paused = false;
        // If less than 0
        if (currentTime < 0)
            // Start playing when current time is 0
            Invoke("ResumeMusic", Math.Abs(currentTime));
        else
            // Play the music
            audioSource.Play();
    }

    public void Quit() {
        // Load other scene
        SceneManager.LoadScene("MenuScene");
    }

    private void ShowEndScreen() {
        endScreen.Show(score, musicHandler.PropCount() * 2 - misses, misses, (int) ((float) score / maxScore * 100), (long) Math.Sqrt(score));
    }

    private void ShowFailScreen() {
        failScreen.Show();
    }

    void PlayMusic() {
        Debug.Log("Playing...");
        audioSource.Stop();
        Invoke("ChangeTime", 2);
        audioSource.Play();
        startedPlaying = true;
    }

    void ChangeTime() {
        audioSource.time = 175;
    }

    void ResumeMusic() {
        //Debug.Log("Started playing at: " + currentTime);
        audioSource.Play();
    }

    public void Spawn(Sector sector, float length, float startTime) {
        // Sector ID
        int sectorID = (int) sector;
        
        // Full size of the prop (1/2)
        float xSize = (float) (length / 2 * DIAGONAL_MOVE_SPEED) + ((float) (prefab.transform.localScale.x / 2 / Prop.SQRT_OF_TWO));
        // Positions (NOTE : ((float) offset + (length / 2 / Prop.SQRT_OF_TWO)))
        double x = xOffset + xSize, y = yOffset + xSize;

        if (sectorID >= 2)
            x = -x;
        if (sectorID == 1 || sectorID == 2)
            y = -y;


        // Instantiate
        GameObject spawned = (GameObject) Instantiate(prefab, new Vector3((float) x, (float) y), Quaternion.Euler(new Vector3(0, 0, -45 + 90*sectorID)));
        // Change size
        spawned.transform.localScale = new Vector2(1, length);
        // Set sorting order in layer
        spawned.GetComponent<SpriteRenderer>().sortingOrder = sectorID+1;
        
        // Prop component
        Prop prop = spawned.GetComponent<Prop>();
        // Set start time
        prop.SetStartTime(startTime);
        // Spawn
        sectors[sector].Spawn(prop);

        // ADD ONCLICK TRIGGER (USE BUTTONS?)

        // Add
        //active[sector].Add(spawned.GetComponent<Prop>());
    }

    public void Clicked(GameObject prop) {
        Debug.Log(prop);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currentTime);
        // If finished or failed
        if (finishedPlaying || failedPlaying)
            return;

        // If HP lower than required
        if (hp < FAIL_HP) {
            // Failed
            failedPlaying = true;
            // Cancel invoke
            CancelInvoke();
            // Pause the music
            audioSource.Pause();

            // Iterate
            foreach (Sector sector in Enum.GetValues(typeof(Sector)))
                // Add data
                sectors[sector].Failed();
            
            // Show fail screen after 1 second
            Invoke("ShowFailScreen", 1);
            return;
        }

        // If paused
        if (paused)
            return;

        // If finished playing
        if (!audioSource.isPlaying && audioSource.time == 0 && startedPlaying) {
            // Finished
            finishedPlaying = true;
            // Update the bar
            progressBar.fillAmount = 1;

            // Show end screen after 1 second
            Invoke("ShowEndScreen", 1);
        }

        // Update current time
        currentTime = audioSource.isPlaying ? audioSource.time : currentTime + Time.deltaTime;

        // If is playing
        if (audioSource.isPlaying)
            // Update the bar
            progressBar.fillAmount = currentTime / songLength;


        //Debug.Log(currentTime);

        // For each sector
        foreach (Sector sector in Enum.GetValues(typeof(Sector))) {
            // If pressed
            if (Input.GetKeyDown(keyboardKeys[sector]))
                HandleScore(sector, sectors[sector].HandlePress(currentTime));
            else if (Input.GetKeyUp(keyboardKeys[sector]))
                HandleScore(sector, sectors[sector].HandleRelease(currentTime));

            // Update
            sectors[sector].Update(currentTime);
        }

        // Animate
        displayedScore = animateLong(displayedScore, score);
        displayedAccuracy = (maxScore > 0 ? animateFloat(displayedAccuracy, (float) score / maxScore) : 1);
        displayedMultiplier = animateFloat(displayedMultiplier, multiplier);
        displayedHP = animateHp(displayedHP, hp);

        // Score
        scoreText.text = displayedScore.ToString();
        // Multiplier
        multiplierText.text = "× " + displayedMultiplier.ToString(MULTIPLIER_FORMAT);
        // Combo
        comboText.text = "× " + combo;
        // Accuracy
        accuracyText.text = (int) (displayedAccuracy*100) + "%";
        accuracyIndicator.fillAmount = displayedAccuracy;
        RefreshRank(maxScore > 0 ? (float) score / maxScore * 100 : 100);
        // HP
        hpBar.fillAmount = (float) displayedHP / START_HP;

        // Spawn
        musicHandler.SpawnNext(currentTime);
    }

    private void RefreshRank(float accuracy) {
        // Deactivate current rank
        RANKS[rankIndex].rank.SetActive(false);

        // Index
        rankIndex = 0;
        // While can move to the upper tier
        while (rankIndex + 1 < RANKS.Count && RANKS[rankIndex + 1].accuracy <= accuracy)
            // Increase
            rankIndex += 1;

        // Activate rank
        RANKS[rankIndex].rank.SetActive(true);
    }

    public void HandleScore(Sector sector, int score) {
        // If -1, reset
        if (score == -1) {
            // Reset combo and multiplier
            combo = 0;
            multiplierIndex = 0;
            // Count miss
            misses += 1;
            // Decrease HP
            hp = hp + MISS_HP >= 0 ? hp + MISS_HP : 0;

        } else {
            // If not negative
            if (score >= 0) {
                // Add score
                this.score += (long) (score * multiplier * DIFFICULTY_MULTIPLIER);
                // Add combo
                this.combo += 1;

                // Spawn text
                GameObject gameObject = Instantiate(scoreEarnedPrefab, new Vector2(), Quaternion.identity);
                // Set parent canvas
                gameObject.transform.SetParent(canvas.transform, false);
                // Set position
                gameObject.transform.position = new Vector2(playerWidth * (1 + UnityEngine.Random.Range(-0.4f, 0.4f)) * -SectorXDirection(sector), playerWidth * (4 + UnityEngine.Random.Range(-0.2f, 0.4f)) * -SectorYDirection(sector));
                // Set text
                Text text = gameObject.GetComponent<Text>();
                text.text = (score * multiplier * DIFFICULTY_MULTIPLIER).ToString();
                // Set color
                if (score > 0) {
                    text.color = score == 300 ? SCORE_COLOR_GOOD : score == 200 ? SCORE_COLOR_AVERAGE : SCORE_COLOR_BAD;
                    // Increase HP
                    hp = hp + CORRECT_HP <= START_HP ? hp + CORRECT_HP : START_HP;
                }
            }
        }

        // While can move to the upper tier
        while (MULTIPLIERS[multiplierIndex + 1].combo <= combo)
            // Increase
            multiplierIndex += 1;

        // Set multiplier
        multiplier = MULTIPLIERS[multiplierIndex].multiplier;
    }

    private long animateLong(long displayed, long target) {
        long diff = (long) (Time.deltaTime * LONG_ANIMATION_RATE);
        return displayed < target ?
        displayed + diff > target ? target : displayed + diff :
        displayed - diff < target ? target : displayed - diff;
    }

    private int animateHp(int displayed, int target) {
        int diff = (int) (Time.deltaTime * 500);
        return displayed < target ?
        displayed + diff > target ? target : displayed + diff :
        displayed - diff < target ? target : displayed - diff;
    }

    public static float animateFloat(float displayed, float target) {
        float diff = Time.deltaTime * FLOAT_ANIMATION_RATE;
        return displayed < target ?
        displayed + diff > target ? target : displayed + diff :
        displayed - diff < target ? target : displayed - diff;
    }

    public void AddToMaxScore(int s) {
        maxScore += s;
    }

    public static float SectorXDirection(Sector sector) {
        return (int) sector < 2 ? -1 : 1;
    }

    public static float SectorYDirection(Sector sector) {
        int sectorID = (int) sector;
        return sectorID == 0 || sectorID == 3 ? -1 : 1;
    }
}
