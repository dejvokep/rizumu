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
using System.ComponentModel;

public class SpawnedController : MonoBehaviour
{

    // Multiplier
    private class Multiplier {
        public int combo;
        public float multiplier;

        public Multiplier(int combo, float multiplier) {
            this.combo = combo;
            this.multiplier = multiplier;
        }
    }

    // Rank
    public class Rank {
        public float accuracy;
        public GameObject rank;

        public Rank(float accuracy, GameObject rank) {
            this.accuracy = accuracy;
            this.rank = rank;
        }
    }

    // Edited by the menu
    public static string songID;

    // Constants
    public const float DIFFICULTY_MULTIPLIER = 1;
    private static Color32 SCORE_COLOR_GOOD = new Color32(0, 255, 0, 255);
    private static Color32 SCORE_COLOR_AVERAGE = new Color32(0, 255, 255, 255);
    private static Color32 SCORE_COLOR_BAD = new Color32(255, 255, 0, 255);
    private const string MULTIPLIER_FORMAT = "0.0";
    private const long LONG_ANIMATION_RATE = 3000;
    private const float FLOAT_ANIMATION_RATE = 1;
    public const int FAIL_HP = 30, START_HP = 100, MISS_HP = -15, CORRECT_HP = 25;

    // List of multipliers
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

    [Header("Audio")]
    public AudioMixerGroup mixer;
    public AudioSource audioSource;
    private MusicHandler musicHandler;

    [Header("Particles")]
    public ParticleSystem particlesNE;
    public ParticleSystem particlesSE;
    public ParticleSystem particlesSW;
    public ParticleSystem particlesNW;
    public Dictionary<Sector, ParticleSystem> particles = new Dictionary<Sector, ParticleSystem>();

    [Header("UI elements")]
    public Text scoreText;
    public Text multiplierText;
    public Text comboText;
    public Text accuracyText;
    public Image accuracyIndicator;
    public Image progressBar;
    public Image hpBar;
    public Image hpBarFill;

    [Header("Prefabs and scene elements")]
    // Prefab to spawn
    public GameObject prefab;
    // Player
    public GameObject player;
    public GameObject scoreEarnedPrefab;
    public GameObject canvas;
    public GameObject targets;

    [Header("Ranks")]
    public GameObject rankS;
    public GameObject rankA;
    public GameObject rankB;
    public GameObject rankC;
    public GameObject rankD;

    private EndScreen endScreen;
    private FailScreen failScreen;

    [Header("Backgrounds and panels")]
    public GameObject gameBackground;
    public GameObject pauseBackground;
    public GameObject gamePanel;

    [Header("Internals (do not edit)")]
    // Spawn offsets
    public double xOffset;
    public double yOffset;
    private double height, width;

    // Ranks
    private List<Rank> RANKS;

    // Currently active (spawned) props
    public Dictionary<Sector, SectorData> sectors;
    // Keyboard keys by sector (managed by settings menu)
    public static Dictionary<Sector, string> keyboardKeys;

    // Timing and others
    public float currentTime;
    public float playerWidth;
    public float songLength;
    public float noteXSize;

    // Multipliers
    public float multiplier = MULTIPLIERS[0].multiplier;
    private int multiplierIndex = 0;

    // Analytics
    private int combo = 0;
    private long maxScore = 0;
    private long score = 0;
    private int misses = 0;
    private int bestCombo = 0;
    private int hp = START_HP;
    private int rankIndex;

    // Displays
    private long displayedScore = 0;
    private float displayedMultiplier = MULTIPLIERS[0].multiplier;
    private float displayedAccuracy = 1;
    private float displayedHP = 1;

    // Booleans
    public bool startedPlaying = false, finishedPlaying = false, failedPlaying = false, paused = false, initialized = false;

    // Song data
    public string songName, songAuthor;
    public int songDifficulty;

    // Other
    private Animator playerAnimator;
    private Sprite noteTexture;
    private PausePanel pausePanel;

    // Start is called before the first frame update
    void Start()
    {
        //
        // UTILITY
        //
        // Initialize
        RANKS = new List<Rank>{
            new Rank(0, rankD),
            new Rank(50, rankC),
            new Rank(65, rankB),
            new Rank(85, rankA),
            new Rank(95, rankS)
        };

        // Add particle systems
        particles.Add(Sector.NORTH_EAST, particlesNE);
        particles.Add(Sector.SOUTH_EAST, particlesSE);
        particles.Add(Sector.SOUTH_WEST, particlesSW);
        particles.Add(Sector.NORTH_WEST, particlesNW);

        // Current rank (S)
        rankIndex = RANKS.Count - 1;

        // Get components
        endScreen = GetComponent<EndScreen>();
        failScreen = GetComponent<FailScreen>();
        pausePanel = GetComponent<PausePanel>();
        playerAnimator = player.GetComponent<Animator>();
        // Create new data
        sectors = new Dictionary<Sector, SectorData>();
        // Iterate
        foreach (Sector sector in Enum.GetValues(typeof(Sector)))
            // Create data
            sectors.Add(sector, new SectorData(this, sector));

        //
        // OFFSETS
        //
        // Screen dimensions (1/2)
        Camera cam = Camera.main;
        height = 1f * cam.orthographicSize;
        width = height * cam.aspect;
        // Convert 45 deg to radians
        double diagonalRadians = Math.PI * 45 / 180.0d;
        // Calculate x offset (horizontal)              a = tg(45) *  b
        xOffset = (Math.Sin(diagonalRadians)/Math.Cos(diagonalRadians)) * height;
        // Calculate y offset (vertical)
        yOffset = height;
        // Note size
        noteXSize = (float) (prefab.GetComponent<SpriteRenderer>().size.x / 2 / Prop.SQRT_OF_TWO);

        //
        // SKINS
        //
        string activeSkinsPath = Application.persistentDataPath+ "/activeSkins.json";
        // JSON
        print(activeSkinsPath);
        Dictionary<string, string> activeSkins = File.Exists(activeSkinsPath) ? JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(activeSkinsPath)) : new Dictionary<string, string>();
        // Apply sprite
        player.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Skins/player_skins/" + (activeSkins.TryGetValue("player_skins", out var v1) ? v1 : "Default"));
        // Set note texture
        noteTexture = Resources.Load<Sprite>("Skins/note_skins/" + (activeSkins.TryGetValue("note_skins", out var v2) ? v2 : "Default"));
        // Particle system
        string[] particleColors = (activeSkins.TryGetValue("particle_skin_color", out var v3) ? v3 : "16777215,16777215").Split(',');
        ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient(ParseColor(Int32.Parse(particleColors[0])), ParseColor(Int32.Parse(particleColors[1])));
        // Set color
        var settings = particlesNE.main;
        settings.startColor = gradient;
        settings = particlesSE.main;
        settings.startColor = gradient;
        settings = particlesSW.main;
        settings.startColor = gradient;
        settings = particlesNW.main;
        settings.startColor = gradient;

        // Set
        playerWidth = player.GetComponent<Renderer>().bounds.size.x / 4;

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
    }

    // Parses the given decimal into Color32 (alpha is set to 255)
    private Color32 ParseColor(int color) {
        return new Color32((byte) ((color >> 16) & 255), (byte) ((color >> 8) & 255), (byte) (color & 255), 255);
    }

    // Continues initialization (usually called after the music track has been loaded)
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

    // Resets (stops) all particle systems
    private void ResetParticles() {
        // Stop all
        foreach (Sector sector in Enum.GetValues(typeof(Sector)))
            particles[sector].Stop();
    }

    // Restarts the game
    public void Restart() {
        // Reset
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

        // Reset sectors
        foreach (Sector sector in Enum.GetValues(typeof(Sector)))
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

    // Pauses the game (needed to check paused boolean)
    public void Pause() {
        // Reset particles
        ResetParticles();
        // Paused
        paused = true;
        // Cancel invoke
        CancelInvoke();
        // Pause the music
        audioSource.Pause();
    }

    // Resumes the game (needed to check paused boolean)
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

    // Quits to main menu
    public void Quit() {
        // Load other scene
        SceneManager.LoadScene("MenuScene");
    }

    // Shows the end (finished) screen
    private void ShowEndScreen() {
        ResetParticles();
        endScreen.Show(score, musicHandler.PropCount() * 2 - misses, misses, (int) (((float) score) / ((float) maxScore) * 100F), (long) Math.Sqrt(score));
    }

    // Shows the fail screen
    private void ShowFailScreen() {
        ResetParticles();
        failScreen.Show();
    }

    // Plays the music via the configured audio source
    void PlayMusic() {
        audioSource.Stop();
        audioSource.Play();
        startedPlaying = true;
    }

    // Resumes (plays) the audio source
    void ResumeMusic() {
        audioSource.Play();
    }

    // Spawns a note according to the given variables
    public void Spawn(Sector sector, float length, float startTime, float diagonalSpeed) {
        // Sector ID
        int sectorID = (int) sector;
        
        // Full size of the prop (1/2)
        float xSize = (float) (length / 2 * diagonalSpeed / Prop.SQRT_OF_TWO) + noteXSize;
        // Positions (NOTE : ((float) offset + (length / 2 / Prop.SQRT_OF_TWO)))
        double x = xOffset + xSize, y = yOffset + xSize;

        // Positioning
        if (sectorID >= 2)
            x = -x;
        if (sectorID == 1 || sectorID == 2)
            y = -y;


        // Instantiate
        GameObject spawned = (GameObject) Instantiate(prefab, new Vector3((float) x, (float) y), Quaternion.Euler(new Vector3(0, 0, 135 - 90*sectorID)));
        // Prop component
        Prop prop = spawned.GetComponent<Prop>();

        // Pass properties
        prop.player = player;
        prop.playerWidth = playerWidth;
        prop.sector = sector;
        prop.startTime = startTime;

        // Set sorting order in layer
        prop.renderer.sortingOrder = sectorID+1;
        // Set size
        prop.renderer.size = new Vector2(1, length);
        // Apply skin
        prop.renderer.sprite = noteTexture;
        
        // Set diagonal speed
        prop.SetSpeed(diagonalSpeed);
        // Init
        prop.Init();
        // Spawn
        sectors[sector].Spawn(prop);
    }

    // Saves the game data
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
        } else {
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

        // If not found
        if (jsonBase.highscores == null)
            jsonBase.highscores = new Dictionary<string, JSONHighscore>();

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

    // JSON utility classes
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

            // Failed
            foreach (Sector sector in Enum.GetValues(typeof(Sector)))
                sectors[sector].Failed();
            
            // Show fail screen after 1 second
            Invoke("ShowFailScreen", 1);
            return;
        }

        // If paused
        if (paused)
            return;

        // If finished playing
        if (!audioSource.isPlaying /*&& audioSource.time == 0*/ && startedPlaying) {
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

        // For each sector
        foreach (Sector sector in Enum.GetValues(typeof(Sector))) {
            // If pressed
            if (Input.GetKeyDown((KeyCode) System.Enum.Parse(typeof(KeyCode), keyboardKeys[sector]))) {
                // Score
                int score = sectors[sector].HandlePress(currentTime);
                // If not -2
                if (score != -2)
                    // Turn on particle system
                    particles[sector].Play();

                // Handle
                HandleScore(sector, score, true);
            } else if (Input.GetKeyUp((KeyCode) System.Enum.Parse(typeof(KeyCode), keyboardKeys[sector]))) {
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

        // Spawn next note if applicable
        musicHandler.SpawnNext(currentTime);
    }

    // Refreshes the rank in the bottom right corner
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

    // Handles score
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
                    // Play SFX and animation
                    if (press) {
                        SFXPlayer.Play(SFXPlayer.EffectType.DRUM_HIT);
                        playerAnimator.Play("Player", -1, 0F);
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

    // Adds to the max score
    public void AddToMaxScore(int s) {
        maxScore += s;
    }

    // Animation methods
    private long animateLong(long displayed, long target) {
        long diff = (long) (Time.deltaTime * LONG_ANIMATION_RATE);
        return displayed < target ?
        displayed + diff > target ? target : displayed + diff :
        displayed - diff < target ? target : displayed - diff;
    }
    private float animateHp(float displayed, float target) {
        float diff = Time.deltaTime * 1f;
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

    // Positioning methods
    public static float SectorXDirection(Sector sector) {
        return (int) sector < 2 ? -1 : 1;
    }

    public static float SectorYDirection(Sector sector) {
        int sectorID = (int) sector;
        return sectorID == 0 || sectorID == 3 ? -1 : 1;
    }
}