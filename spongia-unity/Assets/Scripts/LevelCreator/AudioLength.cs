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
    public bool stlacenie = true;
    public GameObject TargetForLengthChange;
    public float LengthMesurre;
    public string ImagePath;
    public string MusicPath;
    public string ImageName;
    public string MusicName;
    //public string[] subs;
    public string name;
    public float ArivalTime;
    public float OldTime;
    public float OldMove = 0;
    public InputField LengthInput;
    public InputField DifficultyInput;
    public InputField AutorName;
    public InputField SaveName;
    public InputField SpawnTimeInput;
    public InputField ArivalTimeInput;
    public InputField SpeedInput  ;
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
        // try
        // {
        //     print("Set audio volume");
        //     m_AudioSource.volume = SettingsMenu.audioVolume;
        // }
        // catch (NullReferenceException)
        // {
        //     print("Audio volume not set, defaulting");
        //     m_AudioSource.volume = -30;
        // }

        if (SpawnedController.keyboardKeys != null)
            keyboardKeys = SpawnedController.keyboardKeys;
        else
        {
            keyboardKeys = new Dictionary<Sector, string>();
            keyboardKeys[Sector.NORTH_EAST] = "f";
            keyboardKeys[Sector.SOUTH_EAST] = "k";
            keyboardKeys[Sector.SOUTH_WEST] = "j";
            keyboardKeys[Sector.NORTH_WEST] = "d";
        }

        print(Application.persistentDataPath);
        SpeedInput.text="1";
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioClip = m_AudioSource.clip;
        try
        {
            Debug.Log("Audio clip length : " + m_AudioSource.clip.length);
            float Len = (float)m_AudioSource.clip.length;
            mSlider.maxValue= Len;
            MaximumTime.text = Len.ToString();
        }
        catch (NullReferenceException)
        {
            return;
        }
    }
    public void Stlacenie(int s)
    {
        
        LengthMesurre=m_AudioSource.time;
        GameObject spawned = (GameObject) Instantiate(Sector1TargetParent, new Vector3((x[s]/84.01437f), (y[s]/84.01437f),0),Quaternion.Euler(new Vector3(0, 0, angle[s])),Canvas.transform);
        spawned.tag=s.ToString() ;
        spawned.transform.Translate (5.6f, 0f, 0f);
        CurrentMap[spawned.GetInstanceID()]=new List<float>{m_AudioSource.time-5.45f,s,float.Parse(SpeedInput.text),0.5f,m_AudioSource.time-5.45f,m_AudioSource.time};
        spawned.GetComponent<SpriteRenderer>().sortingOrder = s;
        TargetForLengthChange=spawned;
        stlacenie=false;
    }
    public void Pustenie()
    {
        float Length = m_AudioSource.time-LengthMesurre;
        print(Length);
        TargetForLengthChange.transform.localScale = new Vector2((85f*Length*CurrentMap[TargetForLengthChange.GetInstanceID()][2]),66f);
        TargetForLengthChange.transform.Translate(new Vector2(-(Length*float.Parse(SpeedInput.text))/2,0f));
        CurrentMap[TargetForLengthChange.GetInstanceID()][3]=Length;
        stlacenie=true;

    }
    // Update is called once per frame
    void Update()

    {
        if (!inputEnabled)
            return;


        mSlider.value = (float)m_AudioSource.time;
        CurrentTime.text = m_AudioSource.time.ToString();
        // foreach (var item in CurrentMap.Keys)
        // {
        //     if (CurrentMap[item][5]==m_AudioSource.time)
        //     {
        //         TargetAudioSource.Play();
        //     }
        //}
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            
            if (hit.collider != null)
            {
                 TargetForEdit = hit.collider.gameObject;
                 SpawnTimeInput.text=CurrentMap[TargetForEdit.GetInstanceID()][3].ToString();
                 ArivalTimeInput.text=(CurrentMap[TargetForEdit.GetInstanceID()][5].ToString());
                
            }
                    
        }
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            
            if (hit.collider != null)
            {
                 TargetForDelete = hit.collider.gameObject;
                 Debug.Log(hit.collider.gameObject);
                 Debug.Log(hit.collider.gameObject.tag);
                 CurrentMap.Remove(TargetForDelete.GetInstanceID());
                 Destroy(TargetForDelete);
                
            }
                    
        }

        

        //mSlider.value = (float)m_AudioSource.time;

        //CurrentTime.text = m_AudioSource.time.ToString();
        if (Input.GetKeyDown("space"))
        {
            Debug.Log(playing);
            if (playing)
            {
    
                m_AudioSource.Pause();
                playing = false ;
                
            }
            else
            {
    
                m_AudioSource.Play();
                playing = true ;
            }
        
        }
        if (playing==true)
        {
        
            if (Input.GetKeyDown(keyboardKeys[Sector.NORTH_EAST]))
            {
                Stlacenie(0);
            }
        
                
        }
        if (playing==true)
        {
            if (Input.GetKeyUp(keyboardKeys[Sector.NORTH_EAST]))
            {
                Pustenie();
            }
        }
        if (playing==true)
        {
            
            if (Input.GetKeyDown(keyboardKeys[Sector.SOUTH_EAST]))
            {
                Stlacenie(1);
            }
            
                
        }
        if (playing==true)
        {
            if (Input.GetKeyUp(keyboardKeys[Sector.SOUTH_EAST]))
            {
                Pustenie();
            }
        }
        if (playing==true)
        {
            
            if (Input.GetKeyDown(keyboardKeys[Sector.SOUTH_WEST]))
            {
                Stlacenie(2);
            }
            
                
        }
        if (playing==true)
        {
            if (Input.GetKeyUp(keyboardKeys[Sector.SOUTH_WEST]))
            {
                Pustenie();
            }
        }
        if (playing==true)
        {
            
            if (Input.GetKeyDown(keyboardKeys[Sector.NORTH_WEST]))
            {
                Stlacenie(3);
            }
            
                
        }
        if (playing==true)
        {
            if (Input.GetKeyUp(keyboardKeys[Sector.NORTH_WEST]))
            {
                Pustenie();
            }
        }            
    }

    public void disableInput()
    {
        inputEnabled = false;

        m_AudioSource.Pause();
        playing = false;
    }
    public void enableInput()
    {
        inputEnabled = true;

        m_AudioSource.UnPause();
        playing = true;
    }


    public void ReadStringInput(string myInput)
    {
        float Input = float.Parse(myInput);
        mSlider.value=Input;
    }
    public void CreateTargetSector(int sector)
    {
        
        
        
        
        GameObject spawned = (GameObject) Instantiate(Sector1TargetParent, new Vector3((x[sector]/84.01437f), (y[sector]/84.01437f),0),Quaternion.Euler(new Vector3(0, 0, angle[sector])),Canvas.transform);
        CurrentMap[spawned.GetInstanceID()]=new List<float>{m_AudioSource.time,sector,float.Parse(SpeedInput.text),0.5f,m_AudioSource.time,m_AudioSource.time+5.45f};
        spawned.GetComponent<SpriteRenderer>().sortingOrder = sector+1 ;
        spawned.tag= sector.ToString();
        //Debug.Log(spawned.GetInstanceID());
        foreach (var item in CurrentMap[spawned.GetInstanceID()])
        {
            //Debug.Log(item);
        }

        

        
    }

    public void ArivalTimeChange(string ArivalTimeString)
    {
        
       
       float ArivalTime=float.Parse(ArivalTimeString); //daco bolo zle
       TargetForEdit.transform.Translate (((-(ArivalTime-(CurrentMap[TargetForEdit.GetInstanceID()][5]))*CurrentMap[TargetForEdit.GetInstanceID()][2])), 0f, 0f);
       CurrentMap[TargetForEdit.GetInstanceID()][0]=ArivalTime-(5.45f/CurrentMap[TargetForEdit.GetInstanceID()][2]);
       //SpawnTimeInput.text=CurrentMap[TargetForEdit.GetInstanceID()][0].ToString();
       CurrentMap[TargetForEdit.GetInstanceID()][4]=ArivalTime-(5.45f/CurrentMap[TargetForEdit.GetInstanceID()][2]);
       CurrentMap[TargetForEdit.GetInstanceID()][5]=ArivalTime;
       

    }

    public void SpeedChange(string SpeedString)
    {
        OldTime=mSlider.value;
        mSlider.value=0;
        int Speed = int.Parse(SpeedString);
        TargetForEdit.transform.position=new Vector3(0f,(-75/84.01437f),0f);
        print((CurrentMap[TargetForEdit.GetInstanceID()][0]+5.45f)*Speed);
        TargetForEdit.transform.Translate(((-(CurrentMap[TargetForEdit.GetInstanceID()][0]+5.45f)*Speed)+(OldTime*Speed)-0.54f/Speed),0f,0f);
        SpawnTimeInput.text=(CurrentMap[TargetForEdit.GetInstanceID()][0]+(5.45f/Speed)*((Speed-CurrentMap[TargetForEdit.GetInstanceID()][2]))).ToString();
        CurrentMap[TargetForEdit.GetInstanceID()][4] = (CurrentMap[TargetForEdit.GetInstanceID()][0]+(5.45f/Speed)*((Speed-CurrentMap[TargetForEdit.GetInstanceID()][2])));
        CurrentMap[TargetForEdit.GetInstanceID()][0] = (CurrentMap[TargetForEdit.GetInstanceID()][0]+(5.45f/Speed)*((Speed-CurrentMap[TargetForEdit.GetInstanceID()][2])));
        TargetForEdit.transform.localScale = new Vector2(85f*CurrentMap[TargetForEdit.GetInstanceID()][3]*Speed,66f);
        TargetForEdit.transform.Translate(new Vector2(-(((Speed/CurrentMap[TargetForEdit.GetInstanceID()][2])*CurrentMap[TargetForEdit.GetInstanceID()][3])/2),0f));
        //TargetForEdit.transform.Translate(((Speed-CurrentMap[TargetForEdit.GetInstanceID()][2])/2),0f,0f);
        CurrentMap[TargetForEdit.GetInstanceID()][2]=Speed;
        mSlider.value=OldTime;

        
    }

    public void Something(string LengthInputString)
    {
        float Length = float.Parse(LengthInputString);
        TargetForEdit.transform.localScale = new Vector2(85f*Length*CurrentMap[TargetForEdit.GetInstanceID()][2],66f);
        TargetForEdit.transform.Translate(new Vector2(-(((Length-CurrentMap[TargetForEdit.GetInstanceID()][3])*CurrentMap[TargetForEdit.GetInstanceID()][2])/2),0f));
        CurrentMap[TargetForEdit.GetInstanceID()][3]=Length;
        //print(-(((Length-CurrentMap[TargetForEdit.GetInstanceID()][3])*CurrentMap[TargetForEdit.GetInstanceID()][2])/2)+Math.Abs(OldMove));
        //OldMove = (-((Length-CurrentMap[TargetForEdit.GetInstanceID()][3])*CurrentMap[TargetForEdit.GetInstanceID()][2])/2)+OldMove;
    }
    public void SaveFunction()
        {
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
            {
                List<float> daco = new List<float> {CurrentMap[item.Key][5], CurrentMap[item.Key][1],CurrentMap[item.Key][3],CurrentMap[item.Key][2]};
                objects.Add(daco);
            }
            
            
            //FOLDER STUFF
            name = SaveName.text+"-"+Guid.NewGuid().ToString();
            string folderPath = Application.persistentDataPath + "/maps/" + name;
            System.IO.Directory.CreateDirectory(folderPath);


            //MAP STUFF
            string jsonMap = JsonConvert.SerializeObject(objects, Formatting.Indented);
            System.IO.File.WriteAllText(folderPath + "/data.json", jsonMap);


            //INFO STUFF
            string jsonInfo = JsonConvert.SerializeObject(info, Formatting.Indented);
            System.IO.File.WriteAllText(folderPath + "/info.json", jsonInfo);


            //MUSIC STUFF
            string sourceFile = MusicPath;
            string destFile = folderPath + "/audio.mp3";
            System.IO.File.Copy(sourceFile, destFile, true);


            //IMAGE STUFF
            string sourceFile2 = ImagePath;
            string destFile2 = folderPath + "/image.png";
            if(!String.IsNullOrEmpty(sourceFile2))
            {
                
                System.IO.File.Copy(sourceFile2, destFile2, true);

            }
                
            else
            {
                System.IO.File.Copy(@"Assets/Textures/Default.png", destFile2, true);
            }


        }

        public void Exit() {
            SceneManager.LoadScene("MainScene");
        }
}
class jsonInfoVeci
    {
        public string song_name {get; set;}
        public string song_author {get; set;}  
        public int difficulty {get; set;}          
    }
