using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

public class AudioLength : MonoBehaviour
{
    //creating objects
    [Header("Input fields")]
    public InputField LengthInput;
    public InputField DifficultyInput;
    public InputField AutorName;
    public InputField SaveName;
    public InputField SpawnTimeInput;
    public InputField ArivalTimeInput;
    public InputField SpeedInput  ;
    [Header("Objects")]
    public GameObject TargetForDelete ;
    public GameObject TargetForEdit;
    public float Dtime;
    public List<int> x = new List<int> { 413, 413,-413,-413 };
    public List<int> y = new List<int> { 338, -488, -488, 338 };
    public List<int> angle = new List<int> { -135, 135, 45, -45 };
    public GameObject Canvas ;
    public GameObject Sector1TargetParent;
    public List<int> Sectors ;
    public IDictionary<int,List<float>> CurrentMap = new Dictionary<int, List<float>>() ;
    public string input;
    public Text CurrentTime;
    public Text MaximumTime;
    [Header("Internals")]
    public bool stlacenie = true;
    public GameObject TargetForLengthChange;
    public float LengthMesurre;
    public string ImagePath;
    public string MusicPath;
    public string ImageName;
    public string MusicName;
    public string name;
    public float ArivalTime;
    public float OldTime;
    public float OldMove = 0;
    bool playing = true;
    public Slider mSlider;
    AudioSource m_AudioSource;
    public AudioSource TargetAudioSource;
    AudioClip m_AudioClip;
    public List<List<float>> objects = new List<List<float>> ();

    private bool inputEnabled = true;
    public static string audioPath, imagePath;


    private Dictionary<Sector, string> keyboardKeys;

    // Start is called before the first frame update
    void Start()
    {
        // Reset
        audioPath = "";
        imagePath = "";
        // Assign keys
        if (SpawnedController.keyboardKeys != null)
            keyboardKeys = SpawnedController.keyboardKeys;
        else {
            keyboardKeys = new Dictionary<Sector, string>();
            keyboardKeys[Sector.NORTH_EAST] = "f";
            keyboardKeys[Sector.SOUTH_EAST] = "k";
            keyboardKeys[Sector.SOUTH_WEST] = "j";
            keyboardKeys[Sector.NORTH_WEST] = "d";
        }

        // Reset
        SpeedInput.text="1";
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioClip = m_AudioSource.clip;
    }
    public void Stlacenie(int s) {
        // Time
        LengthMesurre=m_AudioSource.time;
        // Spawn
        GameObject spawned = (GameObject) Instantiate(Sector1TargetParent, new Vector3((x[s]/84.01437f), (y[s]/84.01437f),0),Quaternion.Euler(new Vector3(0, 0, angle[s])),Canvas.transform);
        // Set tags and move
        spawned.tag=s.ToString() ;
        spawned.transform.Translate (5.6f, 0f, 0f);
        // Add data
        CurrentMap[spawned.GetInstanceID()]=new List<float>{m_AudioSource.time-5.45f,s,float.Parse(SpeedInput.text),0.5f,m_AudioSource.time-5.45f,m_AudioSource.time};
        // Set sorting layer
        spawned.GetComponent<SpriteRenderer>().sortingOrder = s;
        // Currently edited
        TargetForLengthChange=spawned;
        stlacenie=false;
    }
    public void Pustenie() {
        // Length of the prop
        float Length = m_AudioSource.time-LengthMesurre;
        // Scale and move
        TargetForLengthChange.transform.localScale = new Vector2((85f*Length*CurrentMap[TargetForLengthChange.GetInstanceID()][2]),66f);
        TargetForLengthChange.transform.Translate(new Vector2(-(Length*float.Parse(SpeedInput.text))/2,0f));
        // Update length
        CurrentMap[TargetForLengthChange.GetInstanceID()][3]=Length;
        stlacenie=true;
    }

    // Update is called once per frame
    void Update() {
        // If cannot move anything
        if (!inputEnabled)
            return;

        // Update time data
        mSlider.value = (float)m_AudioSource.time;
        CurrentTime.text = m_AudioSource.time.ToString();
        // If clicked
        if (Input.GetMouseButtonDown(0)) {
            // Raycast
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            // If clicked a prop
            if (hit.collider != null) {
                // Set text
                TargetForEdit = hit.collider.gameObject;
                SpawnTimeInput.text=CurrentMap[TargetForEdit.GetInstanceID()][3].ToString();
                ArivalTimeInput.text=(CurrentMap[TargetForEdit.GetInstanceID()][5].ToString());
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            // Raycast
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            
            // If clicked a prop
            if (hit.collider != null) {
                // Target
                TargetForDelete = hit.collider.gameObject;
                // Remove and destroy
                CurrentMap.Remove(TargetForDelete.GetInstanceID());
                Destroy(TargetForDelete);
            }  
        }

        

        // If to pause/resume
        if (Input.GetKeyDown("space")) {
            // Manage
            if (playing)
                m_AudioSource.Pause();
            else
                m_AudioSource.Play();
            // Set
            playing = !playing;
        }

        // If not playing
        if (!playing)
            return;

        // For each sector
        foreach (Sector sector in Enum.GetValues(typeof(Sector))) {
            // The key
            string key = keyboardKeys[sector];
            // If pressed
            if (Input.GetKeyDown(key))
                Stlacenie((int) sector);
            if (Input.GetKeyUp(key))
                Pustenie();
        }
    }

    public void disableInput() {
        // Set
        inputEnabled = false;
        // Pause
        m_AudioSource.Pause();
        playing = false;
    }
    public void enableInput() {
        // Set
        inputEnabled = true;
        // Resume
        m_AudioSource.UnPause();
        playing = true;
    }


    public void ReadStringInput(string myInput)
    {
        float Input = float.Parse(myInput);
        mSlider.value=Input;
    }

    public void ArivalTimeChange(string ArivalTimeString)
    {
        // ID
        int id = TargetForEdit.GetInstanceID();
        List<float> data = CurrentMap[id];

        // Move
        float ArivalTime=float.Parse(ArivalTimeString);
        TargetForEdit.transform.Translate (((-(ArivalTime-(data[5]))*data[2])), 0f, 0f);
        data[0]=ArivalTime-(5.45f/data[2]);
        data[4]=ArivalTime-(5.45f/data[2]);
        data[5]=ArivalTime;
    }

    public void SpeedChange(string SpeedString)
    {
        // Calculations
        OldTime=mSlider.value;
        mSlider.value=0;
        int Speed = int.Parse(SpeedString);

        // ID
        int id = TargetForEdit.GetInstanceID();
        List<float> data = CurrentMap[id];

        // Change speed and size
        TargetForEdit.transform.position=new Vector3(0f,(-75/84.01437f),0f);
        TargetForEdit.transform.Translate(((-(data[0]+5.45f)*Speed)+(OldTime*Speed)-0.54f/Speed),0f,0f);
        SpawnTimeInput.text=(data[0]+(5.45f/Speed)*((Speed-data[2]))).ToString();
        data[4] = (data[0]+(5.45f/Speed)*((Speed-data[2])));
        data[0] = (data[0]+(5.45f/Speed)*((Speed-data[2])));
        TargetForEdit.transform.localScale = new Vector2(85f*data[3]*Speed,66f);
        TargetForEdit.transform.Translate(new Vector2(-(((Speed/data[2])*data[3])/2),0f));
        data[2]=Speed;
        mSlider.value=OldTime;
    }

    public void Something(string LengthInputString)
    {
        // ID
        int id = TargetForEdit.GetInstanceID();
        List<float> data = CurrentMap[id];

        // Move
        float Length = float.Parse(LengthInputString);
        TargetForEdit.transform.localScale = new Vector2(85f*Length*data[2],66f);
        TargetForEdit.transform.Translate(new Vector2(-(((Length-data[3])*data[2])/2),0f));
        data[3]=Length;
    }


    public void SaveFunction() {
        int difficulty0;
        try
        {
            difficulty0 = int.Parse(DifficultyInput.text);
        }
        catch (FormatException)
        {
            difficulty0 = 0;
        }
        var info = new jsonInfoVeci
        {
            song_name = SaveName.text,
            song_author = AutorName.text,
            difficulty = difficulty0
        };

        MusicPath=audioPath;
        List<string> subsMusic = new List<string>(
        MusicPath.Split(new string[] { @"\" }, StringSplitOptions.None));
        MusicName=subsMusic[subsMusic.Count-1];
        
        ImagePath=imagePath;
        List<string> subsImage = new List<string>(
        ImagePath.Split(new string[] { @"\" }, StringSplitOptions.None));
        ImageName=subsImage[subsImage.Count-1];
        foreach (var item in CurrentMap)
            objects.Add(new List<float> {CurrentMap[item.Key][5], CurrentMap[item.Key][1],CurrentMap[item.Key][3],CurrentMap[item.Key][2]});

        // Create folder
        string folderPath = Application.persistentDataPath + "/maps/" + SaveName.text + "-";
        string id = Guid.NewGuid().ToString();
        // Safety check for unique IDs
        while (File.Exists(folderPath + id))
            id = Guid.NewGuid().ToString();
        // Add
        folderPath += id;
        // Create directory
        System.IO.Directory.CreateDirectory(folderPath);

        // Save notes
        string jsonMap = JsonConvert.SerializeObject(objects, Formatting.Indented);
        System.IO.File.WriteAllText(folderPath + "/data.json", jsonMap);

        // Save the map info
        string jsonInfo = JsonConvert.SerializeObject(info, Formatting.Indented);
        System.IO.File.WriteAllText(folderPath + "/info.json", jsonInfo);

        // Audio paths
        string sourceFile = MusicPath;
        string destFile = folderPath + "/audio.mp3";
        // Audio copying
        System.IO.File.Copy(sourceFile, destFile, true);

        // Image paths
        sourceFile = ImagePath;
        destFile = folderPath + "/image.png";
        // Image copying
        if(!String.IsNullOrEmpty(sourceFile))
            System.IO.File.Copy(sourceFile, destFile, true);
        else
            System.IO.File.Copy(@"Assets/Textures/Default.png", destFile, true);
    }

        public void Exit() {
            SceneManager.LoadScene("MenuScene");
        }
}
class jsonInfoVeci
    {
        public string song_name {get; set;}
        public string song_author {get; set;}  
        public int difficulty {get; set;}          
    }
