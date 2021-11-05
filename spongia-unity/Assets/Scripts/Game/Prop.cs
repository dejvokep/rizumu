using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Prop : MonoBehaviour
{

    public static double SQRT_OF_TWO = Math.Sqrt(2);

    public GameObject player;
    private Vector2 playerBounds;
    public float length;
    private Sector sector;
    // Move speed
    private double gridSize = 0;

    public float maxPoints, points = 0;
    public float startTime;

    private float playerWidth;

    public TonePosition position = TonePosition.WAITING;

    public bool pressed = false, startedPressing = false;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        // Tone length
        length = gameObject.transform.localScale.y;
        gameObject.transform.localScale = new Vector2(1, length * SpawnedController.DIAGONAL_MOVE_SPEED);
        // Max points
        maxPoints = length * 1000;

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
        playerWidth = player.GetComponent<Renderer>().bounds.size.x / 4;
        // Set player bounds
        playerBounds = new Vector2(playerWidth, playerWidth);
        // Set grid size
        gridSize = length * SpawnedController.DIAGONAL_MOVE_SPEED / SQRT_OF_TWO;
    }

    public TonePosition Move(float time)
    {
        // Transform
        UnityEngine.Transform transform = gameObject.transform;
        // New coords
        //float newX = transform.position.x + MOVE_SPEED * SpawnedController.SectorXDirection(sector) * Time.deltaTime;
        //float newY = transform.position.y + MOVE_SPEED * SpawnedController.SectorYDirection(sector) * Time.deltaTime;

        //Debug.Log(startTime);
        
        // pos > 0 ak je status WAITING
        // pos 
        float pos = playerWidth + ((float) gridSize/2) + (startTime - time) * SpawnedController.MOVE_SPEED;

        //Debug.Log("Delta X t=" + Time.deltaTime + " , moved=" + (Math.Abs(newX - transform.position.x)));

        // Move
        transform.position = new Vector2(pos * -SpawnedController.SectorXDirection(sector), pos * -SpawnedController.SectorYDirection(sector));

        // If waiting
        if (startTime > time)
            return TonePosition.WAITING;
        // If finished
        if (position == TonePosition.FINISHED)
            return position;

        // If east sector
        if ((int) sector < 2) {
            // If waiting
            if (pos - gridSize/2 > playerBounds.x)
                return position = TonePosition.WAITING;
            else if (playerBounds.x > pos + gridSize/2)
                // Just finished playing
                return position = TonePosition.FINISHED;
            else
                // Playing currently
                return position = TonePosition.PLAYING;
        } else {
            // If waiting
            if (-pos + gridSize/2 < playerBounds.x)
                return position = TonePosition.WAITING;
            else if (-playerBounds.x < -pos - gridSize/2)
                // Just finished playing
                return position = TonePosition.FINISHED;
            else
                // Playing currently
                return position = TonePosition.PLAYING;
        }

        /*
        if (dOBRE == papa)
        {
            neee.staci(ak, distance);
            stop();
        }
        */
    }

    public void SetStartTime(float time) {
        startTime = time;
    }

    
}
