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
    const int PROP_SIZE = 1;
    // Move speed
    const float MOVE_SPEED = 1;
    // One-directional distance from the center when to despawn the prop
    const float DESTROY_DISTANCE = 0.55F;

    // Hlavna classa, kde by mal byt cely logic co sa tyka spawnutych kruzkov, whatever.
    // To znamena, metoda ked sa nejaky klikne, ked treba nejaky spawnut, etc.

    // Spawn offsets
    private double xOffset, yOffset;
    private double height, width;

    // Currently active (spawned) props
    private Dictionary<GameObject, Sector> active;

    // Start is called before the first frame update
    void Start()
    {
        // Reset
        active = new Dictionary<GameObject, Sector>();
        // Screen dimensions (/2)
        height = 1080*0.0092/2;//Screen.height / 2;
        width = 1920*0.0092/2;//Screen.width / 2;
        // Convert 45 deg to radians
        double diagonalRadians = Math.PI * 45 / 180.0d;
        
        // Calculate x offset (horizontal)              a = tg(45) *  b
        xOffset = (Math.Sin(diagonalRadians)/Math.Cos(diagonalRadians)) * height + PROP_SIZE/2;
        // Calculate y offset (vertical)
        yOffset = height + PROP_SIZE/2;

        Debug.Log(height);
        Debug.Log(width);
        Spawn(Sector.NORTH_WEST);
    }

    void Spawn(Sector sector) {
        // Sector ID
        int sectorID = (int) sector;
        // Positions
        double x = (sectorID < 2 ? xOffset : -xOffset), y = (sectorID == 0 || sectorID == 3 ? yOffset : -yOffset);
        // Instantiate
        GameObject spawned = (GameObject) Instantiate(prefab, new Vector3((float) x, (float) y), Quaternion.Euler(new Vector3(0, 0, -45 + 90*sectorID)));
        // 0 > -45
        // 1 > -135
        // 2 > 

        // ADD ONCLICK TRIGGER (USE BUTTONS?)

        // Add
        active.Add(spawned, sector);
    }

    void DeleteClicked(GameObject prop) {

    }

    // Update is called once per frame
    void Update()
    {
        // Keys
        List<GameObject> props = new List<GameObject>(active.Keys);
        // For each key
        foreach (GameObject prop in props) {
            // Transform
            UnityEngine.Transform transform = prop.transform;
            // Sector
            Sector sector = active[prop];
            // Move
            transform.position = new Vector2(transform.position.x + MOVE_SPEED * SectorXDirection(sector) * Time.deltaTime, transform.position.y + MOVE_SPEED * SectorYDirection(sector) * Time.deltaTime);

            // If to destroy
            if (Math.Abs(transform.position.x) <= DESTROY_DISTANCE) {
                // Destroy
                Destroy(prop);
                // Remove
                active.Remove(prop);
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
