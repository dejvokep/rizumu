using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Sector;

public class SpawnedController : MonoBehaviour
{

    // Masks
    public GameObject NEmask, SEmask, SWmask, NWmask;
    // Prefab to spawn
    public GameObject prefab;
    // Player
    public GameObject player;

    // Offscreen spawn offset
    const int OFFSCREEN_SPAWN_OFFSET = 3;
    // Move speed
    const float MOVE_SPEED = 1;
    // One-directional distance from the center when to despawn the prop
    private float destroyDistance;

    // Hlavna classa, kde by mal byt cely logic co sa tyka spawnutych kruzkov, whatever.
    // To znamena, metoda ked sa nejaky klikne, ked treba nejaky spawnut, etc.

    // Spawn offsets
    private double xOffset, yOffset;
    private double height, width;

    // Currently active (spawned) props
    private Dictionary<Sector, List<GameObject>> active;
    // Keyboard keys by sector
    private Dictionary<string, Sector> keyboardKeys;

    // Start is called before the first frame update
    void Start()
    {
        // Create new
        active = new Dictionary<Sector, List<GameObject>>();
        // Iterate
        foreach (Sector sector in Enum.GetValues(typeof(Sector))) {
            // Add sets
            active.Add(sector, new List<GameObject>());
        }

        // Create new
        keyboardKeys = new Dictionary<string, Sector>();
        keyboardKeys.Add("a", Sector.NORTH_WEST);
        keyboardKeys.Add("s", Sector.NORTH_EAST);
        keyboardKeys.Add("d", Sector.SOUTH_WEST);
        keyboardKeys.Add("f", Sector.SOUTH_EAST);

        // Screen dimensions (/2)
        Camera cam = Camera.main;
        height = 1f * cam.orthographicSize;
        width = height * cam.aspect;
        // Convert 45 deg to radians
        double diagonalRadians = Math.PI * 45 / 180.0d;
        
        // Calculate x offset (horizontal)              a = tg(45) *  b
        xOffset = (Math.Sin(diagonalRadians)/Math.Cos(diagonalRadians)) * height + OFFSCREEN_SPAWN_OFFSET;
        // Calculate y offset (vertical)
        yOffset = height + OFFSCREEN_SPAWN_OFFSET;

        // SPAWN PROP
        // DEBUG ONLY!!!
        Spawn(Sector.NORTH_WEST, 1);
    }

    void Spawn(Sector sector, float length) {
        // Sector ID
        int sectorID = (int) sector;
        // Positions
        double x = (sectorID < 2 ? xOffset : -xOffset), y = (sectorID == 0 || sectorID == 3 ? yOffset : -yOffset);
        // Instantiate
        GameObject spawned = (GameObject) Instantiate(prefab, new Vector3((float) x, (float) y), Quaternion.Euler(new Vector3(0, 0, -45 + 90*sectorID))/*, GetMask(sector).transform*/);
        // Change size
        spawned.transform.localScale = new Vector2(1, length);
        // Set sorting order in layer
        spawned.GetComponent<SpriteRenderer>().sortingOrder = sectorID+1;

        // ADD ONCLICK TRIGGER (USE BUTTONS?)

        // Add
        active[sector].Add(spawned);
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
        /*// For each key
        foreach (string key in keyboardKeys.Keys) {
            // If the key is pressed
            if (Input.GetKeyDown(key))
            {
                // Props
                List<GameObject> props = active[keyboardKeys[key]];
                // If there is an object
                if (props.Count > 0) {
                    // Destroy
                    Destroy(props[0]);
                    // Remove
                    props.RemoveAt(0);
                    Debug.Log(props);
                }
            }
        }*/

        // For each sector
        foreach (Sector sector in Enum.GetValues(typeof(Sector))) {
            // Keys
            List<GameObject> props = new List<GameObject>(active[sector]);

            // For each prop
            foreach (GameObject prop in props) {
                // Remove
                if (prop.GetComponent<Prop>().Move()) {
                    // Destroy
                    Destroy(prop);
                    // Remove
                    active[sector].RemoveAt(0);
                }
            }
        }
    }

    float SectorXDirection(Sector sector) {
        return (int) sector < 2 ? -1 : 1;
    }

    float SectorYDirection(Sector sector) {
        int sectorID = (int) sector;
        return sectorID == 0 || sectorID == 3 ? -1 : 1;
    }
}
