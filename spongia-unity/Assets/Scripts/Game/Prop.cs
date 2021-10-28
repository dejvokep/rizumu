using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Prop : MonoBehaviour
{

    public GameObject player;
    private Vector2 playerBounds;
    private float length;
    private Sector sector;
    // Move speed
    const float MOVE_SPEED = 2;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        // Tone length
        length = gameObject.transform.localScale.y;

        int rotation = (int) gameObject.transform.rotation.eulerAngles.z;
        if (rotation == 315)
            sector = Sector.NORTH_EAST;
        else if (rotation == 45)
            sector = Sector.SOUTH_EAST;
        else if (rotation == 135)
            sector = Sector.SOUTH_WEST;
        else
            sector = Sector.NORTH_WEST;

        // Player width
        float playerWidth = player.GetComponent<Renderer>().bounds.size.x / 4;
        playerBounds = new Vector2(playerWidth, playerWidth);
        Debug.Log(playerBounds);
    }

    // ??? Update is called once per frame ???
    public bool Move()
    {
        // Transform
        UnityEngine.Transform transform = gameObject.transform;
        // New coords
        float newX = transform.position.x + MOVE_SPEED * SectorXDirection(sector) * Time.deltaTime;
        float newY = transform.position.y + MOVE_SPEED * SectorYDirection(sector) * Time.deltaTime;

        // Move
        transform.position = new Vector2(newX, newY);

        /*
        if (dOBRE == papa)
        {
            neee.staci(ak, distance);
            stop();
        }
        */

        // Delete if prop surpasses playerBounds by 2*length to avoid square root
        if ((int)sector < 2) 
            return playerBounds.x > newX + length;
        else
            return -playerBounds.x < newX - length;
    }

    float SectorXDirection(Sector sector) {
        return (int) sector < 2 ? -1 : 1;
    }

    float SectorYDirection(Sector sector) {
        int sectorID = (int) sector;
        return sectorID == 0 || sectorID == 3 ? -1 : 1;
    }
}
