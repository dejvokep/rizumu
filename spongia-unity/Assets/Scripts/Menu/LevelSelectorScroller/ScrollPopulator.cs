using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
    private List<bool> mapsFromPresistentDataPath = new List<bool>();
    public static Dictionary<string, Sprite> mapsSprites = new System.Collections.Generic.Dictionary<string, Sprite>();
    public static Dictionary<string, float> mapsSpritesAspectRatio = new System.Collections.Generic.Dictionary<string, float>();
    
    // Map Info Load
    public SongInfo LoadJsonData(string mapPath)
    {
        SongInfo mapInfo = new SongInfo();


        if (FileManager.LoadFromFile("", out var json, mapPath + "/info.json"))
        {
            mapInfo.LoadFromJson(json);

            Debug.Log("Load complete - presistenDataPath");

            mapsFromPresistentDataPath.Add(true);

            return mapInfo;
        }
        // else if (FileManager.LoadFromFile("info.json", out var json1, "Assets/Resources/maps/" + mapID + "/"))
        // {
        //     mapInfo.LoadFromJson(json1);

        //     Debug.Log("Load complete - assets");

        //     mapsFromPresistentDataPath.Add(false);
        //     return mapInfo;
        // }
        else
        {
            mapInfo.Default();

            Debug.Log("Load failed");

            return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        scrollWidth = Screen.width * 0.345f - 51;

        maps = Directory.GetDirectories(Application.persistentDataPath + "/maps/").Concat(Directory.GetDirectories("Assets/Resources/maps/")).ToList();

        foreach (string mapPath in maps)
        {
            // Get map info
            string mapID = Path.GetFileName(mapPath);

            Debug.Log(mapID);

            SongInfo loadedMapInfo = LoadJsonData(mapPath);

            if (loadedMapInfo == null)
            {
                continue;
            }

            mapsID.Add(mapID);
            mapsInfo[mapID] = loadedMapInfo;

            // Scroller Unit Creation
            GameObject scrollerUnit = Instantiate(scrollerUnitPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            scrollerUnit.transform.SetParent(transform, false);
            scrollerUnit.transform.localScale.Set(1, 1, 1);

            scrollerUnit.name = mapID;

            LoadImage(mapPath);

            // Song Image
            scrollerUnit.transform.GetChild(0).GetComponent<Image>().sprite = mapsSprites[mapID];

            float aspectRatio = mapsSprites[mapID].rect.width / mapsSprites[mapID].rect.height;
            mapsSpritesAspectRatio[mapID] = aspectRatio;
            scrollerUnit.transform.GetChild(0).GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
            
            scrollerUnit.transform.GetChild(0).transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, unitSize - 2*margin);

            float imageWidth = scrollerUnit.transform.GetChild(0).transform.GetComponent<RectTransform>().sizeDelta.x;
            // Debug.Log($"{i}: imageWidth: {imageWidth}");

            scrollerUnit.transform.GetChild(0).transform.localPosition = new Vector3(imageWidth/2 + margin, 0, 0);
            // Debug.Log($"{i}: localPosition: x = {imageWidth/2 + margin}, y = {unitSize/2 + margin}, z = 0");



            // Song Name
            scrollerUnit.transform.GetChild(1).GetComponent<Text>().text = mapsInfo[mapID].song_name;
            scrollerUnit.transform.GetChild(1).transform.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollWidth - imageWidth - 2*margin, unitSize/2);
            scrollerUnit.transform.GetChild(1).transform.localPosition = new Vector3((scrollWidth + imageWidth + 2*margin)/2, unitSize/4, 0);
            // Debug.Log($"{i}: sizeDelta: x = {(scrollWidth - imageWidth - 2*margin)/2}, y = {unitSize/2}");

            // Song Difficulty
            scrollerUnit.transform.GetChild(2).GetComponent<Text>().text = "Difficulty: " + mapsInfo[mapID].difficulty.ToString() + "★";
            scrollerUnit.transform.GetChild(2).transform.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollWidth - imageWidth - 2*margin, unitSize/2);
            scrollerUnit.transform.GetChild(2).transform.localPosition = new Vector3((scrollWidth + imageWidth + 2*margin)/2, -unitSize/4, 0);
            // Debug.Log($"{i}: localPosition: x = {(scrollWidth + imageWidth + 2*margin)/2}, y = {-unitSize/4}, z = 0");
        }
    }


    private void LoadImage(string mapPath) {
        string path = mapPath+"/image.png";

        Texture2D texture = new Texture2D(1, 1);
        // Load image
        texture.LoadImage(File.ReadAllBytes(path));
        // Set sprite
        mapsSprites[Path.GetFileName(mapPath)] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    void Update()
    {
        Debug.Log(ScrollUnitButton.selectedMapID);
    }
}
