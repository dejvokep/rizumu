using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using static Sector;

public class SpawnedController : MonoBehaviour
{

    public static string songName = "tajne_zaznamy";

    // Music handler
    private MusicHandler musicHandler;
    // Mixer
    public AudioMixerGroup mixer;
    // Audio source
    public AudioSource audioSource;

    public Text timeText, pointText;
    public Image progressBar;

    // Masks
    public GameObject NEmask, SEmask, SWmask, NWmask;
    // Prefab to spawn
    public GameObject prefab;
    // Player
    public GameObject player;

    // Move speed
    public static float MOVE_SPEED = 2;

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

    private long points = 0;

    private float songLength;


    // Start is called before the first frame update
    void Start()
    {
        playerWidth = GameObject.Find("Player").GetComponent<Renderer>().bounds.size.x / 4;
        // Create new data
        sectors = new Dictionary<Sector, SectorData>();
        // Iterate
        foreach (Sector sector in Enum.GetValues(typeof(Sector)))
            // Add data
            sectors.Add(sector, new SectorData());

        //
        // CONTROLS
        //
        keyboardKeys = new Dictionary<Sector, string>();
        keyboardKeys.Add(Sector.NORTH_WEST, "a");
        keyboardKeys.Add(Sector.NORTH_EAST, "s");
        keyboardKeys.Add(Sector.SOUTH_WEST, "d");
        keyboardKeys.Add(Sector.SOUTH_EAST, "f");

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
            audioSource.Play();
    }

    void PlayMusic() {
        //Debug.Log("Started playing at: " + currentTime);
        audioSource.Play();
    }

    public void Spawn(Sector sector, float length, float startTime) {
        // Sector ID
        int sectorID = (int) sector;
        
        // Full size of the prop (1/2)
        float xSize = (float) (length / 2 / Prop.SQRT_OF_TWO) + ((float) (prefab.transform.localScale.x / 2 / Prop.SQRT_OF_TWO));
        // Positions (NOTE : ((float) offset + (length / 2 / Prop.SQRT_OF_TWO)))
        double x = xOffset + xSize, y = yOffset + xSize;

        if (sectorID >= 2)
            x = -x;
        if (sectorID == 1 || sectorID == 2)
            y = -y;


        // Instantiate
        GameObject spawned = (GameObject) Instantiate(prefab, new Vector3((float) x, (float) y), Quaternion.Euler(new Vector3(0, 0, -45 + 90*sectorID))/*, GetMask(sector).transform*/);
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

    private GameObject GetMask(Sector sector) {
        return sector == NORTH_EAST ? NEmask : sector == NORTH_WEST ? NWmask : sector == SOUTH_EAST ? SEmask : SWmask;
    }

    public void Clicked(GameObject prop) {
        Debug.Log(prop);
    }

    // Update is called once per frame
    void Update()
    {
        // Update current time
        currentTime = audioSource.isPlaying ? audioSource.time : currentTime + Time.deltaTime;
        timeText.text = currentTime.ToString("0.00") + "s";

        // If is playing
        if (audioSource.isPlaying) {
            progressBar.fillAmount = currentTime / songLength;
        }
        //Debug.Log(currentTime);

        // For each sector
        foreach (Sector sector in Enum.GetValues(typeof(Sector))) {
            // If pressed
            if (Input.GetKeyDown(keyboardKeys[sector]))
                points += sectors[sector].HandlePress(currentTime);
            else if (Input.GetKeyUp(keyboardKeys[sector]))
                points += sectors[sector].HandleRelease(currentTime);

            // Update
            sectors[sector].Update(currentTime);
        }
        pointText.text = "Points: " + points.ToString();
        // Spawn
        musicHandler.SpawnNext(currentTime);
    }

    public static float SectorXDirection(Sector sector) {
        return (int) sector < 2 ? -1 : 1;
    }

    public static float SectorYDirection(Sector sector) {
        int sectorID = (int) sector;
        return sectorID == 0 || sectorID == 3 ? -1 : 1;
    }
}
