using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Sector;

public class SpawnedController : MonoBehaviour
{

    // Prefab to spawn
    public GameObject prefab;

    // Size of one side of the spawned props in px (the prop must have square boundaries)
    const int propSize = 100;
    // Move speed
    const float moveSpeed = 100;

    // Hlavna classa, kde by mal byt cely logic co sa tyka spawnutych kruzkov, whatever.
    // To znamena, metoda ked sa nejaky klikne, ked treba nejaky spawnut, etc.

    // Spawn offsets
    private double xOffset, yOffset;
    private int height, width;

    // Currently active (spawned) props
    private Dictionary<GameObject, Sector> active;

    // Start is called before the first frame update
    void Start()
    {
        // Reset
        active = new Dictionary<GameObject, Sector>();
        // Screen dimensions (/2)
        height = Screen.height / 2;
        width = Screen.width / 2;
        // Convert 45 deg to radians
        double diagonalRadians = Math.PI * 45 / 180.0d;
        
        // Calculate x offset (horizontal)              a = tg(45) *  b
        xOffset = (Math.Sin(diagonalRadians)/Math.Cos(diagonalRadians)) * height + propSize/2;
        // Calculate y offset (vertical)
        yOffset = height + propSize/2;
        Spawn(Sector.NORTH_WEST);
    }

    void Spawn(Sector sector) {
        // Sector ID
        int sectorID = (int) sector;
        // Positions
        double x = width + (sectorID < 2 ? xOffset : -xOffset), y = height + (sectorID % 2 == 0 ? yOffset : yOffset);
        // Instantiate
        GameObject spawned = (GameObject) Instantiate(prefab, new Vector3((float) x, (float) y), Quaternion.Euler(new Vector3()));

        // ADD ONCLICK TRIGGER (USE BUTTONS?)

        // Add
        active.Add(spawned, sector);
    }

    void DeleteClicked(GameObject prop) {

    }

    // Update is called once per frame
    void Update()
    {
        // For each entry
        foreach (KeyValuePair<GameObject, Sector> entry in active) {
            // Transform
            UnityEngine.Transform transform = entry.Key.transform;
            // Sector
            Sector sector = entry.Value;
            // Update transform
            transform.position = new Vector2(transform.position.x + moveSpeed * SectorXDirection(sector) * Time.deltaTime, transform.position.y + moveSpeed * SectorYDirection(sector) * Time.deltaTime);
        }
    }

    float SectorXDirection(Sector sector) {
        return (int) sector < 2 ? 1 : -1;
    }

    float SectorYDirection(Sector sector) {
        return (int) sector % 2 == 0 ? 1 : -1;
    }
}
