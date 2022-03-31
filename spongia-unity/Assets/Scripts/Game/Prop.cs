using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Prop : MonoBehaviour
{

    // Constants
    public static float SQRT_OF_TWO = (float) Math.Sqrt(2);

    // Variables accessed publicly
    public GameObject player;
    public Sector sector;
    public float length, maxPoints, points = 0, startTime, playerWidth;
    public TonePosition position = TonePosition.WAITING;
    public bool pressed = false, startedPressing = false;
    public SpriteRenderer renderer;

    // Internals
    private double gridSize = 0;
    private float diagonalSpeed, speed;

    // Gets the renderer component for later use
    void Awake() {
        renderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Initializes the prop
    public void Init() {
        // Tone length
        length = renderer.size.y;
        renderer.size = new Vector2(1, length * diagonalSpeed);
        // Max points
        maxPoints = length * 1000;

        // Set grid size
        gridSize = length * diagonalSpeed / SQRT_OF_TWO;
    }

    // Moves the prop
    public TonePosition Move(float time) {
        // Transform
        UnityEngine.Transform transform = gameObject.transform;
        // pos > 0 if status is WAITING
        // pos 
        float pos = playerWidth + ((float) gridSize/2) + (startTime - time) * speed;
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
            if (pos - gridSize/2 > playerWidth)
                return position = TonePosition.WAITING;
            else if (playerWidth > pos + gridSize/2)
                // Just finished playing
                return position = TonePosition.FINISHED;
            else
                // Playing currently
                return position = TonePosition.PLAYING;
        } else {
            // If waiting
            if (-pos + gridSize/2 < -playerWidth)
                return position = TonePosition.WAITING;
            else if (-playerWidth < -pos - gridSize/2)
                // Just finished playing
                return position = TonePosition.FINISHED;
            else
                // Playing currently
                return position = TonePosition.PLAYING;
        }
    }

    // Sets the diagonal speed of this prop
    public void SetSpeed(float diagonalSpeed) {
        this.diagonalSpeed = diagonalSpeed;
        speed = diagonalSpeed / SQRT_OF_TWO;
    }

    
}
