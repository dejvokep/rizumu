using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

public class MusicHandler : MonoBehaviour
{

    const float SPAWN_TIME_OFFSET = 2;

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
            SpawnTimestamps json = JsonUtility.FromJson<SpawnTimestamps>(reader.ReadToEnd());
            // Close
            reader.Close();

            Debug.Log(json.timestamps.Count);

            // Create dictionary
            Dictionary<float, int> timestamps = new Dictionary<float, int>();
            // All json entries
            foreach (KeyValuePair<string, int> entry in json.timestamps) {
                // Parse and add
                timestamps.Add(float.Parse(entry.Key, CultureInfo.InvariantCulture.NumberFormat) - SPAWN_TIME_OFFSET, entry.Value);
                Debug.Log(float.Parse(entry.Key, CultureInfo.InvariantCulture.NumberFormat) - SPAWN_TIME_OFFSET);
                Debug.Log(entry.Value);
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
