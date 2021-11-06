using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollPopulator : MonoBehaviour
{
    public GameObject scrollerUnitPrefab;
    [Space]
    public float unitSize = 123;
    public float margin = 6;

    private string[] maps;
    private float scrollWidth;

    public static List<SongInfo> mapsInfo = new List<SongInfo>();
    public static List<string> mapsID = new List<string>();
    public static List<bool> mapsFromPresistentDataPath = new List<bool>();
    public static List<Sprite> mapsSprites = new List<Sprite>();
    public static List<Vector2> mapsSpritesSizes = new List<Vector2>();
    
    // Map Info Load
    public SongInfo LoadJsonData(string mapID)
    {
        SongInfo mapInfo = new SongInfo();
        
        string path = "/maps/" + mapID;


        if (FileManager.LoadFromFile("info.json", out var json, Application.persistentDataPath + path))
        {
            mapInfo.LoadFromJson(json);

            Debug.Log("Load complete - presistenDataPath");

            mapsFromPresistentDataPath.Add(true);

            return mapInfo;
        }
        else if (FileManager.LoadFromFile("info.json", out var json1, "Assets/Resources/" + path))
        {
            mapInfo.LoadFromJson(json1);

            Debug.Log("Load complete - assets");

            mapsFromPresistentDataPath.Add(false);
            return mapInfo;
        }
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

        maps = Directory.GetDirectories(Application.persistentDataPath + "/maps/");

        for (int i = 0; i < maps.Length; i++)
        {
            // Get map info
            string[] splitMapPath = maps[i].Split("/".ToCharArray()[0]);
            string mapID = splitMapPath[splitMapPath.Length - 1];

            Debug.Log(mapID);

            SongInfo loadedMapInfo = LoadJsonData(mapID);

            if (loadedMapInfo == null)
            {
                continue;
            }

            mapsID.Add(mapID);
            mapsInfo.Add(loadedMapInfo);

            // Scroller Unit Creation
            GameObject scrollerUnit = Instantiate(scrollerUnitPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            scrollerUnit.transform.SetParent(transform, false);
            scrollerUnit.transform.localScale.Set(1, 1, 1);

            scrollerUnit.name = mapID;

            LoadImage(i);
            // float imageWidth = mapsSpritesSizes[i].x/mapsSpritesSizes[i].y*(unitSize - 2*margin);
            // Debug.Log($"{i}: imageWidth: {imageWidth}");

            // Song Image
            scrollerUnit.transform.GetChild(0).GetComponent<Image>().sprite = mapsSprites[i];

            float aspectRatio = mapsSprites[i].rect.width / mapsSprites[i].rect.height;
            scrollerUnit.transform.GetChild(0).GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
            
            scrollerUnit.transform.GetChild(0).transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, unitSize - 2*margin);

            float imageWidth = scrollerUnit.transform.GetChild(0).transform.GetComponent<RectTransform>().sizeDelta.x;
            Debug.Log($"{i}: imageWidth: {imageWidth}");

            scrollerUnit.transform.GetChild(0).transform.localPosition = new Vector3(imageWidth/2 + margin, 0, 0);
            Debug.Log($"{i}: localPosition: x = {imageWidth/2 + margin}, y = {unitSize/2 + margin}, z = 0");



            // Song Name
            scrollerUnit.transform.GetChild(1).GetComponent<Text>().text = mapsInfo[i].song_name;
            scrollerUnit.transform.GetChild(1).transform.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollWidth - imageWidth - 2*margin, unitSize/2);
            scrollerUnit.transform.GetChild(1).transform.localPosition = new Vector3((scrollWidth + imageWidth + 2*margin)/2, unitSize/4, 0);
            Debug.Log($"{i}: sizeDelta: x = {(scrollWidth - imageWidth - 2*margin)/2}, y = {unitSize/2}");

            // Song Difficulty
            scrollerUnit.transform.GetChild(2).GetComponent<Text>().text = "Difficulty: " + mapsInfo[i].difficulty.ToString() + "★";
            scrollerUnit.transform.GetChild(2).transform.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollWidth - imageWidth - 2*margin, unitSize/2);
            scrollerUnit.transform.GetChild(2).transform.localPosition = new Vector3((scrollWidth + imageWidth + 2*margin)/2, -unitSize/4, 0);
            Debug.Log($"{i}: localPosition: x = {(scrollWidth + imageWidth + 2*margin)/2}, y = {-unitSize/4}, z = 0");
        }
    }


    private void LoadImage(int index) {
        string path;

        if (mapsFromPresistentDataPath[index])
        {
            path = Application.persistentDataPath + "/maps/" + mapsID[index] + "/image.png";
            Debug.Log(path);
        }
        else
        {
            path = "Assets/Resources/maps/" + mapsID[index] + "/image.png";
        }

        Texture2D texture = new Texture2D(1, 1);
        // Load image
        texture.LoadImage(File.ReadAllBytes(path));
        // Set sprite
        mapsSprites.Add(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f)));
    }

    void Update()
    {
        Debug.Log(ScrollUnitButton.selectedMapID);
    }
}
