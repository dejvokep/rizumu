using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using Newtonsoft.Json;

public class MusicHandler
{

    // Controller
    public SpawnedController controller;
    // Loaded clip
    public AudioClip audioClip;

    // Mappings by track names
    private List<List<float>> mappings;
    // Mapping index
    private int mappingIndex = 0;

    // The first spawn time time
    public float firstSpawn = float.MaxValue;

    public MusicHandler(SpawnedController c) {
        // Set
        controller = c;
        // Base offset
        double offset = c.xOffset;

        // Create reader
        StreamReader reader = new StreamReader("Assets/Resources/Mappings/" + SpawnedController.songName + ".json");
        // Load JSON
        mappings = JsonConvert.DeserializeObject<List<List<float>>>(reader.ReadToEnd());
        // Close
        reader.Close();
        // For each prop
        foreach (List<float> propData in mappings) {
            // Tone length
            float length = propData[2];

            // Full size of the prop (1/2)
            float xSize = (float) (length / 2 / Prop.SQRT_OF_TWO) + ((float) (c.prefab.transform.localScale.x / 2 / Prop.SQRT_OF_TWO));
            // Add
            propData.Add(propData[0]);
            // Time where the prop will touch the player - how long it will take from the spawn position to that position
            propData[0] = propData[0] - ((float) offset + ((float) (c.prefab.transform.localScale.x / 2 / Prop.SQRT_OF_TWO)) - c.playerWidth) / SpawnedController.MOVE_SPEED;

            // If earlier spawn time
            if (propData[0] < firstSpawn)
                firstSpawn = propData[0];
        }

        // Load the clip
        audioClip = Resources.Load<AudioClip>("Music/" + SpawnedController.songName);
    }

    public void SpawnNext(float time) {
        // If out of range
        if (mappingIndex >= mappings.Count)
            return;

        // While should have been spawned already
        while (mappingIndex < mappings.Count && mappings[mappingIndex][0] <= time) {
            // Data
            List<float> propData = mappings[mappingIndex];
            // Spawn
            controller.Spawn(SectorByID((int) propData[1]), propData[2], propData[3]);
            // Add
            mappingIndex += 1;
        }
    }

    private Sector SectorByID(int id) {
        return id == 0 ? Sector.NORTH_EAST : id == 1 ? Sector.SOUTH_EAST : id == 2 ? Sector.SOUTH_WEST : Sector.NORTH_WEST;
    }

}
