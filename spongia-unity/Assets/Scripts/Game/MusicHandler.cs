using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using Newtonsoft.Json;

public class MusicHandler : MonoBehaviour
{

    // Offset between object spawn <=> object at perfect click position (beat)
    // MUST CORRESPOND TO MOVE SPEED SET IN THE CONTROLLER!!!
    const float SPAWN_TIME_OFFSET = 2;

    // Controller used to spawn props etc...
    public SpawnedController controller;

    // Mappings by track names
    private Dictionary<string, Dictionary<float, int>> mappings = new Dictionary<string, Dictionary<float, int>>();

    // Start is called before the first frame update
    void Start()
    {
        // Load file names
        string[] names = Directory.GetFiles("Assets/Resources/Mappings/");
        // All names
        foreach (string name in names) {
            // If ends with .meta
            if (name.EndsWith(".meta"))
                continue;

            // Create reader
            StreamReader reader = new StreamReader(name);
            // Load JSON
            Dictionary<string, int> json = JsonConvert.DeserializeObject<Dictionary<string, int>>(reader.ReadToEnd());
            // Close
            reader.Close();

            Debug.Log(json.Count);


            // Create dictionary
            Dictionary<float, int> timestamps = new Dictionary<float, int>();
            // All json entries
            foreach (KeyValuePair<string, int> entry in json) {
                // Parse and add
                timestamps.Add(float.Parse(entry.Key, CultureInfo.InvariantCulture.NumberFormat) - SPAWN_TIME_OFFSET, entry.Value);
            }

            //Debug.Log(timestamps);
        }
        
        
        
        
        //Debug.Log(json);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
