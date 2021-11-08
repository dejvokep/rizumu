using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScrollPopulator : MonoBehaviour
{
    public GameObject scrollerUnitPrefab;
    [Space]
    public float unitSize = 123;
    public float margin = 6;

    private List<string> maps;
    private float scrollWidth;

    public static Dictionary<string, SongInfo> mapsInfo = new System.Collections.Generic.Dictionary<string, SongInfo>();
    public static List<string> mapsID = new List<string>();
    public static Dictionary<string, Sprite> mapsSprites = new System.Collections.Generic.Dictionary<string, Sprite>();
    public static Dictionary<string, float> mapsSpritesAspectRatio = new System.Collections.Generic.Dictionary<string, float>();
    
    // Map Info Load
    public SongInfo LoadJsonData(string mapID, bool loadFromResources = false)
    {
        SongInfo mapInfo = new SongInfo();

        if (loadFromResources)
        {
            string json = Resources.Load<TextAsset>("maps/" + mapID + "/info").ToString();
            mapInfo.LoadFromJson(json);

            return mapInfo;

        }
        else if (FileManager.LoadFromFile("", out var json, Application.persistentDataPath+ "/maps/" + mapID + "/info.json"))
        {
            mapInfo.LoadFromJson(json);

            return mapInfo;
        }
        else
        {
            mapInfo.Default();

            return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mapsInfo.Clear();
        mapsID.Clear();
        mapsSprites.Clear();
        mapsSpritesAspectRatio.Clear();

        string[] defaultMapIDs = LoadMapsFromResources();
        foreach (string mapID in defaultMapIDs)
        {
            if (mapID == null || mapID == "")
                continue;

            SongInfo loadedMapInfo = LoadJsonData(mapID, true);
            mapsInfo[mapID] = loadedMapInfo;

            mapsID.Add(mapID);

            LoadImage(mapID, true);

            float aspectRatio = mapsSprites[mapID].rect.width / mapsSprites[mapID].rect.height;
            mapsSpritesAspectRatio[mapID] = aspectRatio;
        }

        try
        {
            List<string> playerMapPaths = Directory.GetDirectories(Application.persistentDataPath + "/maps/").ToList();
            foreach (string mapPath in playerMapPaths)
            {
                string mapID = Path.GetFileName(mapPath);

                SongInfo loadedMapInfo = LoadJsonData(mapID);

                if (loadedMapInfo == null)
                {
                    continue;
                }

                mapsInfo[mapID] = loadedMapInfo;

                mapsID.Add(mapID);

                LoadImage(mapID);

                float aspectRatio = mapsSprites[mapID].rect.width / mapsSprites[mapID].rect.height;
                mapsSpritesAspectRatio[mapID] = aspectRatio;
            }
        }
        catch (DirectoryNotFoundException)
        {
            
        }


        scrollWidth = Screen.width * 0.345f - 51;

        foreach (string mapID in mapsID)
        {
            // Scroller Unit Creation
            GameObject scrollerUnit = Instantiate(scrollerUnitPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            scrollerUnit.transform.SetParent(transform, false);
            scrollerUnit.transform.localScale.Set(1, 1, 1);

            scrollerUnit.name = mapID;


            // Song Image
            scrollerUnit.transform.GetChild(0).GetComponent<Image>().sprite = mapsSprites[mapID];
            scrollerUnit.transform.GetChild(0).GetComponent<AspectRatioFitter>().aspectRatio = mapsSpritesAspectRatio[mapID];
            scrollerUnit.transform.GetChild(0).transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, unitSize - 2*margin);

            float imageWidth = scrollerUnit.transform.GetChild(0).transform.GetComponent<RectTransform>().sizeDelta.x;
            scrollerUnit.transform.GetChild(0).transform.localPosition = new Vector3(imageWidth/2 + margin, 0, 0);


            // Song Name
            scrollerUnit.transform.GetChild(1).GetComponent<Text>().text = mapsInfo[mapID].song_name;
            scrollerUnit.transform.GetChild(1).transform.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollWidth - imageWidth - 2*margin, unitSize/2);
            scrollerUnit.transform.GetChild(1).transform.localPosition = new Vector3((scrollWidth + imageWidth + 2*margin)/2, unitSize/4, 0);


            // Song Difficulty
            scrollerUnit.transform.GetChild(2).GetComponent<Text>().text = "Difficulty: " + mapsInfo[mapID].difficulty.ToString() + "★";
            scrollerUnit.transform.GetChild(2).transform.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollWidth - imageWidth - 2*margin, unitSize/2);
            scrollerUnit.transform.GetChild(2).transform.localPosition = new Vector3((scrollWidth + imageWidth + 2*margin)/2, -unitSize/4, 0);
        }
    }


    private void LoadImage(string mapID, bool loadFromResources = false)
    {
        Texture2D texture;
        if (loadFromResources)
        {
            texture = Resources.Load<Texture2D>("maps/" + mapID + "/image");
        }
        else
        {
            try
            {
                string path = Application.persistentDataPath + "/maps/" + mapID + "/image.png";
                texture = new Texture2D(1, 1);
                // Load image
                texture.LoadImage(File.ReadAllBytes(path));
            }
            catch (FileNotFoundException)
            {
                texture = Resources.Load<Texture2D>("image");
            }
            
        }
        
        // Set sprite
        mapsSprites[mapID] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }


    public static string[] LoadMapsFromResources()
    {
        var textFile = Resources.Load<TextAsset>("maps_list");
        string textString = textFile.ToString();
        string[] lines = textString.Split(new [] { '\r', '\n' });;
        return lines;
    }

    void Update()
    {
        if (ScrollUnitButton.selectedMapID != null && ScrollUnitButton.selectedMapID != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SpawnedController.songID = ScrollUnitButton.selectedMapID;
                SceneManager.LoadScene("GameScene");
            }
        }
    }
}