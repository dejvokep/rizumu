using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class MusicHandler
{

    public class MappingComparer : IComparer<List<float>> {
        public int Compare(List<float> a, List<float> b) {
            return a[4].CompareTo(b[4]);
        }
    }

    // Controller
    public SpawnedController controller;
    // Loaded clip
    public AudioClip audioClip;
    // Loaded image
    public Sprite image;

    // Mappings by track names
    private List<List<float>> mappings;
    // Mapping index
    public int mappingIndex = 0;

    // The first spawn time time
    public float firstSpawn = float.MaxValue;

    public bool bundled;

    public MusicHandler(SpawnedController c) {
        // Set
        controller = c;
    }

    public void Load() {
        // Base offset
        double offset = controller.xOffset;

        // If bundled into the game by default
        string mappingsString = "";
        try
        {
            mappingsString = Resources.Load<TextAsset>("maps/" + SpawnedController.songID + "/data").ToString();
        }
        catch (NullReferenceException)
        {

        }

        if (mappingsString == "")
        {
            // Create reader
            StreamReader reader = new StreamReader(Path.Combine(Application.persistentDataPath, "maps/" + SpawnedController.songID + "/data.json"));
            // Load JSON
            mappings = JsonConvert.DeserializeObject<List<List<float>>>(reader.ReadToEnd());
            // Close
            reader.Close();
            bundled = false;
        }
        else
        {
            mappings = JsonConvert.DeserializeObject<List<List<float>>>(mappingsString);
            bundled = true;
        }
        // For each prop
        foreach (List<float> propData in mappings) {
            // Tone length
            float length = propData[2];

            // Full size of the prop (1/2)
            float xSize = (float) (length / 2 / Prop.SQRT_OF_TWO) + ((float) (controller.prefab.GetComponent<SpriteRenderer>().size.x / 2 / Prop.SQRT_OF_TWO));
            // Time where the prop will touch the player - how long it will take from the spawn position to that position
            propData.Add(propData[0] - ((float) offset - controller.playerWidth + xSize) / (propData[3] / Prop.SQRT_OF_TWO));
            //Debug.Log("Assigned spawn time=" + (propData[0] - ((float) offset - c.playerWidth + xSize) / (propData[3] / Prop.SQRT_OF_TWO)));

            // If earlier spawn time
            if (propData[4] < firstSpawn)
                firstSpawn = propData[4];
        }

        // Sort
        mappings.Sort(delegate(List<float> x, List<float> y){
            return x[4].CompareTo(y[4]);
        });

        string songDataString;
        // If bundled
        if (bundled) {
            songDataString = Resources.Load<TextAsset>("maps/" + SpawnedController.songID + "/info").ToString();
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
        controller.songName = songData["song_name"].ToString();
        controller.songAuthor = songData["song_author"].ToString();
        controller.songDifficulty = Int32.Parse(songData["difficulty"].ToString());

        // If bundled
        if (bundled) {
            // Load the clip
            audioClip = Resources.Load<AudioClip>("maps/" + SpawnedController.songID + "/audio");
            // Load the image
            image = Resources.Load<Sprite>("maps/" + SpawnedController.songID + "/image");
            // Continue in the controller
            controller.ContinueInit();
        } else {
            // Load user image
            LoadUserImage();
            // Load the clip
            LoadClip();
        }
    }

    async void LoadClip() {
        // Make request
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file://" + Path.Combine(Application.persistentDataPath, "maps/" + SpawnedController.songID + "/audio.mp3"), AudioType.MPEG)) {
            // Send the request
            request.SendWebRequest();

            try {
                // While not done
                while (!request.isDone)
                    // Wait
                    await Task.Delay(5);

                // If failed
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                    // Log
                    Debug.Log(request.error);
                else
                    this.audioClip = DownloadHandlerAudioClip.GetContent(request);
            } catch (Exception err) {
                Debug.Log(err);
            }
        }
        // Continue in the controller
        controller.ContinueInit();
    }

    private void LoadUserImage() {
        // Create texture
        Texture2D texture;
        // Load image
        try
        {
            byte[] fileBytes = File.ReadAllBytes(Path.Combine(Application.persistentDataPath, "maps/" + SpawnedController.songID + "/image.png"));
            texture = new Texture2D(1, 1);
            texture.LoadImage(fileBytes);
        }
        catch (FileNotFoundException)
        {
            texture = Resources.Load<Texture2D>("image");
        }

        // Set sprite
        image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public void Reset() {
        mappingIndex = 0;
    }

    public void SpawnNext(float time) {
        // If out of range
        if (mappingIndex >= mappings.Count)
            return;

        // While should have been spawned already
        while (mappingIndex < mappings.Count && mappings[mappingIndex][4] <= time) {
            // Data
            List<float> propData = mappings[mappingIndex];
            // Spawn
            controller.Spawn(SectorByID((int) propData[1]), propData[2], propData[0], propData[3]);
            // Add
            mappingIndex += 1;
        }
    }

    public int PropCount() {
        return mappings.Count;
    }

    private Sector SectorByID(int id) {
        return id == 0 ? Sector.NORTH_EAST : id == 1 ? Sector.SOUTH_EAST : id == 2 ? Sector.SOUTH_WEST : Sector.NORTH_WEST;
    }

}