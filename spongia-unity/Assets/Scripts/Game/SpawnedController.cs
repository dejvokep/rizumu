using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using static Sector;
using Newtonsoft.Json;

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

    public static string songID; //"cf954cc6-aba1-47bd-a855-6f77c00006ad"
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

    public ParticleSystem particlesNE, particlesSE, particlesSW, particlesNW;
    public Dictionary<Sector, ParticleSystem> particles = new Dictionary<Sector, ParticleSystem>();

    public Text scoreText, multiplierText, comboText, accuracyText;
    public Image accuracyIndicator;
    public Image progressBar, hpBar, hpBarFill;

    // Prefab to spawn
    public GameObject prefab;
    // Player
    public GameObject player;
    public GameObject scoreEarnedPrefab, canvas, targets;

    public GameObject rankD, rankS, rankA, rankB, rankC;

    private EndScreen endScreen;
    private FailScreen failScreen;

    public GameObject gameBackground, pauseBackground, gamePanel;

    // Hlavna classa, kde by mal byt cely logic co sa tyka spawnutych kruzkov, whatever.
    // To znamena, metoda ked sa nejaky klikne, ked treba nejaky spawnut, etc.

    // Spawn offsets
    public double xOffset, yOffset;
    private double height, width;

    // Currently active (spawned) props
    public Dictionary<Sector, SectorData> sectors;
    // Keyboard keys by sector
    public static Dictionary<Sector, string> keyboardKeys;

    public float currentTime;
    public float playerWidth;

    private long score = 0;

    public float songLength;
    public float multiplier = MULTIPLIERS[0].multiplier;
    private int combo = 0;

    private long maxScore = 0;

    private const long LONG_ANIMATION_RATE = 3000;
    private const float FLOAT_ANIMATION_RATE = 1;

    private long displayedScore = 0;
    private float displayedMultiplier = MULTIPLIERS[0].multiplier;
    private float displayedAccuracy = 1;
    private float displayedHP = 1;

    private int hp = START_HP;

    public bool startedPlaying = false, finishedPlaying = false, failedPlaying = false, paused = false;

    private PausePanel pausePanel;

    public static int FAIL_HP = 30;
    private int rankIndex;

    public static int START_HP = 100, MISS_HP = -15, CORRECT_HP = 25;

    private int misses = 0;
    private int bestCombo = 0;
    private bool initialized = false;

    public string songName, songAuthor;
    public int songDifficulty;

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
        particles.Add(Sector.NORTH_EAST, particlesNE);
        particles.Add(Sector.SOUTH_EAST, particlesSE);
        particles.Add(Sector.SOUTH_WEST, particlesSW);
        particles.Add(Sector.NORTH_WEST, particlesNW);

        rankIndex = RANKS.Count - 1;

        playerWidth = GameObject.Find("Player").GetComponent<Renderer>().bounds.size.x / 4;
        endScreen = GetComponent<EndScreen>();
        failScreen = GetComponent<FailScreen>();
        pausePanel = GetComponent<PausePanel>();
        // Create new data
        sectors = new Dictionary<Sector, SectorData>();
        // Iterate
        foreach (Sector sector in Enum.GetValues(typeof(Sector)))
            // Add data
            sectors.Add(sector, new SectorData(this, sector));

        //
        // CONTROLS
        //
        // keyboardKeys = new Dictionary<Sector, string>();
        // keyboardKeys.Add(Sector.NORTH_WEST, "a");
        // keyboardKeys.Add(Sector.NORTH_EAST, "s");
        // keyboardKeys.Add(Sector.SOUTH_EAST, "d");
        // keyboardKeys.Add(Sector.SOUTH_WEST, "f");

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
        musicHandler.Load();

        string songDataString;
        // If bundled
        if (musicHandler.bundled) {
            songDataString = Resources.Load<TextAsset>("maps/" + songID + "/info").ToString();
        } else {
            // Create reader
            StreamReader reader = new StreamReader(Path.Combine(Application.persistentDataPath, "maps/" + SpawnedController.songID + "/data.json"));
            // Read
            songDataString = reader.ReadToEnd();
            // Close
            reader.Close();
        }

        // Load
        Dictionary<string, object> songData = JsonConvert.DeserializeObject<Dictionary<string, object>>(songDataString);
        songName = songData["song_name"].ToString();
        songAuthor = songData["song_author"].ToString();
        songDifficulty = Int32.Parse(songData["difficulty"].ToString());
    }

    public void ContinueInit() {
        // Set clip
        audioSource.clip = musicHandler.audioClip;
        songLength = audioSource.clip.length;
        // Set current time
        currentTime = (int) (musicHandler.firstSpawn - 2);

        // Aspect ratio
        float aspectRatio = musicHandler.image.rect.width / musicHandler.image.rect.height;
        // Set backgrounds
        gameBackground.GetComponent<Image>().sprite = musicHandler.image;
        gameBackground.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
        pauseBackground.GetComponent<Image>().sprite = musicHandler.image;
        pauseBackground.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;

        // If lower than 0
        if (currentTime < 0)
            // Start playing when current time is 0
            Invoke("PlayMusic", Math.Abs(currentTime));
        else
            // Play now
            PlayMusic();

        // Initialized
        initialized = true;
    }

    private void ResetParticles()
    {
        foreach (Sector sector in Enum.GetValues(typeof(Sector)))
            // Add data
            particles[sector].Stop();
    }

    public void Restart() {
        ResetParticles();
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
        displayedAccuracy = 1;
        displayedHP = 1;
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
        currentTime = (int) (musicHandler.firstSpawn - 2);
        // Reset progress
        progressBar.fillAmount = 0;
        // Reset HP
        hpBar.fillAmount = 1;

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
        ResetParticles();
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

    // public void Quit() {
    //     // Load other scene
    //     SceneManager.LoadScene("MenuScene");
    // }

    private void ShowEndScreen() {
        ResetParticles();
        endScreen.Show(score, musicHandler.PropCount() * 2 - misses, misses, (int) ((float) score / maxScore * 100), (long) Math.Sqrt(score));
    }

    private void ShowFailScreen() {
        ResetParticles();
        failScreen.Show();
    }

    void PlayMusic() {
        audioSource.Stop();
        //Invoke("ChangeTime", 10);
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

    public void Spawn(Sector sector, float length, float startTime, float diagonalSpeed) {
        // Sector ID
        int sectorID = (int) sector;
        
        // Full size of the prop (1/2)
        float xSize = (float) (length / 2 * diagonalSpeed) + ((float) (prefab.transform.localScale.x / 2 / Prop.SQRT_OF_TWO));
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
        // Set diagonal speed
        prop.SetSpeed(diagonalSpeed);
        // Init
        prop.Init();
        // Spawn
        sectors[sector].Spawn(prop);

        // ADD ONCLICK TRIGGER (USE BUTTONS?)

        // Add
        //active[sector].Add(spawned.GetComponent<Prop>());
    }

    private void Save() {
        // File path
        string filePath = Path.Combine(Application.persistentDataPath, "userdata.json");
        
        // Json base
        JSONBase jsonBase;
        // If exists
        if (File.Exists(filePath)) {
            // Create reader
            StreamReader reader = new StreamReader(filePath);
            // Deserialize
            jsonBase = JsonConvert.DeserializeObject<JSONBase>(reader.ReadToEnd());
            // Close
            reader.Close();
            print("exists");
        } else {
            print("creating");
            // Create default
            jsonBase = new JSONBase {
                sp = 0,
                highscores = new Dictionary<string, JSONHighscore>()
            };
        }

        // Accurracy
        float accuracy = (float) score / maxScore * 100;
        // Add sp
        jsonBase.sp += (long) Math.Sqrt(this.score);
        // New score
        JSONHighscore highscore = new JSONHighscore {
            score = this.score,
            accuracy = (float) this.score / this.maxScore * 100,
            rank = rankIndex == 0 ? "D" : rankIndex == 1 ? "C" : rankIndex == 2 ? "B" : rankIndex == 3 ? "A" : "S",
            combo = this.bestCombo
        };

        if (jsonBase.highscores == null) {
            jsonBase.highscores = new Dictionary<string, JSONHighscore>();
        }

        // If does not contain
        if (!jsonBase.highscores.ContainsKey(songID))
            // Add
            jsonBase.highscores.Add(songID, highscore);
        else if (jsonBase.highscores[songID].accuracy < accuracy)
            // Replace
            jsonBase.highscores[songID] = highscore;
        
        // Serialize
        File.WriteAllText(filePath, JsonConvert.SerializeObject(jsonBase, Formatting.Indented));
    }

    public class JSONBase {
        public long sp {get; set;}
        public Dictionary<string, JSONHighscore> highscores {get; set;}
    }

    public class JSONHighscore {
        public long score {get; set;}
        public float accuracy {get; set;}
        public string rank {get; set;}
        public int combo {get; set;}
    }

    // Update is called once per frame
    void Update()
    {
        // If not initialized
        if (!initialized)
            return;
            
        // Animate HP bar
        displayedHP = animateHp(displayedHP, (float) hp/START_HP);
        hpBar.fillAmount = displayedHP;

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
            // Save
            Save();

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
            if (Input.GetKeyDown(keyboardKeys[sector])) {
                // Score
                int score = sectors[sector].HandlePress(currentTime);
                // If not -2
                if (score != -2)
                    // Turn on particle system
                    particles[sector].Play();

                // Handle
                HandleScore(sector, score, true);
            } else if (Input.GetKeyUp(keyboardKeys[sector])) {
                // Score
                int score = sectors[sector].HandleRelease(currentTime);
                // If not -2
                if (score != -2)
                    // Turn off particle system
                    particles[sector].Stop();

                // Handle
                HandleScore(sector, score, false);
            }

            // Update
            sectors[sector].Update(currentTime);
        }

        // Animate
        displayedScore = animateLong(displayedScore, score);
        displayedAccuracy = (maxScore > 0 ? animateFloat(displayedAccuracy, (float) score / maxScore) : displayedAccuracy);
        displayedMultiplier = animateFloat(displayedMultiplier, multiplier);
        displayedHP = animateHp(displayedHP, (float) hp/START_HP);

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
        hpBar.fillAmount = displayedHP;
        //hpBarFill.color = displayedHP < ((float) hp/START_HP) ? new Color32(0, 255, 63, 255) : displayedHP > ((float) hp/START_HP) ? new Color32(255, 0, 0, 255) : new Color32(255, 255, 255, 255);

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

    public void HandleScore(Sector sector, int score, bool press) {
        // If -1, reset
        if (score == -1) {
            // If the best combo
            if (bestCombo < combo)
                bestCombo = combo;
            
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
                gameObject.transform.SetParent(gamePanel.transform, false);
                // Set position
                gameObject.transform.position = new Vector2(playerWidth * (1 + UnityEngine.Random.Range(-0.4f, 0.4f)) * -SectorXDirection(sector), playerWidth * (4 + UnityEngine.Random.Range(-0.2f, 0.4f)) * -SectorYDirection(sector));
                // Set text
                Text text = gameObject.GetComponent<Text>();
                text.text = (score * multiplier * DIFFICULTY_MULTIPLIER).ToString();
                // If greater than 0
                if (score > 0) {
                    // Play SFX
                    if (press) {
                        SFXPlayer.Play(SFXPlayer.EffectType.DRUM_HIT);
                    }
                    // Set text color
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

    private float animateHp(float displayed, float target) {
        float diff = Time.deltaTime * 1f;
        //Debug.Log(diff + ": " + displayed + " " + target);
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